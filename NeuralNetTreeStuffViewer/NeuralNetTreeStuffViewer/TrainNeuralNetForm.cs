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
    public partial class TrainNeuralNetForm : Form
    {
        public TrainNeuralNetForm()
        {
            InitializeComponent();
        }

        private void TrainNeuralNet_Load(object sender, EventArgs e)
        {
            string netPath = "chessGen2Net.net";
            int maxOut = 20;
            NavigationInfo.Trainer.PruneInputOutputs(maxOut);
            NavigationInfo.Trainer.StoreInputOutputs(NavigationInfo.TrainingDataPath);
            NavigationInfo.Trainer.TrainNeuralNet(10000, .0002f, 0.01f, 0, 1, 0, maxOut, true, false, netPath, 0.8f, 3, .5f);
        }
    }
}
