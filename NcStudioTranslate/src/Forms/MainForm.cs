using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using NcStudioTranslate.Helpers;
using NcStudioTranslate.Models;

namespace NcStudioTranslate.Forms
{
    public partial class MainForm : Form
    {
        private const int DefaultSplitterDistance = 200;

        private const string DefaultNcStudioResourcesFolder = @"C:\Program Files\Weihong\NcStudio\Bin\Resources";

        private const string LogFileName = "NcStudioTranslate.log";

        private readonly BindingList<ResxFileItem> _resxFiles = new();
        private readonly BindingList<ResourceEntry> _visibleEntries = new();
        private List<ResourceEntry> _allEntries = new();
        private readonly Dictionary<string, string> _originalValues = new(StringComparer.Ordinal);

        private readonly BindingList<LogEntry> _logEntries = new();
        private string _logFilePath = string.Empty;
        private XDocument? _currentDocument;
        private string _selectedZhPath = string.Empty;
        private string _referenceEnPath = string.Empty;
        private string _backupZhFilePath = string.Empty;
        private ResxFileItem? _currentItem;
        private string _currentFilter = string.Empty;

        private AppSettings _settings = new();
        private readonly string _settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");

        private int _zoomPercent = 100;
        private Font? _baseFormFont;
        private Font? _baseGridFont;
        private Font? _baseListFont;
        private Font? _baseSearchFont;
        private Font? _baseStatusFont;

        private CancellationTokenSource? _loadingCts;
        private bool _isLoading;

        private static string GetAppVersion()
        {
            var assembly = typeof(MainForm).Assembly;
            var informational = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            static string CleanInformational(string value)
            {
                var trimmed = value.Trim();

                // .NET SDK often appends build metadata like "+<gitsha>". Keep only the semantic portion.
                var plusIndex = trimmed.IndexOf('+');
                if (plusIndex >= 0)
                {
                    trimmed = trimmed[..plusIndex];
                }

                return string.IsNullOrWhiteSpace(trimmed) ? value.Trim() : trimmed;
            }

            static string FormatAssemblyVersion(Version version)
            {
                // Prefer "major.minor.build" when revision is 0 to avoid showing "1.0.1.0".
                if (version.Revision <= 0)
                {
                    return $"{version.Major}.{version.Minor}.{version.Build}";
                }

                return version.ToString();
            }

            if (!string.IsNullOrWhiteSpace(informational))
            {
                return CleanInformational(informational);
            }

            var version = assembly.GetName().Version;
            return version != null ? FormatAssemblyVersion(version) : "unknown";
        }

        public MainForm()
        {
            InitializeComponent();
            entriesBindingSource.DataSource = _visibleEntries;
            lstResxFiles.DataSource = _resxFiles;
            lstResxFiles.DisplayMember = nameof(ResxFileItem.DisplayName);
            lstResxFiles.ValueMember = nameof(ResxFileItem.FilePath);

            _baseFormFont = Font;
            _baseGridFont = dgvEntries.Font;
            _baseListFont = lstResxFiles.Font;
            _baseSearchFont = txtSearch.Font;
            _baseStatusFont = lblStatus.Font;

            // Habilita double-buffering no DataGridView para melhorar performance
            EnableDoubleBuffering(dgvEntries);

            // Garante que o painel de conteúdo nunca suma por splitter salvo fora do limite
            splitContainerMain.Panel1MinSize = 160;
            splitContainerMain.Panel2MinSize = 420;

            LoadSettings();

            // Estado inicial (antes de carregar qualquer pasta)
            exibirColunaChaveToolStripMenuItem.Checked = _settings.ShowKeyColumn;

            Shown += (_, _) =>
            {
                // Aplica o splitter distance após a janela estar visível (importante para maximizado)
                if (_settings.SplitterDistance > 0)
                {
                    // Usa o valor salvo, mas garante que está dentro dos limites
                    var clamped = ClampSplitterDistance(_settings.SplitterDistance);
                    splitContainerMain.SplitterDistance = clamped;
                }
                else
                {
                    // Primeira vez: começa com ~1/5 da largura
                    var suggested = ClampSplitterDistance(ComputeDefaultSplitterDistance());
                    splitContainerMain.SplitterDistance = suggested;
                }
            };

            ApplyZoom(_settings.ZoomPercent > 0 ? _settings.ZoomPercent : 100);

            // Persistir ajustes do usuário
            splitContainerMain.SplitterMoved += (_, _) =>
            {
                _settings.SplitterDistance = ClampSplitterDistance(splitContainerMain.SplitterDistance);
                SaveSettings();
            };

            FormClosing += (_, _) =>
            {
                _settings.SplitterDistance = ClampSplitterDistance(splitContainerMain.SplitterDistance);
                SaveSettings();
            };

            SizeChanged += (_, _) =>
            {
                // Em resize, mantém o painel direito visível.
                try
                {
                    if (_settings.SplitterDistance > 0 && splitContainerMain.Width > splitContainerMain.Panel1MinSize + splitContainerMain.Panel2MinSize)
                    {
                        var clamped = ClampSplitterDistance(splitContainerMain.SplitterDistance);
                        if (clamped != splitContainerMain.SplitterDistance)
                        {
                            splitContainerMain.SplitterDistance = clamped;
                        }
                    }
                }
                catch
                {
                    // Ignora erros durante redimensionamento
                }
            };

            // Pasta inicial: usa a última pasta válida; caso não exista, usa a pasta padrão do NcStudio.
            var initialFolder = ResolveInitialFolder();
            if (!string.IsNullOrWhiteSpace(initialFolder))
            {
                txtFolderPath.Text = initialFolder;
                folderBrowserDialog.SelectedPath = initialFolder;
                LoadResxFiles(initialFolder);
            }

            // Centralizar o painel de loading quando o form redimensionar
            Resize += (_, _) => CenterLoadingPanel();
            
            // Mostrar tutorial na primeira execução (após o form estar visível)
            Load += (_, _) =>
            {
                if (!_settings.FirstRunCompleted)
                {
                    _settings.FirstRunCompleted = true;
                    SaveSettings();
                    BeginInvoke(new Action(() => ShowTutorial()));
                }
            };
        }

        private string? ResolveInitialFolder()
        {
            if (!string.IsNullOrWhiteSpace(_settings.LastFolder) && Directory.Exists(_settings.LastFolder))
            {
                return _settings.LastFolder;
            }

            if (Directory.Exists(DefaultNcStudioResourcesFolder))
            {
                return DefaultNcStudioResourcesFolder;
            }

            return null;
        }

        private static void EnableDoubleBuffering(DataGridView grid)
        {
            // Habilita double-buffering via reflection (propriedade protegida)
            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                grid,
                new object[] { true });
        }

        private void CenterLoadingPanel()
        {
            if (loadingPanel != null)
            {
                loadingPanel.Left = (ClientSize.Width - loadingPanel.Width) / 2;
                loadingPanel.Top = (ClientSize.Height - loadingPanel.Height) / 2;
            }
        }

        private void ShowLoading(string message)
        {
            _isLoading = true;
            _loadingCts = new CancellationTokenSource();
            
            // Desabilitar controles PRIMEIRO (antes de mostrar o loading)
            tabControlMain.Enabled = false;
            topPanel.Enabled = false;
            menuStrip.Enabled = false;
            
            // Configura o painel de loading
            lblLoadingStatus.Text = message;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Value = 0;
            btnCancelLoading.Enabled = true;
            
            // Garante que o loadingPanel está habilitado e visível
            loadingPanel.Enabled = true;
            CenterLoadingPanel();
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            
            Cursor = Cursors.WaitCursor;
            
            // Força renderização imediata - essencial para mostrar antes do processamento
            loadingPanel.Update();
            progressBar.Update();
            lblLoadingStatus.Update();
            btnCancelLoading.Update();
            Application.DoEvents();
        }

        private void ShowLoadingWithProgress(string message, int maximum)
        {
            _isLoading = true;
            _loadingCts = new CancellationTokenSource();
            
            lblLoadingStatus.Text = message;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Minimum = 0;
            progressBar.Maximum = maximum;
            progressBar.Value = 0;
            
            CenterLoadingPanel();
            loadingPanel.Visible = true;
            loadingPanel.BringToFront();
            
            Cursor = Cursors.WaitCursor;
            
            // Desabilitar controles durante carregamento
            tabControlMain.Enabled = false;
            topPanel.Enabled = false;
            menuStrip.Enabled = false;
        }

        private void UpdateLoadingProgress(int value, string? message = null)
        {
            if (progressBar.Style == ProgressBarStyle.Blocks)
            {
                progressBar.Value = Math.Min(value, progressBar.Maximum);
            }
            if (message != null)
            {
                lblLoadingStatus.Text = message;
            }
        }

        private void HideLoading()
        {
            _isLoading = false;
            _loadingCts?.Dispose();
            _loadingCts = null;
            
            loadingPanel.Visible = false;
            Cursor = Cursors.Default;
            
            // Reabilitar controles
            tabControlMain.Enabled = true;
            topPanel.Enabled = true;
            menuStrip.Enabled = true;
        }

        private void btnCancelLoading_Click(object sender, EventArgs e)
        {
            _loadingCts?.Cancel();
            lblLoadingStatus.Text = "Cancelando...";
            btnCancelLoading.Enabled = false;
        }

        private void exibirColunaChaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var show = exibirColunaChaveToolStripMenuItem.Checked;
            _settings.ShowKeyColumn = show;
            SaveSettings();
            // Atualiza visibilidade se a coluna já existir
            if (dgvEntries.Columns.Contains("Key"))
            {
                dgvEntries.Columns["Key"].Visible = show;
            }
        }

        private void abrirLogErrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            
            if (!File.Exists(errorLogPath))
            {
                ZoomMessageBox.ShowInfo("O arquivo de log de erros não existe.\n\nIsso significa que nenhum erro foi registrado.", "Log de erros");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = errorLogPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ZoomMessageBox.ShowError($"Erro ao abrir o log de erros: {ex.Message}");
            }
        }

        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTutorial();
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            ApplyZoom(_zoomPercent - 10);
        }

        private void btnZoomReset_Click(object sender, EventArgs e)
        {
            ApplyZoom(100);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            ApplyZoom(_zoomPercent + 10);
        }

        private void ApplyZoom(int percent)
        {
            if (_baseFormFont == null || _baseGridFont == null || _baseListFont == null || _baseSearchFont == null || _baseStatusFont == null)
            {
                return;
            }

            var clamped = Math.Max(70, Math.Min(200, percent));
            _zoomPercent = clamped;

            // Sincroniza o zoom com o ZoomMessageBox
            ZoomMessageBox.ZoomPercent = _zoomPercent;

            float scale = _zoomPercent / 100f;

            Font = new Font(_baseFormFont.FontFamily, _baseFormFont.Size * scale, _baseFormFont.Style);
            dgvEntries.Font = new Font(_baseGridFont.FontFamily, _baseGridFont.Size * scale, _baseGridFont.Style);
            lstResxFiles.Font = new Font(_baseListFont.FontFamily, _baseListFont.Size * scale, _baseListFont.Style);
            txtSearch.Font = new Font(_baseSearchFont.FontFamily, _baseSearchFont.Size * scale, _baseSearchFont.Style);
            lblStatus.Font = new Font(_baseStatusFont.FontFamily, _baseStatusFont.Size * scale, _baseStatusFont.Style);

            dgvEntries.RowTemplate.Height = (int)Math.Round(28 * scale);
            lblZoomLevel.Text = $"{_zoomPercent}%";

            // Cabeçalhos acompanharem o zoom (evita texto cortado)
            try
            {
                dgvEntries.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvEntries.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                dgvLog.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dgvLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            }
            catch
            {
                // ignorar ajustes visuais
            }

            FixTextBoxHeightsForCurrentZoom();

            _settings.ZoomPercent = _zoomPercent;
            SaveSettings();
        }

        private void FixTextBoxHeightsForCurrentZoom()
        {
            // Evita "dançar" desalinhamento entre label/textbox/botões em FlowLayout.
            try
            {
                if (txtSearch != null)
                {
                    txtSearch.AutoSize = false;
                    txtSearch.Height = Math.Max(27, txtSearch.PreferredHeight);
                }

                if (txtFolderPath != null)
                {
                    txtFolderPath.AutoSize = false;
                    txtFolderPath.Height = Math.Max(23, txtFolderPath.PreferredHeight);
                }
            }
            catch
            {
                // ignorar ajustes visuais
            }
        }

        private int ComputeDefaultSplitterDistance()
        {
            // Aproximação: 1/5 da área do formulário.
            var width = Math.Max(1, ClientSize.Width);
            var suggested = (int)Math.Round(width * 0.2);
            return Math.Max(160, suggested);
        }

        private int ClampSplitterDistance(int desired)
        {
            try
            {
                var min = splitContainerMain.Panel1MinSize;
                var max = Math.Max(min, splitContainerMain.Width - splitContainerMain.Panel2MinSize - splitContainerMain.SplitterWidth);
                return Math.Max(min, Math.Min(max, desired));
            }
            catch
            {
                return desired;
            }
        }

        private void ShowTutorial()
        {
            var appVersion = GetAppVersion();
            var text =
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "                    EDITOR DE TRADUÇÕES - TUTORIAL\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n\r\n" +
                "PASSO 1 - SELECIONAR PASTA\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Clique em 'Escolher pasta' e selecione a pasta dos arquivos .resx.\r\n" +
                "• Endereço padrão (mais comum):\r\n" +
                "  C:\\Program Files\\Weihong\\NcStudio\\Bin\\Resources\r\n" +
                "• A lista mostra somente arquivos que possuem par (zh-CN + en-US).\r\n\r\n" +
                "PASSO 2 - CRIAR TRADUÇÃO\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Selecione um arquivo na lista à esquerda.\r\n" +
                "• Clique no botão 'Criar tradução'.\r\n" +
                "• A ferramenta irá:\r\n" +
                "  1. Criar um backup do chinês original: *.zh-CN.resx.original\r\n" +
                "  2. Substituir o arquivo chinês pelo conteúdo em inglês\r\n" +
                "  3. Habilitar a edição do arquivo\r\n" +
                "• O backup original é PROTEGIDO e não será sobrescrito.\r\n\r\n" +
                "PASSO 3 - TRADUZIR\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Edite a coluna 'Atual' com a tradução em português.\r\n" +
                "• A coluna 'Referência' mostra o texto original em inglês (somente leitura).\r\n" +
                "• As alterações são salvas automaticamente ao sair da célula.\r\n" +
                "• Use a busca para filtrar entradas (suporta % como curinga).\r\n" +
                "• Clique com botão direito para:\r\n" +
                "  - Copiar célula\r\n" +
                "  - Abrir tradutores AI (ChatGPT, Perplexity, Google AI)\r\n\r\n" +
                "PASSO 4 - EXCLUIR TRADUÇÃO (se necessário)\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Clique em 'Excluir tradução' para descartar TODO o progresso.\r\n" +
                "• O arquivo original chinês será restaurado do backup.\r\n" +
                "• O arquivo de backup (.original) será excluído.\r\n" +
                "• ATENÇÃO: Esta ação NÃO pode ser desfeita!\r\n\r\n" +
                "PASSO 5 - APLICAR NO NCSTUDIO\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Abra o NcStudio.\r\n" +
                "• Vá no menu 'LanguageChanged'.\r\n" +
                "• Selecione 'Chines' (geralmente a primeira opção).\r\n" +
                "• O NcStudio carregará os arquivos .zh-CN traduzidos.\r\n\r\n" +
                "FUNCIONALIDADES ADICIONAIS\r\n" +
                "───────────────────────────────────────────────────────────────────\r\n" +
                "• Zoom: Use os botões A- / 100% / A+ para ajustar o tamanho.\r\n" +
                "• Opções > Exibir coluna 'Chave': Mostra/oculta a coluna de chaves.\r\n" +
                "• Aba 'Log': Histórico de todas as edições realizadas.\r\n" +
                "• Restaurar do Log: Clique em uma entrada do log para reverter.\r\n" +
                "• Limpar Log: Remove todo o histórico de edições.\r\n\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "                           DICAS IMPORTANTES\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "• Sem '.original' = Arquivo em modo SOMENTE LEITURA (criar tradução primeiro)\r\n" +
                "• Com '.original' = Arquivo em modo EDIÇÃO\r\n" +
                "• Botão 'Criar tradução' = Ativo quando NÃO existe backup\r\n" +
                "• Botão 'Excluir tradução' = Ativo quando EXISTE backup\r\n\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "                           COMPATIBILIDADE\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                $"• Versão do NcStudio Translate: {appVersion}\r\n" +
                "• Versão compatível do NcStudio Phoenix: 15.550.25\r\n" +
                "• Embora desenvolvido para a versão acima, a ferramenta PODE\r\n" +
                "  funcionar com outras versões do NcStudio Phoenix.\r\n" +
                "• IMPORTANTE: Antes de testar em outras versões, faça um BACKUP\r\n" +
                "  COMPLETO da pasta de instalação do NcStudio!\r\n\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "                    AVISO DE RESPONSABILIDADE\r\n" +
                "═══════════════════════════════════════════════════════════════════\r\n" +
                "O uso desta ferramenta é de TOTAL RESPONSABILIDADE DO USUÁRIO.\r\n" +
                "Os desenvolvedores não se responsabilizam por quaisquer danos,\r\n" +
                "perdas de dados, mau funcionamento do NcStudio Phoenix ou\r\n" +
                "problemas decorrentes do uso deste software.\r\n" +
                "Sempre faça backup dos arquivos originais antes de qualquer\r\n" +
                "modificação. USE POR SUA CONTA E RISCO.\r\n";

            using var dialog = new Form
            {
                Text = "Tutorial - Editor de Traduções",
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                Font = Font,
                Width = 750,
                Height = 620
            };

            var txt = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Text = text,
                Font = new Font("Consolas", 9.5F)
            };

            var btnClose = new Button
            {
                Text = "Fechar",
                Dock = DockStyle.Bottom,
                Height = 38
            };
            btnClose.Click += (_, _) => dialog.Close();

            dialog.Controls.Add(txt);
            dialog.Controls.Add(btnClose);
            dialog.ShowDialog(this);
        }

        private void ShowAbout()
        {
            const string NcStudioVersion = "15.550.25";
            const string GitHubUrl = "https://github.com/dermicvas/NcStudio-Translate";
            const string GitHubUser = "dermicvas";
            const string License = "Licença de uso privativo (não comercial)";

            var appVersion = GetAppVersion();

            float scale = _zoomPercent / 100f;
            int baseWidth = 450;
            int baseHeight = 480;

            using var dialog = new Form
            {
                Text = $"Sobre - NcStudio Translate v{appVersion}",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size((int)(baseWidth * scale), (int)(baseHeight * scale)),
                BackColor = Color.White
            };

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding((int)(20 * scale))
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Icon + Title
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Info
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Button

            // Header com ícone e título
            var headerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                WrapContents = false
            };

            static Image? TryLoadImageCopy(string path)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        return null;
                    }

                    using var original = Image.FromFile(path);
                    return new Bitmap(original);
                }
                catch
                {
                    return null;
                }
            }

            // Logo (preferencial) com contorno discreto para fundo branco
            var logoPath = Path.Combine(AppContext.BaseDirectory, "logo.png");
            var logoImage = TryLoadImageCopy(logoPath);
            if (logoImage != null)
            {
                var logoWidth = (int)(240 * scale);
                var logoHeight = (int)(80 * scale);

                var borderPanel = new Panel
                {
                    BackColor = Color.Gainsboro,
                    Padding = new Padding(1),
                    Size = new Size(logoWidth + 2, logoHeight + 2),
                    Margin = new Padding(0, 0, 0, (int)(10 * scale))
                };

                var pictureLogo = new PictureBox
                {
                    Image = logoImage,
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                borderPanel.Controls.Add(pictureLogo);
                headerPanel.Controls.Add(borderPanel);
            }
            else
            {
                // Fallback: ícone atual
                var iconPath = Path.Combine(AppContext.BaseDirectory, "ico.ico");
                if (File.Exists(iconPath))
                {
                    try
                    {
                        var icon = new Icon(iconPath);
                        var pictureBox = new PictureBox
                        {
                            Image = icon.ToBitmap(),
                            Size = new Size((int)(64 * scale), (int)(64 * scale)),
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Margin = new Padding(0, 0, 0, (int)(10 * scale)),
                            Anchor = AnchorStyles.None
                        };
                        headerPanel.Controls.Add(pictureBox);
                    }
                    catch { }
                }
            }

            var lblTitle = new Label
            {
                Text = "NcStudio Translate",
                Font = new Font("Segoe UI", 16 * scale, FontStyle.Bold),
                AutoSize = true,
                ForeColor = Color.FromArgb(50, 50, 50),
                Margin = new Padding(0, 0, 0, (int)(5 * scale))
            };
            headerPanel.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Editor de traduções para NcStudio Phoenix",
                Font = new Font("Segoe UI", 9 * scale, FontStyle.Regular),
                AutoSize = true,
                ForeColor = Color.Gray,
                Margin = new Padding(0, 0, 0, (int)(15 * scale))
            };
            headerPanel.Controls.Add(lblSubtitle);

            mainPanel.Controls.Add(headerPanel, 0, 0);

            // Informações
            var infoPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                AutoSize = false
            };
            infoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            infoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Garante que cada linha cresça conforme o conteúdo (evita texto/links “cortados”)
            infoPanel.RowStyles.Clear();
            for (int i = 0; i < infoPanel.RowCount; i++)
            {
                infoPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            void AddInfoRow(int row, string label, string value, bool isLink = false)
            {
                var lblLabel = new Label
                {
                    Text = label,
                    Font = new Font("Segoe UI", 9 * scale, FontStyle.Bold),
                    AutoSize = true,
                    Margin = new Padding(0, (int)(5 * scale), (int)(10 * scale), (int)(5 * scale))
                };
                infoPanel.Controls.Add(lblLabel, 0, row);

                if (isLink)
                {
                    string displayText = value;
                    if (Uri.TryCreate(value, UriKind.Absolute, out var uri)
                        && (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                            || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
                    {
                        displayText = uri.Host + uri.AbsolutePath;
                        if (displayText.EndsWith("/", StringComparison.Ordinal))
                        {
                            displayText = displayText[..^1];
                        }
                    }

                    var linkFont = new Font("Segoe UI", 9 * scale, FontStyle.Underline);
                    var linkLabel = new Label
                    {
                        Text = displayText,
                        Font = linkFont,
                        AutoSize = true,
                        ForeColor = SystemColors.HotTrack,
                        Cursor = Cursors.Hand,
                        Margin = new Padding(0, (int)(5 * scale), 0, (int)(5 * scale)),
                        Padding = new Padding(0, 0, 0, Math.Max(2, (int)Math.Ceiling(2 * scale))),
                        UseCompatibleTextRendering = true
                    };

                    linkLabel.Click += (_, _) =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = value,
                                UseShellExecute = true
                            });
                        }
                        catch { }
                    };

                    infoPanel.Controls.Add(linkLabel, 1, row);
                }
                else
                {
                    var lblValue = new Label
                    {
                        Text = value,
                        Font = new Font("Segoe UI", 9 * scale),
                        AutoSize = true,
                        Margin = new Padding(0, (int)(5 * scale), 0, (int)(5 * scale))
                    };
                    infoPanel.Controls.Add(lblValue, 1, row);
                }
            }

            AddInfoRow(0, "Versão:", appVersion);
            AddInfoRow(1, "NcStudio compatível:", NcStudioVersion);
            AddInfoRow(2, "Licença:", License);
            AddInfoRow(3, "Desenvolvedor:", GitHubUser);
            AddInfoRow(4, "GitHub:", GitHubUrl, isLink: true);

            // Aviso de responsabilidade
            var lblDisclaimer = new Label
            {
                Text = "⚠️ O uso desta ferramenta é de total responsabilidade do usuário.",
                Font = new Font("Segoe UI", 8 * scale, FontStyle.Italic),
                ForeColor = Color.DarkRed,
                AutoSize = true,
                Margin = new Padding(0, (int)(15 * scale), 0, 0)
            };
            infoPanel.Controls.Add(lblDisclaimer, 0, 5);
            infoPanel.SetColumnSpan(lblDisclaimer, 2);

            mainPanel.Controls.Add(infoPanel, 0, 1);

            // Botão fechar
            var btnClose = new Button
            {
                Text = "Fechar",
                Width = (int)(100 * scale),
                Height = (int)(35 * scale),
                Anchor = AnchorStyles.None,
                FlatStyle = FlatStyle.System
            };
            btnClose.Click += (_, _) => dialog.Close();

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            buttonPanel.Controls.Add(btnClose);

            mainPanel.Controls.Add(buttonPanel, 0, 2);

            dialog.Controls.Add(mainPanel);
            dialog.AcceptButton = btnClose;
            dialog.ShowDialog(this);
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtFolderPath.Text = folderBrowserDialog.SelectedPath;
            _settings.LastFolder = folderBrowserDialog.SelectedPath;
            SaveSettings();
            _currentFilter = string.Empty;
            txtSearch.Text = string.Empty;
            _ = LoadResxFilesAsync(folderBrowserDialog.SelectedPath);
        }

        private void LoadResxFiles(string folder)
        {
            _ = LoadResxFilesAsync(folder);
        }

        private async Task LoadResxFilesAsync(string folder)
        {
            _logFilePath = string.IsNullOrWhiteSpace(folder) ? string.Empty : Path.Combine(folder, LogFileName);
            await LoadLogForFolderAsync();

            _resxFiles.Clear();
            if (!Directory.Exists(folder))
            {
                ClearEntries();
                return;
            }

            var entries = new Dictionary<string, ResxFileItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var zhPath in Directory.GetFiles(folder, "*.zh-CN.resx", SearchOption.TopDirectoryOnly))
            {
                var baseName = GetBaseName(zhPath, ".zh-CN.resx");
                if (!entries.TryGetValue(baseName, out var item))
                {
                    item = new ResxFileItem(baseName);
                    entries[baseName] = item;
                }

                item.ZhPath = zhPath;
            }

            foreach (var enPath in Directory.GetFiles(folder, "*.en-US.resx", SearchOption.TopDirectoryOnly))
            {
                var baseName = GetBaseName(enPath, ".en-US.resx");
                if (!entries.TryGetValue(baseName, out var item))
                {
                    item = new ResxFileItem(baseName);
                    entries[baseName] = item;
                }

                item.EnPath = enPath;
            }

            foreach (var item in entries.Values
                         .Where(item => !string.IsNullOrWhiteSpace(item.ZhPath)
                                       && !string.IsNullOrWhiteSpace(item.EnPath)
                                       && File.Exists(item.ZhPath!)
                                       && File.Exists(item.EnPath!))
                         .OrderBy(item => item.BaseName, StringComparer.OrdinalIgnoreCase))
            {
                _resxFiles.Add(item);
            }

            if (_resxFiles.Count > 0)
            {
                lstResxFiles.SelectedIndex = 0;
            }
            else
            {
                ClearEntries();
            }

            UpdateStatus($"{_resxFiles.Count} arquivo(s) encontrados");
        }

        private void lstResxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            
            if (lstResxFiles.SelectedItem is ResxFileItem item)
            {
                _ = LoadResxFileAsync(item);
            }
        }

        private void LoadResxFile(ResxFileItem item)
        {
            _ = LoadResxFileAsync(item);
        }

        private async Task LoadResxFileAsync(ResxFileItem item)
        {
            if (string.IsNullOrWhiteSpace(item.ZhPath) || !File.Exists(item.ZhPath))
            {
                ClearEntries();
                return;
            }

            if (string.IsNullOrWhiteSpace(item.EnPath) || !File.Exists(item.EnPath))
            {
                ClearEntries();
                UpdateStatus($"Arquivo en-US não encontrado para {item.BaseName}");
                return;
            }

            _currentItem = item;
            _selectedZhPath = item.ZhPath!;
            _referenceEnPath = item.EnPath ?? string.Empty;
            _backupZhFilePath = $"{_selectedZhPath}.original";

            // Mostrar loading ANTES de qualquer operação pesada
            ShowLoading($"Carregando {item.BaseName}...");
            
            // Dá tempo para a UI renderizar o painel de loading
            await Task.Delay(100).ConfigureAwait(true);

            try
            {
                var ct = _loadingCts?.Token ?? CancellationToken.None;
                
                // Copia caminhos para uso local na thread
                var zhPath = _selectedZhPath;
                var enPath = _referenceEnPath;
                var backupPath = _backupZhFilePath;

                // Faz todas as operações pesadas em background
                var (document, entries, originalValues) = await Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    
                    // NÃO cria backup automaticamente - aguarda usuário clicar em "Criar tradução"
                    
                    ct.ThrowIfCancellationRequested();
                    
                    // LoadOriginalValues - em background (sempre carrega referência inglesa)
                    var origValues = new Dictionary<string, string>(StringComparer.Ordinal);
                    if (!string.IsNullOrWhiteSpace(enPath) && File.Exists(enPath))
                    {
                        var enDoc = XDocument.Load(enPath);
                        foreach (var data in enDoc.Root?.Elements("data") ?? Enumerable.Empty<XElement>())
                        {
                            ct.ThrowIfCancellationRequested();
                            var key = data.Attribute("name")?.Value;
                            if (!string.IsNullOrWhiteSpace(key))
                            {
                                origValues[key] = data.Element("value")?.Value ?? string.Empty;
                            }
                        }
                    }
                    
                    ct.ThrowIfCancellationRequested();
                    
                    // Carrega o documento principal (chinês atual)
                    var doc = XDocument.Load(zhPath);
                    
                    ct.ThrowIfCancellationRequested();

                    var dataElements = doc.Descendants("data").ToList();
                    var result = new List<ResourceEntry>();
                    
                    foreach (var data in dataElements)
                    {
                        ct.ThrowIfCancellationRequested();
                        
                        var key = data.Attribute("name")?.Value;
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            continue;
                        }

                        var currentValue = data.Element("value")?.Value ?? string.Empty;
                        origValues.TryGetValue(key, out var originalValue);

                        result.Add(new ResourceEntry
                        {
                            Key = key,
                            CurrentValue = currentValue,
                            OriginalValue = originalValue ?? string.Empty
                        });
                    }
                    
                    return (doc, result.OrderBy(e => e.Key, StringComparer.Ordinal).ToList(), origValues);
                }, ct).ConfigureAwait(true);

                // Atualiza estado na UI thread
                _currentDocument = document;
                _allEntries = entries;
                _originalValues.Clear();
                foreach (var kv in originalValues)
                {
                    _originalValues[kv.Key] = kv.Value;
                }
                
                // Verifica se já existe tradução iniciada
                var translationStarted = File.Exists(_backupZhFilePath);
                
                ApplyFilter();
                ForceRebindEntriesGrid();
                
                // Controla modo de edição do grid
                SetGridEditMode(translationStarted);
                
                // Criar tradução: ativo quando NÃO existe .original
                // Excluir tradução: ativo quando existe .original
                btnCreateTranslation.Enabled = !translationStarted;
                btnDeleteTranslation.Enabled = translationStarted;
                
                if (_allEntries.Count == 0)
                {
                    UpdateStatus($"Arquivo carregado, mas nenhuma entrada <data> foi encontrada: {Path.GetFileName(_selectedZhPath)}");
                }
                else if (translationStarted)
                {
                    UpdateStatus($"Arquivo carregado (editável): {Path.GetFileName(_selectedZhPath)} | Entradas: {_visibleEntries.Count}/{_allEntries.Count}");
                }
                else
                {
                    UpdateStatus($"Arquivo carregado (somente leitura - clique em 'Criar tradução' para editar): {Path.GetFileName(_selectedZhPath)} | Entradas: {_visibleEntries.Count}/{_allEntries.Count}");
                }
            }
            catch (OperationCanceledException)
            {
                ClearEntries();
                UpdateStatus("Carregamento cancelado");
            }
            catch (Exception ex)
            {
                ClearEntries();
                UpdateStatus($"Erro ao carregar arquivo: {ex.Message}");
            }
            finally
            {
                HideLoading();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _currentFilter = txtSearch.Text;
            ApplyFilter();
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            // O TextChanged já vai chamar ApplyFilter automaticamente
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            if (_logEntries.Count == 0)
            {
                ZoomMessageBox.ShowInfo("O log já está vazio.", "Limpar log");
                return;
            }

            var result = ZoomMessageBox.ShowDeleteConfirmation(
                $"Deseja limpar todas as {_logEntries.Count} entrada(s) do log?\nEsta ação não pode ser desfeita.",
                "Limpar log");

            if (result != DialogResult.Yes)
            {
                return;
            }

            _logEntries.Clear();

            // Remove o arquivo de log físico
            if (!string.IsNullOrWhiteSpace(_logFilePath) && File.Exists(_logFilePath))
            {
                try
                {
                    File.Delete(_logFilePath);
                }
                catch
                {
                    // ignorar falha ao deletar
                }
            }

            RefreshLogGrid();
            UpdateStatus("Log limpo");
        }

        private void btnCreateTranslation_Click(object sender, EventArgs e)
        {
            if (_currentItem == null || string.IsNullOrWhiteSpace(_selectedZhPath) || string.IsNullOrWhiteSpace(_referenceEnPath))
            {
                ZoomMessageBox.ShowInfo("Selecione um arquivo primeiro.", "Criar tradução");
                return;
            }

            // Verifica se já existe backup .original (tradução já iniciada)
            if (File.Exists(_backupZhFilePath))
            {
                ZoomMessageBox.ShowWarning(
                    "Já existe uma tradução em andamento para este arquivo.\n\n" +
                    "Para reiniciar, primeiro exclua a tradução atual usando o botão 'Excluir tradução'.",
                    "Criar tradução");
                return;
            }

            var result = ZoomMessageBox.ShowConfirmation(
                "Esta ação irá:\n\n" +
                "1. Criar backup do arquivo chinês atual (.original)\n" +
                "2. Substituir o chinês pelo conteúdo em inglês para tradução\n" +
                "3. Habilitar edição do arquivo\n\n" +
                "O backup original será protegido e não poderá ser sobrescrito.\n\n" +
                "Deseja continuar?",
                "Criar tradução");

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Faz backup do chinês original
                File.Copy(_selectedZhPath, _backupZhFilePath);
                // Substitui pelo inglês
                File.Copy(_referenceEnPath, _selectedZhPath, overwrite: true);

                // Registra no log a criação da tradução
                AppendLog(new LogEntry
                {
                    File = _currentItem?.BaseName ?? Path.GetFileNameWithoutExtension(_selectedZhPath),
                    Key = "-",
                    OriginalValue = "-",
                    InsertedValue = "-",
                    Origin = "Criação de tradução"
                });

                // Recarrega o arquivo (agora com edição habilitada)
                LoadResxFile(_currentItem);
                UpdateStatus("Tradução criada - backup salvo e arquivo preparado para edição");
            }
            catch (IOException ex)
            {
                ZoomMessageBox.ShowError($"Erro de acesso ao arquivo: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                ZoomMessageBox.ShowError($"Acesso negado ao arquivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                ZoomMessageBox.ShowError($"Erro ao criar tradução: {ex.Message}");
            }
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvEntries.CurrentCell == null)
            {
                return;
            }

            var cellValue = dgvEntries.CurrentCell.Value?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(cellValue))
            {
                Clipboard.SetText(cellValue);
            }
        }

        private void translateChatGPTMenuItem_Click(object sender, EventArgs e)
        {
            OpenAITranslation("https://chat.openai.com/?q=");
        }

        private void translatePerplexityMenuItem_Click(object sender, EventArgs e)
        {
            OpenAITranslation("https://www.perplexity.ai/?q=");
        }

        private void translateGoogleAIMenuItem_Click(object sender, EventArgs e)
        {
            OpenAITranslation("https://www.google.com/search?udm=50&q=");
        }

        private void OpenAITranslation(string baseUrl)
        {
            if (dgvEntries.CurrentRow == null)
            {
                return;
            }

            if (dgvEntries.CurrentRow.DataBoundItem is not ResourceEntry entry)
            {
                return;
            }

            var referenceText = entry.OriginalValue;
            if (string.IsNullOrWhiteSpace(referenceText))
            {
                ZoomMessageBox.ShowInfo("Não há texto de referência para traduzir.", "Traduzir");
                return;
            }

            var prompt = $"Traduza para português brasileiro o seguinte texto de interface do software NcStudio Phoenix: \"{referenceText}\". Não é necessário adicionar observações e explicações. Entregue numa caixa de código pronto para copiar e colar.";
            var encodedPrompt = Uri.EscapeDataString(prompt);
            var url = baseUrl + encodedPrompt;

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ZoomMessageBox.ShowError($"Erro ao abrir navegador: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            var normalizedFilter = _currentFilter?.Trim() ?? string.Empty;

            // Cria uma nova lista filtrada
            var filtered = new List<ResourceEntry>();
            foreach (var entry in _allEntries)
            {
                if (MatchesFilter(entry, normalizedFilter))
                {
                    filtered.Add(entry);
                }
            }

            _visibleEntries.Clear();
            foreach (var entry in filtered)
            {
                _visibleEntries.Add(entry);
            }

            // Suspende o layout e redesenho para melhorar performance
            dgvEntries.SuspendLayout();
            dgvEntries.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            
            try
            {
                // Binding direto na lista - AutoGenerateColumns = true vai criar as colunas
                dgvEntries.DataSource = null;
                dgvEntries.DataSource = filtered;

                // Configura as colunas após o binding
                ConfigureEntriesGridColumns();
            }
            finally
            {
                // Usa DisplayedCells para calcular apenas as linhas visíveis (muito mais rápido)
                dgvEntries.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
                dgvEntries.ResumeLayout();
            }

            lblEntryCount.Text = $"Entradas: {filtered.Count} / {_allEntries.Count}";
        }

        private void ConfigureEntriesGridColumns()
        {
            if (dgvEntries.Columns.Count == 0) return;

            // Coluna Key
            if (dgvEntries.Columns.Contains("Key"))
            {
                var colKey = dgvEntries.Columns["Key"];
                colKey.HeaderText = "Chave";
                colKey.ReadOnly = true;
                colKey.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                colKey.Visible = _settings.ShowKeyColumn;
                colKey.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna CurrentValue
            if (dgvEntries.Columns.Contains("CurrentValue"))
            {
                var colCurrent = dgvEntries.Columns["CurrentValue"];
                colCurrent.HeaderText = "Atual (substitui .zh-CN)";
                colCurrent.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colCurrent.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                colCurrent.Resizable = DataGridViewTriState.True;
                colCurrent.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna OriginalValue
            if (dgvEntries.Columns.Contains("OriginalValue"))
            {
                var colOriginal = dgvEntries.Columns["OriginalValue"];
                colOriginal.HeaderText = "Referência (.en-US)";
                colOriginal.ReadOnly = true;
                colOriginal.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                colOriginal.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                colOriginal.Resizable = DataGridViewTriState.True;
                colOriginal.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void SetGridEditMode(bool enabled)
        {
            // Define se a coluna CurrentValue permite edição
            if (dgvEntries.Columns.Contains("CurrentValue"))
            {
                var colCurrent = dgvEntries.Columns["CurrentValue"];
                colCurrent.ReadOnly = !enabled;
                
                // Muda a cor de fundo para indicar modo somente leitura
                if (enabled)
                {
                    colCurrent.DefaultCellStyle.BackColor = Color.White;
                    colCurrent.HeaderText = "Atual (substitui .zh-CN)";
                }
                else
                {
                    colCurrent.DefaultCellStyle.BackColor = Color.LightGray;
                    colCurrent.HeaderText = "Atual (.zh-CN) - Somente leitura";
                }
            }
        }

        private void ConfigureLogGridColumns()
        {
            if (dgvLog.Columns.Count == 0) return;

            // Coluna File
            if (dgvLog.Columns.Contains("File"))
            {
                var col = dgvLog.Columns["File"];
                col.HeaderText = "Arquivo";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DisplayIndex = 0;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna Key
            if (dgvLog.Columns.Contains("Key"))
            {
                var col = dgvLog.Columns["Key"];
                col.HeaderText = "Chave";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DisplayIndex = 1;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna Timestamp
            if (dgvLog.Columns.Contains("Timestamp"))
            {
                var col = dgvLog.Columns["Timestamp"];
                col.HeaderText = "Data/Hora";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
                col.DisplayIndex = 2;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna OriginalValue
            if (dgvLog.Columns.Contains("OriginalValue"))
            {
                var col = dgvLog.Columns["OriginalValue"];
                col.HeaderText = "Valor anterior";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                col.Resizable = DataGridViewTriState.True;
                col.DisplayIndex = 3;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna InsertedValue
            if (dgvLog.Columns.Contains("InsertedValue"))
            {
                var col = dgvLog.Columns["InsertedValue"];
                col.HeaderText = "Valor inserido";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                col.Resizable = DataGridViewTriState.True;
                col.DisplayIndex = 4;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // Coluna Origin
            if (dgvLog.Columns.Contains("Origin"))
            {
                var col = dgvLog.Columns["Origin"];
                col.HeaderText = "Origem";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.DisplayIndex = 5;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void ForceRebindEntriesGrid()
        {
            // Já feito no ApplyFilter - método mantido por compatibilidade
        }

        private static bool MatchesFilter(ResourceEntry entry, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            return MatchesWildcard(entry.Key, filter)
                   || MatchesWildcard(entry.CurrentValue, filter)
                   || MatchesWildcard(entry.OriginalValue, filter);
        }

        /// <summary>
        /// Verifica se o texto contém todos os tokens do filtro na ordem especificada.
        /// O curinga % divide o filtro em partes que devem aparecer em sequência.
        /// Exemplo: "deseja%que%software" encontra "Você deseja que o software faça isso"
        /// </summary>
        private static bool MatchesWildcard(string? source, string filter)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            // Se não tem curinga %, faz busca simples
            if (!filter.Contains('%'))
            {
                return source.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            // Divide pelos curingas e busca cada parte em sequência
            var parts = filter.Split('%', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return true;
            }

            int currentIndex = 0;
            foreach (var part in parts)
            {
                int foundAt = source.IndexOf(part, currentIndex, StringComparison.OrdinalIgnoreCase);
                if (foundAt < 0)
                {
                    return false;
                }
                currentIndex = foundAt + part.Length;
            }

            return true;
        }

        private void ClearEntries()
        {
            _selectedZhPath = string.Empty;
            _referenceEnPath = string.Empty;
            _backupZhFilePath = string.Empty;
            _currentItem = null;
            _currentDocument = null;
            _allEntries = new List<ResourceEntry>();
            _visibleEntries.Clear();
            _originalValues.Clear();
            entriesBindingSource.ResetBindings(false);
            lblEntryCount.Text = "Entradas: 0 / 0";
            btnCreateTranslation.Enabled = false;
            btnDeleteTranslation.Enabled = false;
        }

        private void LoadSettings()
        {
            _settings = new AppSettings();
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    return;
                }

                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                if (loaded != null)
                {
                    _settings = loaded;
                }
            }
            catch
            {
                _settings = new AppSettings();
            }
        }

        private void SaveSettings()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                var tmp = _settingsPath + ".tmp";
                File.WriteAllText(tmp, json);
                File.Copy(tmp, _settingsPath, overwrite: true);
                File.Delete(tmp);
            }
            catch
            {
                // ignorar falhas de persistência
            }
        }

        private void LoadLogForFolder()
        {
            _ = LoadLogForFolderAsync();
        }

        private async Task LoadLogForFolderAsync()
        {
            _logEntries.Clear();
            btnRestoreFromLog.Enabled = false;

            if (string.IsNullOrWhiteSpace(_logFilePath) || !File.Exists(_logFilePath))
            {
                RefreshLogGrid();
                return;
            }

            // Verifica o tamanho do arquivo para decidir se mostra loading
            var fileInfo = new FileInfo(_logFilePath);
            var showProgress = fileInfo.Length > 50_000; // Mais de 50KB mostra loading
            
            if (showProgress)
            {
                ShowLoading("Carregando log...");
                
                // Dá tempo para a UI renderizar
                await Task.Delay(100).ConfigureAwait(true);
            }

            try
            {
                var ct = _loadingCts?.Token ?? CancellationToken.None;
                var logPath = _logFilePath;
                
                var entries = await Task.Run(() =>
                {
                    var result = new List<LogEntry>();
                    var lines = File.ReadAllLines(logPath);
                    
                    for (int i = 0; i < lines.Length; i++)
                    {
                        ct.ThrowIfCancellationRequested();
                        
                        var line = lines[i];
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        try
                        {
                            var item = JsonSerializer.Deserialize<LogEntry>(line);
                            if (item != null)
                            {
                                result.Add(item);
                            }
                        }
                        catch
                        {
                            // ignorar linhas inválidas
                        }
                    }
                    
                    return result;
                }, ct).ConfigureAwait(true);

                foreach (var entry in entries)
                {
                    _logEntries.Add(entry);
                }
            }
            catch (OperationCanceledException)
            {
                _logEntries.Clear();
                UpdateStatus("Carregamento do log cancelado");
            }
            catch
            {
                // ignorar falhas de leitura/parse
            }
            finally
            {
                if (showProgress)
                {
                    HideLoading();
                }
            }

            RefreshLogGrid();
        }

        private void RefreshLogGrid()
        {
            // Ordena por Arquivo, depois Chave, depois Data/Hora
            var sorted = _logEntries
                .OrderBy(e => e.File, StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.Key, StringComparer.OrdinalIgnoreCase)
                .ThenBy(e => e.Timestamp)
                .ToList();

            dgvLog.DataSource = null;
            dgvLog.DataSource = sorted;
            ConfigureLogGridColumns();
        }

        private void AppendLog(LogEntry entry)
        {
            if (entry == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entry.File))
            {
                entry.File = _currentItem?.BaseName ?? string.Empty;
            }

            entry.Timestamp = DateTimeOffset.Now;

            _logEntries.Add(entry);
            RefreshLogGrid();

            if (string.IsNullOrWhiteSpace(_logFilePath))
            {
                return;
            }

            try
            {
                var serialized = JsonSerializer.Serialize(entry);
                File.AppendAllText(_logFilePath, serialized + Environment.NewLine);
            }
            catch
            {
                // ignorar falhas de persistência do log
            }
        }

        private void dgvLog_SelectionChanged(object sender, EventArgs e)
        {
            btnRestoreFromLog.Enabled = dgvLog.SelectedRows.Count == 1;
        }

        private void btnRestoreFromLog_Click(object sender, EventArgs e)
        {
            if (dgvLog.SelectedRows.Count != 1)
            {
                return;
            }

            if (dgvLog.SelectedRows[0].DataBoundItem is not LogEntry logEntry)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(logEntry.File) || string.IsNullOrWhiteSpace(logEntry.Key))
            {
                return;
            }

            var targetItem = _resxFiles.FirstOrDefault(x => string.Equals(x.BaseName, logEntry.File, StringComparison.OrdinalIgnoreCase));
            if (targetItem == null)
            {
                ZoomMessageBox.ShowWarning($"Arquivo '{logEntry.File}' não foi encontrado na lista desta pasta.", "Restaurar do log");
                return;
            }

            if (!ReferenceEquals(_currentItem, targetItem))
            {
                lstResxFiles.SelectedItem = targetItem;
            }

            if (_currentDocument == null || string.IsNullOrWhiteSpace(_selectedZhPath))
            {
                return;
            }

            // Verifica se há tradução em andamento
            if (!File.Exists(_backupZhFilePath))
            {
                ZoomMessageBox.ShowWarning("Não há tradução em andamento. Crie uma tradução primeiro.", "Restaurar do log");
                return;
            }
            
            var dataElement = FindDataElement(logEntry.Key);
            if (dataElement == null)
            {
                ZoomMessageBox.ShowWarning($"Chave '{logEntry.Key}' não encontrada no arquivo.", "Restaurar do log");
                return;
            }

            var valueElement = dataElement.Element("value");
            if (valueElement == null)
            {
                valueElement = new XElement("value");
                dataElement.Add(valueElement);
            }

            var oldValue = valueElement.Value;
            var restoredValue = logEntry.OriginalValue ?? string.Empty;

            if (oldValue == restoredValue)
            {
                UpdateStatus($"Nada a restaurar para {logEntry.Key}");
                return;
            }

            var confirm = ZoomMessageBox.ShowConfirmation(
                $"Restaurar a chave '{logEntry.Key}' para o valor anterior registrado no log?",
                "Restaurar do log");

            if (confirm != DialogResult.Yes)
            {
                return;
            }

            valueElement.Value = restoredValue;
            _currentDocument.Save(_selectedZhPath);

            var entry = _allEntries.FirstOrDefault(x => string.Equals(x.Key, logEntry.Key, StringComparison.Ordinal));
            if (entry != null)
            {
                entry.CurrentValue = restoredValue;
                entriesBindingSource.ResetBindings(false);
            }

            AppendLog(new LogEntry
            {
                File = logEntry.File,
                Key = logEntry.Key,
                OriginalValue = oldValue,
                InsertedValue = restoredValue,
                Origin = "restauração de log"
            });

            UpdateStatus($"Restaurado {logEntry.Key} via log");
        }

        private void dgvEntries_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            // Só processa se a edição foi na coluna CurrentValue
            var columnName = dgvEntries.Columns[e.ColumnIndex].Name;
            if (columnName != "CurrentValue")
            {
                return;
            }

            var row = dgvEntries.Rows[e.RowIndex];
            if (row.DataBoundItem is not ResourceEntry entry)
            {
                return;
            }

            var newValue = entry.CurrentValue ?? string.Empty;

            // Captura o valor antigo do documento atual
            var dataElementBefore = FindDataElement(entry.Key);
            var oldValue = dataElementBefore?.Element("value")?.Value ?? string.Empty;

            // Se os valores são iguais, não há nada a fazer
            if (oldValue == newValue)
            {
                return;
            }

            // Verifica se há tradução em andamento
            if (!File.Exists(_backupZhFilePath))
            {
                ZoomMessageBox.ShowWarning("Não há tradução em andamento. Crie uma tradução primeiro.");
                // Reverter o valor no grid
                entry.CurrentValue = oldValue;
                return;
            }
            
            // Busca o elemento no documento
            var dataElement = FindDataElement(entry.Key);
            if (dataElement == null || _currentDocument == null)
            {
                return;
            }

            var valueElement = dataElement.Element("value");
            if (valueElement == null)
            {
                valueElement = new XElement("value");
                dataElement.Add(valueElement);
            }

            valueElement.Value = newValue;
            _currentDocument.Save(_selectedZhPath);
            
            AppendLog(new LogEntry
            {
                File = _currentItem?.BaseName ?? string.Empty,
                Key = entry.Key,
                OriginalValue = oldValue,
                InsertedValue = newValue,
                Origin = "Inserção manual"
            });
            UpdateStatus($"Salvo {entry.Key}");
        }

        private XElement? FindDataElement(string key)
        {
            return _currentDocument?.Root?
                .Elements("data")
                .FirstOrDefault(data => string.Equals(data.Attribute("name")?.Value, key, StringComparison.Ordinal));
        }

        private void LoadOriginalValues()
        {
            _originalValues.Clear();
            // Referência: inglês (sempre intacto)
            if (string.IsNullOrWhiteSpace(_referenceEnPath) || !File.Exists(_referenceEnPath))
            {
                return;
            }

            var document = XDocument.Load(_referenceEnPath);
            foreach (var data in document.Root?.Elements("data") ?? Enumerable.Empty<XElement>())
            {
                var key = data.Attribute("name")?.Value;
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                _originalValues[key] = data.Element("value")?.Value ?? string.Empty;
            }
        }

        private void RefreshOriginalValues()
        {
            foreach (var entry in _allEntries)
            {
                entry.OriginalValue = _originalValues.TryGetValue(entry.Key, out var originalValue)
                    ? originalValue
                    : string.Empty;
            }

            // Adia o ApplyFilter para evitar chamada reentrante durante CellEndEdit
            // Mas só usa BeginInvoke se o handle já existir
            if (IsHandleCreated)
            {
                BeginInvoke(new Action(ApplyFilter));
            }
            else
            {
                ApplyFilter();
            }
        }

        private static string GetBaseName(string filePath, string suffix)
        {
            var fileName = Path.GetFileName(filePath) ?? string.Empty;
            if (fileName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return fileName[..^suffix.Length];
            }

            return Path.GetFileNameWithoutExtension(fileName);
        }

        private void btnDeleteTranslation_Click(object sender, EventArgs e)
        {
            if (_currentItem == null || string.IsNullOrWhiteSpace(_selectedZhPath))
            {
                ZoomMessageBox.ShowInfo("Selecione um arquivo primeiro.", "Excluir tradução");
                return;
            }
            
            if (!File.Exists(_backupZhFilePath))
            {
                ZoomMessageBox.ShowInfo("Não existe tradução em andamento para este arquivo.", "Excluir tradução");
                return;
            }

            var result = ZoomMessageBox.ShowDeleteConfirmation(
                "Esta ação irá EXCLUIR PERMANENTEMENTE a tradução em andamento:\n\n" +
                "• O arquivo de tradução atual será excluído\n" +
                "• O arquivo original chinês será restaurado\n" +
                "• O arquivo de backup (.original) será excluído\n\n" +
                "Esta ação NÃO pode ser desfeita!\n\n" +
                "Deseja continuar?",
                "Excluir tradução");

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Exclui o arquivo atual (tradução em andamento)
                if (File.Exists(_selectedZhPath))
                {
                    File.Delete(_selectedZhPath);
                }
                
                // Restaura o original
                File.Copy(_backupZhFilePath, _selectedZhPath);
                
                // Exclui o backup
                File.Delete(_backupZhFilePath);

                // Registra no log a exclusão da tradução
                AppendLog(new LogEntry
                {
                    File = _currentItem?.BaseName ?? Path.GetFileNameWithoutExtension(_selectedZhPath),
                    Key = "-",
                    OriginalValue = "-",
                    InsertedValue = "-",
                    Origin = "Exclusão de tradução"
                });

                // Recarrega o arquivo (agora em modo somente leitura)
                LoadResxFile(_currentItem);
                UpdateStatus("Tradução excluída - arquivo original restaurado");
            }
            catch (IOException ex)
            {
                ZoomMessageBox.ShowError($"Erro de acesso ao arquivo: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                ZoomMessageBox.ShowError($"Acesso negado ao arquivo: {ex.Message}");
            }
            catch (Exception ex)
            {
                ZoomMessageBox.ShowError($"Erro ao excluir tradução: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }
    }
}