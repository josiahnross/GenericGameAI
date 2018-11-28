namespace NeuralNetTreeStuffViewer
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
            this.gamesComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.neuralNetTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.openNetFileButton = new System.Windows.Forms.Button();
            this.loadSettingsButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.playWithNetButton = new System.Windows.Forms.Button();
            this.playWith2PlayerButton = new System.Windows.Forms.Button();
            this.GenerateData = new System.Windows.Forms.Button();
            this.trainNeuralNetButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // gamesComboBox
            // 
            this.gamesComboBox.FormattingEnabled = true;
            this.gamesComboBox.Location = new System.Drawing.Point(56, 6);
            this.gamesComboBox.Name = "gamesComboBox";
            this.gamesComboBox.Size = new System.Drawing.Size(121, 21);
            this.gamesComboBox.TabIndex = 0;
            this.gamesComboBox.TextChanged += new System.EventHandler(this.gamesComboBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game:";
            // 
            // neuralNetTextBox
            // 
            this.neuralNetTextBox.Location = new System.Drawing.Point(73, 34);
            this.neuralNetTextBox.Name = "neuralNetTextBox";
            this.neuralNetTextBox.Size = new System.Drawing.Size(104, 20);
            this.neuralNetTextBox.TabIndex = 2;
            this.neuralNetTextBox.TextChanged += new System.EventHandler(this.neuralNetTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Neural Net:";
            // 
            // openNetFileButton
            // 
            this.openNetFileButton.Location = new System.Drawing.Point(183, 34);
            this.openNetFileButton.Name = "openNetFileButton";
            this.openNetFileButton.Size = new System.Drawing.Size(82, 20);
            this.openNetFileButton.TabIndex = 4;
            this.openNetFileButton.Text = "Open Net File";
            this.openNetFileButton.UseVisualStyleBackColor = true;
            this.openNetFileButton.Click += new System.EventHandler(this.openNetFileButton_Click);
            // 
            // loadSettingsButton
            // 
            this.loadSettingsButton.Location = new System.Drawing.Point(12, 227);
            this.loadSettingsButton.Name = "loadSettingsButton";
            this.loadSettingsButton.Size = new System.Drawing.Size(99, 23);
            this.loadSettingsButton.TabIndex = 5;
            this.loadSettingsButton.Text = "Load Settings";
            this.loadSettingsButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(202, 59);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 21);
            this.button1.TabIndex = 8;
            this.button1.Text = "Open Training Data File";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Training Data:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(92, 60);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(104, 20);
            this.textBox1.TabIndex = 6;
            // 
            // playWithNetButton
            // 
            this.playWithNetButton.Location = new System.Drawing.Point(12, 93);
            this.playWithNetButton.Name = "playWithNetButton";
            this.playWithNetButton.Size = new System.Drawing.Size(132, 23);
            this.playWithNetButton.TabIndex = 9;
            this.playWithNetButton.Text = "Play Game With Net";
            this.playWithNetButton.UseVisualStyleBackColor = true;
            this.playWithNetButton.Click += new System.EventHandler(this.playWithNetButton_Click);
            // 
            // playWith2PlayerButton
            // 
            this.playWith2PlayerButton.Location = new System.Drawing.Point(12, 122);
            this.playWith2PlayerButton.Name = "playWith2PlayerButton";
            this.playWith2PlayerButton.Size = new System.Drawing.Size(132, 23);
            this.playWith2PlayerButton.TabIndex = 11;
            this.playWith2PlayerButton.Text = "Play Two Player";
            this.playWith2PlayerButton.UseVisualStyleBackColor = true;
            this.playWith2PlayerButton.Click += new System.EventHandler(this.playWith2PlayerButton_Click);
            // 
            // GenerateData
            // 
            this.GenerateData.Location = new System.Drawing.Point(12, 151);
            this.GenerateData.Name = "GenerateData";
            this.GenerateData.Size = new System.Drawing.Size(132, 23);
            this.GenerateData.TabIndex = 12;
            this.GenerateData.Text = "Generate Training Data";
            this.GenerateData.UseVisualStyleBackColor = true;
            // 
            // trainNeuralNetButton
            // 
            this.trainNeuralNetButton.Location = new System.Drawing.Point(12, 180);
            this.trainNeuralNetButton.Name = "trainNeuralNetButton";
            this.trainNeuralNetButton.Size = new System.Drawing.Size(132, 23);
            this.trainNeuralNetButton.TabIndex = 13;
            this.trainNeuralNetButton.Text = "Train Neural Net";
            this.trainNeuralNetButton.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Neural Network files (*.net)|*.net";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "Training Data files (*.train)|*.train";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 258);
            this.Controls.Add(this.trainNeuralNetButton);
            this.Controls.Add(this.GenerateData);
            this.Controls.Add(this.playWith2PlayerButton);
            this.Controls.Add(this.playWithNetButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.loadSettingsButton);
            this.Controls.Add(this.openNetFileButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.neuralNetTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gamesComboBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox gamesComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox neuralNetTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button openNetFileButton;
        private System.Windows.Forms.Button loadSettingsButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button playWithNetButton;
        private System.Windows.Forms.Button playWith2PlayerButton;
        private System.Windows.Forms.Button GenerateData;
        private System.Windows.Forms.Button trainNeuralNetButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}

