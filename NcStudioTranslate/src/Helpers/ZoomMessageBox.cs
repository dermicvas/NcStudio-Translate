using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NcStudioTranslate.Helpers
{
    /// <summary>
    /// Classe para exibir caixas de mensagem personalizadas que respeitam o zoom do aplicativo.
    /// </summary>
    public static class ZoomMessageBox
    {
        private static int _zoomPercent = 100;

        /// <summary>
        /// Define o zoom atual para ser aplicado nas mensagens.
        /// </summary>
        public static int ZoomPercent
        {
            get => _zoomPercent;
            set => _zoomPercent = Math.Max(50, Math.Min(200, value));
        }

        private static string GetImagePath(string imageName)
        {
            return Path.Combine(AppContext.BaseDirectory, imageName);
        }

        /// <summary>
        /// Exibe uma mensagem de erro.
        /// </summary>
        public static DialogResult ShowError(string message, string title = "Erro")
        {
            return ShowMessage(message, title, "error.png", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Exibe uma mensagem de alerta/aviso.
        /// </summary>
        public static DialogResult ShowWarning(string message, string title = "Aviso")
        {
            return ShowMessage(message, title, "alert.png", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Exibe uma mensagem de confirmação com Sim/Não.
        /// </summary>
        public static DialogResult ShowConfirmation(string message, string title = "Confirmação")
        {
            return ShowMessage(message, title, "confirmation.png", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Exibe uma mensagem de exclusão com Sim/Não.
        /// </summary>
        public static DialogResult ShowDeleteConfirmation(string message, string title = "Confirmar Exclusão")
        {
            return ShowMessage(message, title, "exclude.png", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Exibe uma mensagem de informação.
        /// </summary>
        public static DialogResult ShowInfo(string message, string title = "Informação")
        {
            return ShowMessage(message, title, "confirmation.png", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Exibe uma mensagem personalizada.
        /// </summary>
        public static DialogResult ShowMessage(
            string message,
            string title,
            string? imageName,
            MessageBoxButtons buttons,
            MessageBoxIcon fallbackIcon)
        {
            using var form = new Form();
            form.Text = title;
            form.StartPosition = FormStartPosition.CenterParent;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.ShowInTaskbar = false;
            form.AutoScaleMode = AutoScaleMode.Font;

            // Calcula tamanhos baseados no zoom
            float scale = _zoomPercent / 100f;
            int baseFontSize = 9;
            int baseIconSize = 48;
            int basePadding = 20;
            int baseButtonWidth = 90;
            int baseButtonHeight = 30;
            int baseMinWidth = 350;
            int baseMaxWidth = 600;

            int iconSize = (int)(baseIconSize * scale);
            int padding = (int)(basePadding * scale);
            int buttonWidth = (int)(baseButtonWidth * scale);
            int buttonHeight = (int)(baseButtonHeight * scale);
            int minWidth = (int)(baseMinWidth * scale);
            int maxWidth = (int)(baseMaxWidth * scale);

            var font = new Font("Segoe UI", baseFontSize * scale, FontStyle.Regular);
            form.Font = font;

            // Painel principal
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(padding),
                AutoSize = true
            };
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Ícone
            PictureBox? pictureBox = null;
            string? imagePath = imageName != null ? GetImagePath(imageName) : null;

            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    var originalImage = Image.FromFile(imagePath);
                    var resizedImage = new Bitmap(iconSize, iconSize);
                    using (var g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(originalImage, 0, 0, iconSize, iconSize);
                    }

                    pictureBox = new PictureBox
                    {
                        Image = resizedImage,
                        Size = new Size(iconSize, iconSize),
                        Margin = new Padding(0, 0, padding, 0),
                        SizeMode = PictureBoxSizeMode.Zoom
                    };
                    mainPanel.Controls.Add(pictureBox, 0, 0);
                }
                catch
                {
                    // Se falhar ao carregar a imagem, continua sem ela
                }
            }

            // Mensagem
            var lblMessage = new Label
            {
                Text = message,
                Font = font,
                AutoSize = true,
                MaximumSize = new Size(maxWidth - iconSize - padding * 3, 0),
                Margin = new Padding(0),
                Padding = new Padding(0, padding / 2, 0, padding)
            };

            if (pictureBox != null)
            {
                mainPanel.Controls.Add(lblMessage, 1, 0);
            }
            else
            {
                mainPanel.SetColumnSpan(lblMessage, 2);
                mainPanel.Controls.Add(lblMessage, 0, 0);
            }

            // Painel de botões
            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, padding / 2, 0, 0)
            };
            mainPanel.SetColumnSpan(buttonPanel, 2);
            mainPanel.Controls.Add(buttonPanel, 0, 1);

            // Cria botões baseados no tipo
            DialogResult result = DialogResult.None;

            Button CreateButton(string text, DialogResult dialogResult, bool isDefault = false)
            {
                var btn = new Button
                {
                    Text = text,
                    Size = new Size(buttonWidth, buttonHeight),
                    Font = font,
                    Margin = new Padding(padding / 2, 0, 0, 0),
                    DialogResult = dialogResult,
                    FlatStyle = FlatStyle.System
                };

                if (isDefault)
                {
                    form.AcceptButton = btn;
                }

                btn.Click += (_, _) =>
                {
                    result = dialogResult;
                    form.Close();
                };

                return btn;
            }

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    buttonPanel.Controls.Add(CreateButton("OK", DialogResult.OK, true));
                    break;

                case MessageBoxButtons.OKCancel:
                    buttonPanel.Controls.Add(CreateButton("Cancelar", DialogResult.Cancel));
                    buttonPanel.Controls.Add(CreateButton("OK", DialogResult.OK, true));
                    form.CancelButton = buttonPanel.Controls[0] as Button;
                    break;

                case MessageBoxButtons.YesNo:
                    buttonPanel.Controls.Add(CreateButton("Não", DialogResult.No));
                    buttonPanel.Controls.Add(CreateButton("Sim", DialogResult.Yes, true));
                    form.CancelButton = buttonPanel.Controls[0] as Button;
                    break;

                case MessageBoxButtons.YesNoCancel:
                    buttonPanel.Controls.Add(CreateButton("Cancelar", DialogResult.Cancel));
                    buttonPanel.Controls.Add(CreateButton("Não", DialogResult.No));
                    buttonPanel.Controls.Add(CreateButton("Sim", DialogResult.Yes, true));
                    form.CancelButton = buttonPanel.Controls[0] as Button;
                    break;

                case MessageBoxButtons.RetryCancel:
                    buttonPanel.Controls.Add(CreateButton("Cancelar", DialogResult.Cancel));
                    buttonPanel.Controls.Add(CreateButton("Repetir", DialogResult.Retry, true));
                    form.CancelButton = buttonPanel.Controls[0] as Button;
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    buttonPanel.Controls.Add(CreateButton("Ignorar", DialogResult.Ignore));
                    buttonPanel.Controls.Add(CreateButton("Repetir", DialogResult.Retry));
                    buttonPanel.Controls.Add(CreateButton("Anular", DialogResult.Abort, true));
                    break;
            }

            form.Controls.Add(mainPanel);

            // Calcula o tamanho do form
            form.ClientSize = new Size(10, 10);
            mainPanel.PerformLayout();

            int formWidth = Math.Max(minWidth, mainPanel.PreferredSize.Width + padding * 2);
            int formHeight = mainPanel.PreferredSize.Height + padding;

            form.ClientSize = new Size(formWidth, formHeight);

            // Cor de fundo
            form.BackColor = SystemColors.Window;
            mainPanel.BackColor = SystemColors.Window;

            form.ShowDialog();
            return result;
        }
    }
}
