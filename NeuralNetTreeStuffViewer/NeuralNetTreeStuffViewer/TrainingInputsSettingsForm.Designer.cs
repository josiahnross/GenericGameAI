namespace NeuralNetTreeStuffViewer
{
    partial class TrainingInputsSettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.simulationNumeric = new System.Windows.Forms.NumericUpDown();
            this.depthNumeric = new System.Windows.Forms.NumericUpDown();
            this.evaluatorComboBox = new System.Windows.Forms.ComboBox();
            this.removeDrawsCheck = new System.Windows.Forms.CheckBox();
            this.continueButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.getFolderButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderNameTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.simulationNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Amount of Simulations:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Move Evaluator:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Max MCT Depth:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Remove Draws:";
            // 
            // simulationNumeric
            // 
            this.simulationNumeric.Location = new System.Drawing.Point(132, 7);
            this.simulationNumeric.Name = "simulationNumeric";
            this.simulationNumeric.Size = new System.Drawing.Size(120, 20);
            this.simulationNumeric.TabIndex = 4;
            this.simulationNumeric.ValueChanged += new System.EventHandler(this.simulationNumeric_ValueChanged);
            // 
            // depthNumeric
            // 
            this.depthNumeric.Location = new System.Drawing.Point(106, 59);
            this.depthNumeric.Name = "depthNumeric";
            this.depthNumeric.Size = new System.Drawing.Size(146, 20);
            this.depthNumeric.TabIndex = 5;
            this.depthNumeric.ValueChanged += new System.EventHandler(this.depthNumeric_ValueChanged);
            // 
            // evaluatorComboBox
            // 
            this.evaluatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.evaluatorComboBox.FormattingEnabled = true;
            this.evaluatorComboBox.Location = new System.Drawing.Point(102, 31);
            this.evaluatorComboBox.Name = "evaluatorComboBox";
            this.evaluatorComboBox.Size = new System.Drawing.Size(150, 21);
            this.evaluatorComboBox.TabIndex = 6;
            // 
            // removeDrawsCheck
            // 
            this.removeDrawsCheck.AutoSize = true;
            this.removeDrawsCheck.Location = new System.Drawing.Point(101, 155);
            this.removeDrawsCheck.Name = "removeDrawsCheck";
            this.removeDrawsCheck.Size = new System.Drawing.Size(15, 14);
            this.removeDrawsCheck.TabIndex = 7;
            this.removeDrawsCheck.UseVisualStyleBackColor = true;
            // 
            // continueButton
            // 
            this.continueButton.Enabled = false;
            this.continueButton.Location = new System.Drawing.Point(122, 155);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(130, 23);
            this.continueButton.TabIndex = 8;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(75, 85);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(177, 20);
            this.fileNameTextBox.TabIndex = 9;
            this.fileNameTextBox.TextChanged += new System.EventHandler(this.fileNameTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "File Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Folder:";
            // 
            // getFolderButton
            // 
            this.getFolderButton.Location = new System.Drawing.Point(173, 109);
            this.getFolderButton.Name = "getFolderButton";
            this.getFolderButton.Size = new System.Drawing.Size(79, 23);
            this.getFolderButton.TabIndex = 13;
            this.getFolderButton.Text = "Get Folder";
            this.getFolderButton.UseVisualStyleBackColor = true;
            this.getFolderButton.Click += new System.EventHandler(this.getFolderButton_Click);
            // 
            // folderNameTextBox
            // 
            this.folderNameTextBox.Enabled = false;
            this.folderNameTextBox.Location = new System.Drawing.Point(57, 111);
            this.folderNameTextBox.Name = "folderNameTextBox";
            this.folderNameTextBox.Size = new System.Drawing.Size(110, 20);
            this.folderNameTextBox.TabIndex = 14;
            // 
            // TrainingInputsSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 215);
            this.Controls.Add(this.folderNameTextBox);
            this.Controls.Add(this.getFolderButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.removeDrawsCheck);
            this.Controls.Add(this.evaluatorComboBox);
            this.Controls.Add(this.depthNumeric);
            this.Controls.Add(this.simulationNumeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TrainingInputsSettingsForm";
            this.Text = "TrainingInputsSettingsForm";
            this.Load += new System.EventHandler(this.TrainingInputsSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.simulationNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depthNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown simulationNumeric;
        private System.Windows.Forms.NumericUpDown depthNumeric;
        private System.Windows.Forms.ComboBox evaluatorComboBox;
        private System.Windows.Forms.CheckBox removeDrawsCheck;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button getFolderButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox folderNameTextBox;
    }
}