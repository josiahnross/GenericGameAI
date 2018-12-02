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
    public partial class TrainingSettingsForm : Form
    {
        public TrainingSettingsForm()
        {
            InitializeComponent();
        }

        private void TrainingSettingsForm_Load(object sender, EventArgs e)
        {
            NavigationInfo.FormOrder.Push(this);
        }
    }
}
