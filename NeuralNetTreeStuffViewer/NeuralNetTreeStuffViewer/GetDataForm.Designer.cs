namespace NeuralNetTreeStuffViewer
{
    partial class GetDataForm
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
            this.runParallelCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // runParallelCheckBox
            // 
            this.runParallelCheckBox.AutoSize = true;
            this.runParallelCheckBox.Location = new System.Drawing.Point(12, 12);
            this.runParallelCheckBox.Name = "runParallelCheckBox";
            this.runParallelCheckBox.Size = new System.Drawing.Size(83, 17);
            this.runParallelCheckBox.TabIndex = 4;
            this.runParallelCheckBox.Text = "Run Parallel";
            this.runParallelCheckBox.UseVisualStyleBackColor = true;
            this.runParallelCheckBox.CheckedChanged += new System.EventHandler(this.runParallelCheckBox_CheckedChanged);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 36);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // GetDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 110);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.runParallelCheckBox);
            this.Name = "GetDataForm";
            this.Text = "GetDataForm";
            this.Load += new System.EventHandler(this.GetDataForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox runParallelCheckBox;
        private System.Windows.Forms.Button saveButton;
    }
}