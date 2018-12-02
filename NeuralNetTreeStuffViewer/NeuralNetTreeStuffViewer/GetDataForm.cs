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
    public partial class GetDataForm : Form
    {
        ITrainer trainer;
        public GetDataForm()
        {
            InitializeComponent();

            runParallelCheckBox.Checked = true;
            trainer = null;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

        }


        private void runParallelCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (trainer != null)
            {
                trainer.Parallel = runParallelCheckBox.Checked;
                trainer.Stop();
                while (trainer.CurrentlyParallel != trainer.Parallel)
                {

                }
            }
        }

        private void GetDataForm_Load(object sender, EventArgs e)
        {
            NavigationInfo.FormOrder.Push(this);
        }
    }
}
