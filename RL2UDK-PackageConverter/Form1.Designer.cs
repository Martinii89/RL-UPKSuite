
namespace RL2UDK_PackageConverter
{
    partial class Title
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.runButton = new System.Windows.Forms.Button();
            this.tabViewController = new System.Windows.Forms.TabControl();
            this.MainPage = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Packages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionsPage = new System.Windows.Forms.TabPage();
            this.outputFolderText = new System.Windows.Forms.TextBox();
            this.outputFolderButton = new System.Windows.Forms.Button();
            this.keyFileText = new System.Windows.Forms.TextBox();
            this.keyFileButton = new System.Windows.Forms.Button();
            this.materialExportToggle = new System.Windows.Forms.CheckBox();
            this.compressPackageToggle = new System.Windows.Forms.CheckBox();
            this.consoleLogTab = new System.Windows.Forms.TabPage();
            this.outputLogConsole = new System.Windows.Forms.RichTextBox();
            this.tabViewController.SuspendLayout();
            this.MainPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.OptionsPage.SuspendLayout();
            this.consoleLogTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.runButton.Location = new System.Drawing.Point(12, 395);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // tabViewController
            // 
            this.tabViewController.Controls.Add(this.MainPage);
            this.tabViewController.Controls.Add(this.OptionsPage);
            this.tabViewController.Controls.Add(this.consoleLogTab);
            this.tabViewController.Location = new System.Drawing.Point(12, 12);
            this.tabViewController.Name = "tabViewController";
            this.tabViewController.SelectedIndex = 0;
            this.tabViewController.Size = new System.Drawing.Size(864, 377);
            this.tabViewController.TabIndex = 1;
            // 
            // MainPage
            // 
            this.MainPage.BackColor = System.Drawing.Color.Transparent;
            this.MainPage.Controls.Add(this.dataGridView1);
            this.MainPage.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.MainPage.Location = new System.Drawing.Point(4, 22);
            this.MainPage.Name = "MainPage";
            this.MainPage.Padding = new System.Windows.Forms.Padding(3);
            this.MainPage.Size = new System.Drawing.Size(856, 351);
            this.MainPage.TabIndex = 0;
            this.MainPage.Text = "Main";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowDrop = true;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Packages});
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(847, 339);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.dataGridView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // Packages
            // 
            this.Packages.DataPropertyName = "Packages";
            this.Packages.HeaderText = "Packages Selected";
            this.Packages.Name = "Packages";
            this.Packages.Width = 800;
            // 
            // OptionsPage
            // 
            this.OptionsPage.Controls.Add(this.outputFolderText);
            this.OptionsPage.Controls.Add(this.outputFolderButton);
            this.OptionsPage.Controls.Add(this.keyFileText);
            this.OptionsPage.Controls.Add(this.keyFileButton);
            this.OptionsPage.Controls.Add(this.materialExportToggle);
            this.OptionsPage.Controls.Add(this.compressPackageToggle);
            this.OptionsPage.Location = new System.Drawing.Point(4, 22);
            this.OptionsPage.Name = "OptionsPage";
            this.OptionsPage.Padding = new System.Windows.Forms.Padding(3);
            this.OptionsPage.Size = new System.Drawing.Size(856, 351);
            this.OptionsPage.TabIndex = 1;
            this.OptionsPage.Text = "Options";
            this.OptionsPage.UseVisualStyleBackColor = true;
            // 
            // outputFolderText
            // 
            this.outputFolderText.AllowDrop = true;
            this.outputFolderText.Location = new System.Drawing.Point(98, 46);
            this.outputFolderText.Name = "outputFolderText";
            this.outputFolderText.Size = new System.Drawing.Size(752, 20);
            this.outputFolderText.TabIndex = 5;
            this.outputFolderText.DragDrop += new System.Windows.Forms.DragEventHandler(this.outputFolderDrop);
            this.outputFolderText.DragEnter += new System.Windows.Forms.DragEventHandler(this.outputFolderEnter);
            // 
            // outputFolderButton
            // 
            this.outputFolderButton.Location = new System.Drawing.Point(6, 44);
            this.outputFolderButton.Name = "outputFolderButton";
            this.outputFolderButton.Size = new System.Drawing.Size(86, 23);
            this.outputFolderButton.TabIndex = 4;
            this.outputFolderButton.Text = "Output Folder";
            this.outputFolderButton.UseVisualStyleBackColor = true;
            this.outputFolderButton.Click += new System.EventHandler(this.outputFolderButton_Click);
            // 
            // keyFileText
            // 
            this.keyFileText.AllowDrop = true;
            this.keyFileText.Location = new System.Drawing.Point(98, 8);
            this.keyFileText.Name = "keyFileText";
            this.keyFileText.Size = new System.Drawing.Size(752, 20);
            this.keyFileText.TabIndex = 3;
            this.keyFileText.DragDrop += new System.Windows.Forms.DragEventHandler(this.keyFileDrop);
            this.keyFileText.DragEnter += new System.Windows.Forms.DragEventHandler(this.keyFileEnter);
            // 
            // keyFileButton
            // 
            this.keyFileButton.Location = new System.Drawing.Point(6, 6);
            this.keyFileButton.Name = "keyFileButton";
            this.keyFileButton.Size = new System.Drawing.Size(86, 23);
            this.keyFileButton.TabIndex = 2;
            this.keyFileButton.Text = "Key File";
            this.keyFileButton.UseVisualStyleBackColor = true;
            this.keyFileButton.Click += new System.EventHandler(this.keyFileButton_Click);
            // 
            // materialExportToggle
            // 
            this.materialExportToggle.AutoSize = true;
            this.materialExportToggle.Location = new System.Drawing.Point(130, 82);
            this.materialExportToggle.Name = "materialExportToggle";
            this.materialExportToggle.Size = new System.Drawing.Size(228, 17);
            this.materialExportToggle.TabIndex = 1;
            this.materialExportToggle.Text = "Material Export \"Fixups\" (Not Implemented)";
            this.materialExportToggle.UseVisualStyleBackColor = true;
            // 
            // compressPackageToggle
            // 
            this.compressPackageToggle.AutoSize = true;
            this.compressPackageToggle.Location = new System.Drawing.Point(6, 82);
            this.compressPackageToggle.Name = "compressPackageToggle";
            this.compressPackageToggle.Size = new System.Drawing.Size(118, 17);
            this.compressPackageToggle.TabIndex = 0;
            this.compressPackageToggle.Text = "Compress Package";
            this.compressPackageToggle.UseVisualStyleBackColor = true;
            // 
            // consoleLogTab
            // 
            this.consoleLogTab.Controls.Add(this.outputLogConsole);
            this.consoleLogTab.Location = new System.Drawing.Point(4, 22);
            this.consoleLogTab.Name = "consoleLogTab";
            this.consoleLogTab.Padding = new System.Windows.Forms.Padding(3);
            this.consoleLogTab.Size = new System.Drawing.Size(856, 351);
            this.consoleLogTab.TabIndex = 2;
            this.consoleLogTab.Text = "Console Log";
            this.consoleLogTab.UseVisualStyleBackColor = true;
            // 
            // outputLogConsole
            // 
            this.outputLogConsole.Location = new System.Drawing.Point(3, 6);
            this.outputLogConsole.Name = "outputLogConsole";
            this.outputLogConsole.Size = new System.Drawing.Size(847, 339);
            this.outputLogConsole.TabIndex = 0;
            this.outputLogConsole.Text = "";
            // 
            // Title
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(900, 423);
            this.Controls.Add(this.tabViewController);
            this.Controls.Add(this.runButton);
            this.Name = "Title";
            this.Text = "RL2UDK Converter";
            this.tabViewController.ResumeLayout(false);
            this.MainPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.OptionsPage.ResumeLayout(false);
            this.OptionsPage.PerformLayout();
            this.consoleLogTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.TabControl tabViewController;
        private System.Windows.Forms.TabPage MainPage;
        private System.Windows.Forms.TabPage OptionsPage;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage consoleLogTab;
        private System.Windows.Forms.TextBox outputFolderText;
        private System.Windows.Forms.Button outputFolderButton;
        private System.Windows.Forms.TextBox keyFileText;
        private System.Windows.Forms.Button keyFileButton;
        private System.Windows.Forms.CheckBox materialExportToggle;
        private System.Windows.Forms.CheckBox compressPackageToggle;
        private System.Windows.Forms.RichTextBox outputLogConsole;
        private System.Windows.Forms.DataGridViewTextBoxColumn Packages;
    }
}

