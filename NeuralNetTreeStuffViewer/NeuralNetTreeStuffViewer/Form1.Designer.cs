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
            this.openTrainingDataButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.trainingDataTextBox = new System.Windows.Forms.TextBox();
            this.playWithNetButton = new System.Windows.Forms.Button();
            this.playWith2PlayerButton = new System.Windows.Forms.Button();
            this.generateInputDataButton = new System.Windows.Forms.Button();
            this.trainNeuralNetButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.generateOutputDataButton = new System.Windows.Forms.Button();
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
            this.loadSettingsButton.Location = new System.Drawing.Point(12, 245);
            this.loadSettingsButton.Name = "loadSettingsButton";
            this.loadSettingsButton.Size = new System.Drawing.Size(99, 23);
            this.loadSettingsButton.TabIndex = 5;
            this.loadSettingsButton.Text = "Load Settings";
            this.loadSettingsButton.UseVisualStyleBackColor = true;
            // 
            // openTrainingDataButton
            // 
            this.openTrainingDataButton.Location = new System.Drawing.Point(202, 59);
            this.openTrainingDataButton.Name = "openTrainingDataButton";
            this.openTrainingDataButton.Size = new System.Drawing.Size(137, 21);
            this.openTrainingDataButton.TabIndex = 8;
            this.openTrainingDataButton.Text = "Open Training Data File";
            this.openTrainingDataButton.UseVisualStyleBackColor = true;
            this.openTrainingDataButton.Click += new System.EventHandler(this.opneTrainingDataButtonClick_Click);
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
            // trainingDataTextBox
            // 
            this.trainingDataTextBox.Location = new System.Drawing.Point(92, 60);
            this.trainingDataTextBox.Name = "trainingDataTextBox";
            this.trainingDataTextBox.Size = new System.Drawing.Size(104, 20);
            this.trainingDataTextBox.TabIndex = 6;
            // 
            // playWithNetButton
            // 
            this.playWithNetButton.Location = new System.Drawing.Point(12, 93);
            this.playWithNetButton.Name = "playWithNetButton";
            this.playWithNetButton.Size = new System.Drawing.Size(165, 23);
            this.playWithNetButton.TabIndex = 9;
            this.playWithNetButton.Text = "Play Game With Net";
            this.playWithNetButton.UseVisualStyleBackColor = true;
            this.playWithNetButton.Click += new System.EventHandler(this.playWithNetButton_Click);
            // 
            // playWith2PlayerButton
            // 
            this.playWith2PlayerButton.Location = new System.Drawing.Point(12, 122);
            this.playWith2PlayerButton.Name = "playWith2PlayerButton";
            this.playWith2PlayerButton.Size = new System.Drawing.Size(165, 23);
            this.playWith2PlayerButton.TabIndex = 11;
            this.playWith2PlayerButton.Text = "Play Two Player";
            this.playWith2PlayerButton.UseVisualStyleBackColor = true;
            this.playWith2PlayerButton.Click += new System.EventHandler(this.playWith2PlayerButton_Click);
            // 
            // generateInputDataButton
            // 
            this.generateInputDataButton.Location = new System.Drawing.Point(12, 151);
            this.generateInputDataButton.Name = "generateInputDataButton";
            this.generateInputDataButton.Size = new System.Drawing.Size(165, 23);
            this.generateInputDataButton.TabIndex = 12;
            this.generateInputDataButton.Text = "Generate Input Training Data";
            this.generateInputDataButton.UseVisualStyleBackColor = true;
            this.generateInputDataButton.Click += new System.EventHandler(this.generateInputDataButton_Click);
            // 
            // trainNeuralNetButton
            // 
            this.trainNeuralNetButton.Location = new System.Drawing.Point(12, 209);
            this.trainNeuralNetButton.Name = "trainNeuralNetButton";
            this.trainNeuralNetButton.Size = new System.Drawing.Size(165, 23);
            this.trainNeuralNetButton.TabIndex = 13;
            this.trainNeuralNetButton.Text = "Train Neural Net";
            this.trainNeuralNetButton.UseVisualStyleBackColor = true;
            this.trainNeuralNetButton.Click += new System.EventHandler(this.trainNeuralNetButton_Click);
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
            this.openFileDialog2.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog2_FileOk);
            // 
            // generateOutputDataButton
            // 
            this.generateOutputDataButton.Location = new System.Drawing.Point(12, 180);
            this.generateOutputDataButton.Name = "generateOutputDataButton";
            this.generateOutputDataButton.Size = new System.Drawing.Size(165, 23);
            this.generateOutputDataButton.TabIndex = 14;
            this.generateOutputDataButton.Text = "Generate Ouput Training Data";
            this.generateOutputDataButton.UseVisualStyleBackColor = true;
            this.generateOutputDataButton.Click += new System.EventHandler(this.generateOutputDataButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 280);
            this.Controls.Add(this.generateOutputDataButton);
            this.Controls.Add(this.trainNeuralNetButton);
            this.Controls.Add(this.generateInputDataButton);
            this.Controls.Add(this.playWith2PlayerButton);
            this.Controls.Add(this.playWithNetButton);
            this.Controls.Add(this.openTrainingDataButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trainingDataTextBox);
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
        private System.Windows.Forms.Button openTrainingDataButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox trainingDataTextBox;
        private System.Windows.Forms.Button playWithNetButton;
        private System.Windows.Forms.Button playWith2PlayerButton;
        private System.Windows.Forms.Button generateInputDataButton;
        private System.Windows.Forms.Button trainNeuralNetButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button generateOutputDataButton;
    }
}

