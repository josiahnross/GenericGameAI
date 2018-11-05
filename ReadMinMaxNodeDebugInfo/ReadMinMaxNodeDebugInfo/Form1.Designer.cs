namespace ReadMinMaxNodeDebugInfo
{
    partial class Form1
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.boardPanel = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileButton = new System.Windows.Forms.Button();
            this.justBoardButton = new System.Windows.Forms.Button();
            this.openJustBoardFile = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(403, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(798, 712);
            this.treeView1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 128);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(127, 596);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // boardPanel
            // 
            this.boardPanel.Location = new System.Drawing.Point(145, 12);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.Size = new System.Drawing.Size(252, 289);
            this.boardPanel.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "txt";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.InitialDirectory = "C:\\Users\\Josiah\\Documents\\GitHub\\GenericGameAI\\NeuralNetTreeStuffViewer\\NeuralNet" +
    "TreeStuffViewer\\bin\\Debug";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // fileButton
            // 
            this.fileButton.Location = new System.Drawing.Point(12, 41);
            this.fileButton.Name = "fileButton";
            this.fileButton.Size = new System.Drawing.Size(127, 23);
            this.fileButton.TabIndex = 4;
            this.fileButton.Text = "Open File";
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.fileButton_Click);
            // 
            // justBoardButton
            // 
            this.justBoardButton.Location = new System.Drawing.Point(12, 70);
            this.justBoardButton.Name = "justBoardButton";
            this.justBoardButton.Size = new System.Drawing.Size(127, 23);
            this.justBoardButton.TabIndex = 5;
            this.justBoardButton.Text = "Just Board";
            this.justBoardButton.UseVisualStyleBackColor = true;
            this.justBoardButton.Click += new System.EventHandler(this.justBoardButton_Click);
            // 
            // openJustBoardFile
            // 
            this.openJustBoardFile.Location = new System.Drawing.Point(12, 99);
            this.openJustBoardFile.Name = "openJustBoardFile";
            this.openJustBoardFile.Size = new System.Drawing.Size(127, 23);
            this.openJustBoardFile.TabIndex = 6;
            this.openJustBoardFile.Text = "Open Just Board File";
            this.openJustBoardFile.UseVisualStyleBackColor = true;
            this.openJustBoardFile.Click += new System.EventHandler(this.openJustBoardFile_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "txt";
            this.openFileDialog2.FileName = "openFileDialog1";
            this.openFileDialog2.InitialDirectory = "C:\\Users\\Josiah\\Documents\\GitHub\\GenericGameAI\\NeuralNetTreeStuffViewer\\NeuralNet" +
    "TreeStuffViewer\\bin\\Debug";
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1213, 736);
            this.Controls.Add(this.openJustBoardFile);
            this.Controls.Add(this.justBoardButton);
            this.Controls.Add(this.fileButton);
            this.Controls.Add(this.boardPanel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.treeView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel boardPanel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button fileButton;
        private System.Windows.Forms.Button justBoardButton;
        private System.Windows.Forms.Button openJustBoardFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}

