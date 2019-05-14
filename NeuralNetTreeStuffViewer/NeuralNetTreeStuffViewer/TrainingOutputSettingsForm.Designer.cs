namespace NeuralNetTreeStuffViewer
{
    partial class TrainingOutputSettingsForm
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
            this.evaluatorComboBox = new System.Windows.Forms.ComboBox();
            this.depthNumeric = new System.Windows.Forms.NumericUpDown();
            this.simulationNumeric = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.parallelBatchNumeric = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.writeRateNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.depthWeightTextBox = new System.Windows.Forms.TextBox();
            this.continueButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.depthNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simulationNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.parallelBatchNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.writeRateNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // evaluatorComboBox
            // 
            this.evaluatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.evaluatorComboBox.FormattingEnabled = true;
            this.evaluatorComboBox.Location = new System.Drawing.Point(102, 31);
            this.evaluatorComboBox.Name = "evaluatorComboBox";
            this.evaluatorComboBox.Size = new System.Drawing.Size(150, 21);
            this.evaluatorComboBox.TabIndex = 12;
            // 
            // depthNumeric
            // 
            this.depthNumeric.Location = new System.Drawing.Point(106, 59);
            this.depthNumeric.Name = "depthNumeric";
            this.depthNumeric.Size = new System.Drawing.Size(146, 20);
            this.depthNumeric.TabIndex = 11;
            this.depthNumeric.ValueChanged += new System.EventHandler(this.depthNumeric_ValueChanged);
            // 
            // simulationNumeric
            // 
            this.simulationNumeric.Location = new System.Drawing.Point(132, 7);
            this.simulationNumeric.Name = "simulationNumeric";
            this.simulationNumeric.Size = new System.Drawing.Size(120, 20);
            this.simulationNumeric.TabIndex = 10;
            this.simulationNumeric.ValueChanged += new System.EventHandler(this.simulationNumeric_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Max MCT Depth:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Move Evaluator:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Amount of Simulations:";
            // 
            // parallelBatchNumeric
            // 
            this.parallelBatchNumeric.Location = new System.Drawing.Point(132, 87);
            this.parallelBatchNumeric.Name = "parallelBatchNumeric";
            this.parallelBatchNumeric.Size = new System.Drawing.Size(120, 20);
            this.parallelBatchNumeric.TabIndex = 14;
            this.parallelBatchNumeric.ValueChanged += new System.EventHandler(this.parallelBatchNumeric_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Parallel Batch Amount:";
            // 
            // writeRateNumeric
            // 
            this.writeRateNumeric.Location = new System.Drawing.Point(132, 113);
            this.writeRateNumeric.Name = "writeRateNumeric";
            this.writeRateNumeric.Size = new System.Drawing.Size(120, 20);
            this.writeRateNumeric.TabIndex = 16;
            this.writeRateNumeric.ValueChanged += new System.EventHandler(this.writeRateNumeric_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Write Remaining Rate:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Depth Weight:";
            // 
            // depthWeightTextBox
            // 
            this.depthWeightTextBox.Location = new System.Drawing.Point(94, 138);
            this.depthWeightTextBox.Name = "depthWeightTextBox";
            this.depthWeightTextBox.Size = new System.Drawing.Size(157, 20);
            this.depthWeightTextBox.TabIndex = 18;
            this.depthWeightTextBox.TextChanged += new System.EventHandler(this.depthWeightTextBox_TextChanged);
            // 
            // continueButton
            // 
            this.continueButton.Location = new System.Drawing.Point(12, 167);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(239, 23);
            this.continueButton.TabIndex = 19;
            this.continueButton.Text = "Continue";
            this.continueButton.UseVisualStyleBackColor = true;
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // TrainingOutputSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 201);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.depthWeightTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.writeRateNumeric);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.parallelBatchNumeric);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.evaluatorComboBox);
            this.Controls.Add(this.depthNumeric);
            this.Controls.Add(this.simulationNumeric);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TrainingOutputSettingsForm";
            this.Text = "TrainingOutputSettingsForm";
            this.Load += new System.EventHandler(this.TrainingOutputSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.depthNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simulationNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.parallelBatchNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.writeRateNumeric)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox evaluatorComboBox;
        private System.Windows.Forms.NumericUpDown depthNumeric;
        private System.Windows.Forms.NumericUpDown simulationNumeric;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown parallelBatchNumeric;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown writeRateNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox depthWeightTextBox;
        private System.Windows.Forms.Button continueButton;
    }
}