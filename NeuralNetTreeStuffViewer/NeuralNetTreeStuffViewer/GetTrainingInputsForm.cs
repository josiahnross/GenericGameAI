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
    public partial class GetTrainingInputsForm : Form
    {
        public GetTrainingInputsForm()
        {
            InitializeComponent();
        }

        private void GetTrainingInputsForm_Load(object sender, EventArgs e)
        {
            NavigationInfo.FormOrder.Push(this);

            NavigationInfo.Trainer.GetTrainingInputs(NavigationInfo.Game.Copy(), NavigationInfo.AmountOfInputMCTSimulation, NavigationInfo.InputMCTMaxDepth, NavigationInfo.InputMCTMoveEvalutator, NavigationInfo.InputsRemoveDraws);

            NavigationInfo.Trainer.StoreInputOutputs(NavigationInfo.TrainingDataPath);
        }
    }
}
