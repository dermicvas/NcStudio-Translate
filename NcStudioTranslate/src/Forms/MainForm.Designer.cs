namespace NcStudioTranslate.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.TableLayoutPanel topPanel;

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opcoesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exibirColunaChaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirLogErrosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ajudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tutorialAjudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorAjuda;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageEditor;
        private System.Windows.Forms.TabPage tabPageLog;

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TableLayoutPanel leftPanel;
        private System.Windows.Forms.Label lblFilesTitle;
        private System.Windows.Forms.ListBox lstResxFiles;

        private System.Windows.Forms.TableLayoutPanel rightPanel;
        private System.Windows.Forms.TableLayoutPanel headerPanel;
        private System.Windows.Forms.FlowLayoutPanel leftHeaderFlow;
        private System.Windows.Forms.FlowLayoutPanel zoomFlow;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Button btnDeleteTranslation;
        private System.Windows.Forms.Label lblEntryCount;
        private System.Windows.Forms.Label lblZoomLabel;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomReset;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Label lblZoomLevel;
        private System.Windows.Forms.DataGridView dgvEntries;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Button btnRestoreFromLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label lblLogHint;

        private System.Windows.Forms.Button btnCreateTranslation;
        private System.Windows.Forms.ContextMenuStrip contextMenuTranslate;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripSeparator contextMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem translateChatGPTMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translatePerplexityMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translateGoogleAIMenuItem;

        private System.Windows.Forms.BindingSource entriesBindingSource;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;

        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblLoadingStatus;
        private System.Windows.Forms.Button btnCancelLoading;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnSelectFolder = new System.Windows.Forms.Button();
            txtFolderPath = new System.Windows.Forms.TextBox();
            topPanel = new System.Windows.Forms.TableLayoutPanel();
            menuStrip = new System.Windows.Forms.MenuStrip();
            arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            opcoesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exibirColunaChaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            abrirLogErrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ajudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tutorialAjudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparatorAjuda = new System.Windows.Forms.ToolStripSeparator();
            sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tabControlMain = new System.Windows.Forms.TabControl();
            tabPageEditor = new System.Windows.Forms.TabPage();
            splitContainerMain = new System.Windows.Forms.SplitContainer();
            leftPanel = new System.Windows.Forms.TableLayoutPanel();
            lblFilesTitle = new System.Windows.Forms.Label();
            lstResxFiles = new System.Windows.Forms.ListBox();
            rightPanel = new System.Windows.Forms.TableLayoutPanel();
            headerPanel = new System.Windows.Forms.TableLayoutPanel();
            leftHeaderFlow = new System.Windows.Forms.FlowLayoutPanel();
            lblSearch = new System.Windows.Forms.Label();
            txtSearch = new System.Windows.Forms.TextBox();
            btnClearFilter = new System.Windows.Forms.Button();
            btnDeleteTranslation = new System.Windows.Forms.Button();
            lblEntryCount = new System.Windows.Forms.Label();
            zoomFlow = new System.Windows.Forms.FlowLayoutPanel();
            lblZoomLabel = new System.Windows.Forms.Label();
            btnZoomOut = new System.Windows.Forms.Button();
            btnZoomReset = new System.Windows.Forms.Button();
            btnZoomIn = new System.Windows.Forms.Button();
            lblZoomLevel = new System.Windows.Forms.Label();
            dgvEntries = new System.Windows.Forms.DataGridView();
            lblStatus = new System.Windows.Forms.Label();
            tabPageLog = new System.Windows.Forms.TabPage();
            dgvLog = new System.Windows.Forms.DataGridView();
            btnRestoreFromLog = new System.Windows.Forms.Button();
            btnClearLog = new System.Windows.Forms.Button();
            lblLogHint = new System.Windows.Forms.Label();
            btnCreateTranslation = new System.Windows.Forms.Button();
            contextMenuTranslate = new System.Windows.Forms.ContextMenuStrip(components);
            copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            contextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            translateChatGPTMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            translatePerplexityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            translateGoogleAIMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            entriesBindingSource = new System.Windows.Forms.BindingSource(components);
            folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            loadingPanel = new System.Windows.Forms.Panel();
            progressBar = new System.Windows.Forms.ProgressBar();
            lblLoadingStatus = new System.Windows.Forms.Label();
            btnCancelLoading = new System.Windows.Forms.Button();
            topPanel.SuspendLayout();
            menuStrip.SuspendLayout();
            tabControlMain.SuspendLayout();
            tabPageEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainerMain)).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            leftPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            headerPanel.SuspendLayout();
            leftHeaderFlow.SuspendLayout();
            zoomFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvEntries)).BeginInit();
            tabPageLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(entriesBindingSource)).BeginInit();
            SuspendLayout();
            //
            // topPanel
            //
            topPanel.AutoSize = true;
            topPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            topPanel.BackColor = System.Drawing.Color.White;
            topPanel.ColumnCount = 2;
            topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            topPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            topPanel.Controls.Add(btnSelectFolder, 0, 0);
            topPanel.Controls.Add(txtFolderPath, 1, 0);
            topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            topPanel.Location = new System.Drawing.Point(0, 28);
            topPanel.Margin = new System.Windows.Forms.Padding(0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new System.Windows.Forms.Padding(8);
            topPanel.RowCount = 1;
            topPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            topPanel.Size = new System.Drawing.Size(1000, 50);
            topPanel.TabIndex = 1;
            //
            // btnSelectFolder
            //
            btnSelectFolder.AutoSize = true;
            btnSelectFolder.BackColor = System.Drawing.Color.FromArgb(34, 156, 215);
            btnSelectFolder.FlatAppearance.BorderSize = 0;
            btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSelectFolder.ForeColor = System.Drawing.Color.White;
            btnSelectFolder.Location = new System.Drawing.Point(11, 11);
            btnSelectFolder.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new System.Drawing.Size(118, 27);
            btnSelectFolder.TabIndex = 0;
            btnSelectFolder.Text = "Escolher pasta";
            btnSelectFolder.UseVisualStyleBackColor = false;
            btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            //
            // txtFolderPath
            //
            txtFolderPath.Dock = System.Windows.Forms.DockStyle.Fill;
            txtFolderPath.Location = new System.Drawing.Point(143, 11);
            txtFolderPath.Margin = new System.Windows.Forms.Padding(0);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new System.Drawing.Size(836, 23);
            txtFolderPath.TabIndex = 1;
            //
            // menuStrip
            //
            menuStrip.BackColor = System.Drawing.Color.White;
            menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                arquivoToolStripMenuItem,
                opcoesToolStripMenuItem,
                ajudaToolStripMenuItem
            });
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new System.Drawing.Size(1000, 28);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip";
            //
            // arquivoToolStripMenuItem
            //
            arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                sairToolStripMenuItem
            });
            arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            arquivoToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            arquivoToolStripMenuItem.Text = "Arquivo";
            //
            // sairToolStripMenuItem
            //
            sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            sairToolStripMenuItem.Size = new System.Drawing.Size(110, 24);
            sairToolStripMenuItem.Text = "Sair";
            sairToolStripMenuItem.Click += new System.EventHandler(this.sairToolStripMenuItem_Click);
            //
            // opcoesToolStripMenuItem
            //
            opcoesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                exibirColunaChaveToolStripMenuItem,
                abrirLogErrosToolStripMenuItem
            });
            opcoesToolStripMenuItem.Name = "opcoesToolStripMenuItem";
            opcoesToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            opcoesToolStripMenuItem.Text = "Opções";
            //
            // exibirColunaChaveToolStripMenuItem
            //
            exibirColunaChaveToolStripMenuItem.CheckOnClick = true;
            exibirColunaChaveToolStripMenuItem.Checked = false;
            exibirColunaChaveToolStripMenuItem.Name = "exibirColunaChaveToolStripMenuItem";
            exibirColunaChaveToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            exibirColunaChaveToolStripMenuItem.Text = "Exibir coluna chave";
            exibirColunaChaveToolStripMenuItem.Click += new System.EventHandler(this.exibirColunaChaveToolStripMenuItem_Click);
            //
            // abrirLogErrosToolStripMenuItem
            //
            abrirLogErrosToolStripMenuItem.Name = "abrirLogErrosToolStripMenuItem";
            abrirLogErrosToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            abrirLogErrosToolStripMenuItem.Text = "Abrir log de erros";
            abrirLogErrosToolStripMenuItem.Click += new System.EventHandler(this.abrirLogErrosToolStripMenuItem_Click);
            //
            // ajudaToolStripMenuItem
            //
            ajudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                tutorialAjudaToolStripMenuItem,
                toolStripSeparatorAjuda,
                sobreToolStripMenuItem
            });
            ajudaToolStripMenuItem.Name = "ajudaToolStripMenuItem";
            ajudaToolStripMenuItem.Size = new System.Drawing.Size(55, 24);
            ajudaToolStripMenuItem.Text = "Ajuda";
            //
            // tutorialAjudaToolStripMenuItem
            //
            tutorialAjudaToolStripMenuItem.Name = "tutorialAjudaToolStripMenuItem";
            tutorialAjudaToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            tutorialAjudaToolStripMenuItem.Text = "Tutorial";
            tutorialAjudaToolStripMenuItem.Click += new System.EventHandler(this.tutorialToolStripMenuItem_Click);
            //
            // toolStripSeparatorAjuda
            //
            toolStripSeparatorAjuda.Name = "toolStripSeparatorAjuda";
            toolStripSeparatorAjuda.Size = new System.Drawing.Size(177, 6);
            //
            // sobreToolStripMenuItem
            //
            sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            sobreToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            sobreToolStripMenuItem.Text = "Sobre";
            sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            //
            // tabControlMain
            //
            tabControlMain.Controls.Add(tabPageEditor);
            tabControlMain.Controls.Add(tabPageLog);
            tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlMain.Location = new System.Drawing.Point(0, 78);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new System.Drawing.Size(1000, 522);
            tabControlMain.TabIndex = 2;
            //
            // tabPageEditor
            //
            tabPageEditor.Controls.Add(splitContainerMain);
            tabPageEditor.Location = new System.Drawing.Point(4, 24);
            tabPageEditor.Name = "tabPageEditor";
            tabPageEditor.Padding = new System.Windows.Forms.Padding(0);
            tabPageEditor.Size = new System.Drawing.Size(992, 494);
            tabPageEditor.TabIndex = 0;
            tabPageEditor.Text = "Editor";
            tabPageEditor.UseVisualStyleBackColor = true;
            //
            // splitContainerMain
            //
            splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainerMain.Location = new System.Drawing.Point(0, 0);
            splitContainerMain.Margin = new System.Windows.Forms.Padding(0);
            splitContainerMain.Name = "splitContainerMain";
            //
            // splitContainerMain.Panel1
            //
            splitContainerMain.Panel1.Padding = new System.Windows.Forms.Padding(8);
            splitContainerMain.Panel1.Controls.Add(leftPanel);
            //
            // splitContainerMain.Panel2
            //
            splitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(8);
            splitContainerMain.Panel2.Controls.Add(rightPanel);
            splitContainerMain.Size = new System.Drawing.Size(992, 494);
            splitContainerMain.SplitterDistance = 200;
            splitContainerMain.TabIndex = 0;
            //
            // leftPanel
            //
            leftPanel.BackColor = System.Drawing.Color.White;
            leftPanel.ColumnCount = 1;
            leftPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftPanel.Controls.Add(lblFilesTitle, 0, 0);
            leftPanel.Controls.Add(lstResxFiles, 0, 1);
            leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            leftPanel.Location = new System.Drawing.Point(8, 8);
            leftPanel.Name = "leftPanel";
            leftPanel.RowCount = 2;
            leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            leftPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            leftPanel.Size = new System.Drawing.Size(184, 478);
            leftPanel.TabIndex = 0;
            //
            // lblFilesTitle
            //
            lblFilesTitle.AutoSize = true;
            lblFilesTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            lblFilesTitle.Location = new System.Drawing.Point(3, 0);
            lblFilesTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            lblFilesTitle.Name = "lblFilesTitle";
            lblFilesTitle.Size = new System.Drawing.Size(168, 17);
            lblFilesTitle.TabIndex = 0;
            lblFilesTitle.Text = "Arquivos (pares zh/en)";
            //
            // lstResxFiles
            //
            lstResxFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lstResxFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            lstResxFiles.Font = new System.Drawing.Font("Segoe UI", 9.25F);
            lstResxFiles.FormattingEnabled = true;
            lstResxFiles.ItemHeight = 15;
            lstResxFiles.Location = new System.Drawing.Point(3, 28);
            lstResxFiles.Name = "lstResxFiles";
            lstResxFiles.Size = new System.Drawing.Size(178, 447);
            lstResxFiles.TabIndex = 1;
            lstResxFiles.SelectedIndexChanged += new System.EventHandler(this.lstResxFiles_SelectedIndexChanged);
            //
            // rightPanel
            //
            rightPanel.BackColor = System.Drawing.Color.White;
            rightPanel.ColumnCount = 1;
            rightPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightPanel.Controls.Add(headerPanel, 0, 0);
            rightPanel.Controls.Add(dgvEntries, 0, 1);
            rightPanel.Controls.Add(lblStatus, 0, 2);
            rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            rightPanel.Location = new System.Drawing.Point(8, 8);
            rightPanel.Name = "rightPanel";
            rightPanel.RowCount = 3;
            rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            rightPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            rightPanel.Size = new System.Drawing.Size(772, 478);
            rightPanel.TabIndex = 0;
            //
            // headerPanel
            //
            headerPanel.AutoSize = true;
            headerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            headerPanel.ColumnCount = 2;
            headerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            headerPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            headerPanel.Controls.Add(leftHeaderFlow, 0, 0);
            headerPanel.Controls.Add(zoomFlow, 1, 0);
            headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            headerPanel.Location = new System.Drawing.Point(3, 3);
            headerPanel.Name = "headerPanel";
            headerPanel.RowCount = 1;
            headerPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            headerPanel.Size = new System.Drawing.Size(766, 33);
            headerPanel.TabIndex = 0;
            //
            // leftHeaderFlow
            //
            leftHeaderFlow.AutoSize = true;
            leftHeaderFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            leftHeaderFlow.Controls.Add(btnCreateTranslation);
            leftHeaderFlow.Controls.Add(btnDeleteTranslation);
            leftHeaderFlow.Controls.Add(lblSearch);
            leftHeaderFlow.Controls.Add(txtSearch);
            leftHeaderFlow.Controls.Add(btnClearFilter);
            leftHeaderFlow.Controls.Add(lblEntryCount);
            leftHeaderFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            leftHeaderFlow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            leftHeaderFlow.Location = new System.Drawing.Point(0, 0);
            leftHeaderFlow.Margin = new System.Windows.Forms.Padding(0);
            leftHeaderFlow.Name = "leftHeaderFlow";
            leftHeaderFlow.Padding = new System.Windows.Forms.Padding(0);
            leftHeaderFlow.Size = new System.Drawing.Size(585, 33);
            leftHeaderFlow.TabIndex = 0;
            leftHeaderFlow.WrapContents = false;
            //
            // lblSearch
            //
            lblSearch.AutoSize = true;
            lblSearch.Location = new System.Drawing.Point(0, 8);
            lblSearch.Margin = new System.Windows.Forms.Padding(0, 8, 4, 3);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new System.Drawing.Size(44, 15);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Buscar";
            //
            // txtSearch
            //
            txtSearch.AutoSize = false;
            txtSearch.Location = new System.Drawing.Point(48, 3);
            txtSearch.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new System.Drawing.Size(240, 27);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            //
            // btnClearFilter
            //
            btnClearFilter.AutoSize = true;
            btnClearFilter.Location = new System.Drawing.Point(298, 3);
            btnClearFilter.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new System.Drawing.Size(90, 27);
            btnClearFilter.TabIndex = 2;
            btnClearFilter.Text = "Limpar filtro";
            btnClearFilter.UseVisualStyleBackColor = true;
            btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            //
            // btnDeleteTranslation
            //
            btnDeleteTranslation.AutoSize = true;
            btnDeleteTranslation.Location = new System.Drawing.Point(120, 3);
            btnDeleteTranslation.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            btnDeleteTranslation.Name = "btnDeleteTranslation";
            btnDeleteTranslation.Size = new System.Drawing.Size(120, 27);
            btnDeleteTranslation.TabIndex = 1;
            btnDeleteTranslation.Text = "Excluir tradução";
            btnDeleteTranslation.UseVisualStyleBackColor = true;
            btnDeleteTranslation.Enabled = false;
            btnDeleteTranslation.Click += new System.EventHandler(this.btnDeleteTranslation_Click);
            //
            // lblEntryCount
            //
            lblEntryCount.AutoSize = true;
            lblEntryCount.Location = new System.Drawing.Point(458, 9);
            lblEntryCount.Margin = new System.Windows.Forms.Padding(0, 9, 0, 0);
            lblEntryCount.Name = "lblEntryCount";
            lblEntryCount.Size = new System.Drawing.Size(76, 15);
            lblEntryCount.TabIndex = 3;
            lblEntryCount.Text = "Entradas: 0";
            //
            // zoomFlow
            //
            zoomFlow.AutoSize = true;
            zoomFlow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            zoomFlow.Controls.Add(lblZoomLabel);
            zoomFlow.Controls.Add(btnZoomOut);
            zoomFlow.Controls.Add(btnZoomReset);
            zoomFlow.Controls.Add(btnZoomIn);
            zoomFlow.Controls.Add(lblZoomLevel);
            zoomFlow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            zoomFlow.Location = new System.Drawing.Point(585, 3);
            zoomFlow.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            zoomFlow.Name = "zoomFlow";
            zoomFlow.Size = new System.Drawing.Size(181, 27);
            zoomFlow.TabIndex = 1;
            zoomFlow.WrapContents = false;
            //
            // lblZoomLabel
            //
            lblZoomLabel.AutoSize = true;
            lblZoomLabel.Location = new System.Drawing.Point(0, 7);
            lblZoomLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 3);
            lblZoomLabel.Name = "lblZoomLabel";
            lblZoomLabel.Size = new System.Drawing.Size(45, 15);
            lblZoomLabel.TabIndex = 0;
            lblZoomLabel.Text = "Zoom:";
            //
            // btnZoomOut
            //
            btnZoomOut.AutoSize = true;
            btnZoomOut.Location = new System.Drawing.Point(49, 3);
            btnZoomOut.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new System.Drawing.Size(35, 27);
            btnZoomOut.TabIndex = 1;
            btnZoomOut.Text = "A-";
            btnZoomOut.UseVisualStyleBackColor = true;
            btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            //
            // btnZoomReset
            //
            btnZoomReset.AutoSize = true;
            btnZoomReset.Location = new System.Drawing.Point(88, 3);
            btnZoomReset.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
            btnZoomReset.Name = "btnZoomReset";
            btnZoomReset.Size = new System.Drawing.Size(45, 27);
            btnZoomReset.TabIndex = 2;
            btnZoomReset.Text = "100%";
            btnZoomReset.UseVisualStyleBackColor = true;
            btnZoomReset.Click += new System.EventHandler(this.btnZoomReset_Click);
            //
            // btnZoomIn
            //
            btnZoomIn.AutoSize = true;
            btnZoomIn.Location = new System.Drawing.Point(137, 3);
            btnZoomIn.Margin = new System.Windows.Forms.Padding(0, 3, 4, 3);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new System.Drawing.Size(35, 27);
            btnZoomIn.TabIndex = 3;
            btnZoomIn.Text = "A+";
            btnZoomIn.UseVisualStyleBackColor = true;
            btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            //
            // lblZoomLevel
            //
            lblZoomLevel.AutoSize = true;
            lblZoomLevel.Location = new System.Drawing.Point(176, 8);
            lblZoomLevel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            lblZoomLevel.Name = "lblZoomLevel";
            lblZoomLevel.Size = new System.Drawing.Size(33, 15);
            lblZoomLevel.TabIndex = 4;
            lblZoomLevel.Text = "100%";
            //
            // dgvEntries
            //
            dgvEntries.AllowUserToAddRows = false;
            dgvEntries.AllowUserToDeleteRows = false;
            dgvEntries.AllowUserToResizeRows = true;
            dgvEntries.AutoGenerateColumns = true;
            dgvEntries.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgvEntries.BackgroundColor = System.Drawing.Color.White;
            dgvEntries.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvEntries.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dgvEntries.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEntries.ContextMenuStrip = contextMenuTranslate;
            dgvEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvEntries.EnableHeadersVisualStyles = false;
            dgvEntries.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);
            dgvEntries.Location = new System.Drawing.Point(3, 42);
            dgvEntries.Name = "dgvEntries";
            dgvEntries.RowHeadersVisible = false;
            dgvEntries.RowTemplate.Height = 50;
            dgvEntries.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvEntries.Size = new System.Drawing.Size(766, 388);
            dgvEntries.TabIndex = 1;
            dgvEntries.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 250);
            dgvEntries.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEntries_CellEndEdit);
            //
            // lblStatus
            //
            lblStatus.AutoSize = true;
            lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            lblStatus.ForeColor = System.Drawing.Color.Gray;
            lblStatus.Location = new System.Drawing.Point(3, 433);
            lblStatus.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(766, 13);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Aguardando pasta selecionada...";
            lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tabPageLog
            //
            tabPageLog.Controls.Add(dgvLog);
            tabPageLog.Controls.Add(btnRestoreFromLog);
            tabPageLog.Controls.Add(btnClearLog);
            tabPageLog.Controls.Add(lblLogHint);
            tabPageLog.Location = new System.Drawing.Point(4, 24);
            tabPageLog.Name = "tabPageLog";
            tabPageLog.Padding = new System.Windows.Forms.Padding(8);
            tabPageLog.Size = new System.Drawing.Size(992, 494);
            tabPageLog.TabIndex = 1;
            tabPageLog.Text = "Log";
            tabPageLog.UseVisualStyleBackColor = true;
            //
            // lblLogHint
            //
            lblLogHint.AutoSize = true;
            lblLogHint.Dock = System.Windows.Forms.DockStyle.Top;
            lblLogHint.ForeColor = System.Drawing.Color.Gray;
            lblLogHint.Location = new System.Drawing.Point(8, 8);
            lblLogHint.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            lblLogHint.Name = "lblLogHint";
            lblLogHint.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            lblLogHint.Size = new System.Drawing.Size(215, 23);
            lblLogHint.TabIndex = 0;
            lblLogHint.Text = "Selecione uma linha para restaurar.";
            //
            // btnRestoreFromLog
            //
            btnRestoreFromLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnRestoreFromLog.Enabled = false;
            btnRestoreFromLog.Height = 38;
            btnRestoreFromLog.Name = "btnRestoreFromLog";
            btnRestoreFromLog.TabIndex = 2;
            btnRestoreFromLog.Text = "Restaurar chave selecionada";
            btnRestoreFromLog.UseVisualStyleBackColor = true;
            btnRestoreFromLog.Click += new System.EventHandler(this.btnRestoreFromLog_Click);
            //
            // btnClearLog
            //
            btnClearLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            btnClearLog.Height = 38;
            btnClearLog.Name = "btnClearLog";
            btnClearLog.TabIndex = 3;
            btnClearLog.Text = "Limpar log";
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // btnCreateTranslation
            //
            btnCreateTranslation.AutoSize = true;
            btnCreateTranslation.Location = new System.Drawing.Point(0, 3);
            btnCreateTranslation.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            btnCreateTranslation.Name = "btnCreateTranslation";
            btnCreateTranslation.Size = new System.Drawing.Size(110, 27);
            btnCreateTranslation.TabIndex = 0;
            btnCreateTranslation.Text = "Criar tradução";
            btnCreateTranslation.UseVisualStyleBackColor = true;
            btnCreateTranslation.Enabled = false;
            btnCreateTranslation.Click += new System.EventHandler(this.btnCreateTranslation_Click);
            //
            // contextMenuTranslate
            //
            contextMenuTranslate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            copyMenuItem,
            contextMenuSeparator,
            translateChatGPTMenuItem,
            translatePerplexityMenuItem,
            translateGoogleAIMenuItem});
            contextMenuTranslate.Name = "contextMenuTranslate";
            contextMenuTranslate.Size = new System.Drawing.Size(200, 120);
            //
            // copyMenuItem
            //
            copyMenuItem.Name = "copyMenuItem";
            copyMenuItem.Size = new System.Drawing.Size(199, 22);
            copyMenuItem.Text = "Copiar célula";
            copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            //
            // contextMenuSeparator
            //
            contextMenuSeparator.Name = "contextMenuSeparator";
            contextMenuSeparator.Size = new System.Drawing.Size(196, 6);
            //
            // translateChatGPTMenuItem
            //
            translateChatGPTMenuItem.Name = "translateChatGPTMenuItem";
            translateChatGPTMenuItem.Size = new System.Drawing.Size(199, 22);
            translateChatGPTMenuItem.Text = "Traduzir com ChatGPT";
            translateChatGPTMenuItem.Click += new System.EventHandler(this.translateChatGPTMenuItem_Click);
            //
            // translatePerplexityMenuItem
            //
            translatePerplexityMenuItem.Name = "translatePerplexityMenuItem";
            translatePerplexityMenuItem.Size = new System.Drawing.Size(199, 22);
            translatePerplexityMenuItem.Text = "Traduzir com Perplexity";
            translatePerplexityMenuItem.Click += new System.EventHandler(this.translatePerplexityMenuItem_Click);
            //
            // translateGoogleAIMenuItem
            //
            translateGoogleAIMenuItem.Name = "translateGoogleAIMenuItem";
            translateGoogleAIMenuItem.Size = new System.Drawing.Size(199, 22);
            translateGoogleAIMenuItem.Text = "Traduzir com Google AI";
            translateGoogleAIMenuItem.Click += new System.EventHandler(this.translateGoogleAIMenuItem_Click);
            //
            // dgvLog
            //
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.AllowUserToResizeRows = false;
            dgvLog.AutoGenerateColumns = true;
            dgvLog.BackgroundColor = System.Drawing.Color.White;
            dgvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvLog.Location = new System.Drawing.Point(8, 31);
            dgvLog.MultiSelect = false;
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.RowHeadersVisible = false;
            dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvLog.Size = new System.Drawing.Size(976, 425);
            dgvLog.TabIndex = 1;
            dgvLog.SelectionChanged += new System.EventHandler(this.dgvLog_SelectionChanged);
            //
            // loadingPanel
            //
            loadingPanel.BackColor = System.Drawing.Color.FromArgb(240, 240, 245);
            loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            loadingPanel.Controls.Add(btnCancelLoading);
            loadingPanel.Controls.Add(progressBar);
            loadingPanel.Controls.Add(lblLoadingStatus);
            loadingPanel.Location = new System.Drawing.Point(300, 250);
            loadingPanel.Name = "loadingPanel";
            loadingPanel.Size = new System.Drawing.Size(400, 120);
            loadingPanel.Visible = false;
            //
            // lblLoadingStatus
            //
            lblLoadingStatus.AutoSize = false;
            lblLoadingStatus.Location = new System.Drawing.Point(20, 15);
            lblLoadingStatus.Name = "lblLoadingStatus";
            lblLoadingStatus.Size = new System.Drawing.Size(360, 20);
            lblLoadingStatus.Text = "Carregando...";
            lblLoadingStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // progressBar
            //
            progressBar.Location = new System.Drawing.Point(20, 45);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(360, 25);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
            //
            // btnCancelLoading
            //
            btnCancelLoading.Location = new System.Drawing.Point(150, 80);
            btnCancelLoading.Name = "btnCancelLoading";
            btnCancelLoading.Size = new System.Drawing.Size(100, 30);
            btnCancelLoading.TabIndex = 0;
            btnCancelLoading.Text = "Cancelar";
            btnCancelLoading.UseVisualStyleBackColor = true;
            btnCancelLoading.Click += new System.EventHandler(this.btnCancelLoading_Click);
            //
            // MainForm
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(246, 246, 250);
            ClientSize = new System.Drawing.Size(1000, 600);
            Controls.Add(loadingPanel);
            Controls.Add(tabControlMain);
            Controls.Add(topPanel);
            Controls.Add(menuStrip);
            Font = new System.Drawing.Font("Segoe UI", 9F);
            Icon = new System.Drawing.Icon("ico.ico");
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            Text = "Editor de Traduções";
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tabControlMain.ResumeLayout(false);
            tabPageEditor.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainerMain)).EndInit();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            splitContainerMain.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            rightPanel.ResumeLayout(false);
            rightPanel.PerformLayout();
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            leftHeaderFlow.ResumeLayout(false);
            leftHeaderFlow.PerformLayout();
            zoomFlow.ResumeLayout(false);
            zoomFlow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvEntries)).EndInit();
            tabPageLog.ResumeLayout(false);
            tabPageLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvLog)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}