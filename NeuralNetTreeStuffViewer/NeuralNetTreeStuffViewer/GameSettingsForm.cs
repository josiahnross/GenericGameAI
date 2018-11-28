using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public partial class GameSettingsForm : Form
    {
        public GameSettingsForm()
        {
            InitializeComponent();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.UserPlaysFirst = userPlaysFirstCheckBox.Checked;
            NavigationInfo.MinMaxDepth = (uint)numericUpDown1.Value;
            NavigationInfo.NextForm.Show();
            Hide();
        }

        private void GameSettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(numericUpDown1.Value <= 0)
            {
                numericUpDown1.Value = 1; 
            }
        }
    }
}
