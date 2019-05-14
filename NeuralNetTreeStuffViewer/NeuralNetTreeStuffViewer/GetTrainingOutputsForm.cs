using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public partial class GetTrainingOutputsForm : Form
    {
        ITrainer trainer;
        public GetTrainingOutputsForm()
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
            ITurnBasedGame game = NavigationInfo.Game.Copy();
            Type mctEval = Form1.namesOfGames[NavigationInfo.NameOfGame].mctEvalType;
            var constructors = mctEval.GetConstructors();
            System.Reflection.ConstructorInfo constructor = null;
            foreach (var c in constructors)
            {
                if (c.GetParameters()[0].ParameterType == typeof(MonteCarloTree))
                {
                    constructor = c;
                    break;
                }
            }
            if (constructor != null)
            {
                var getChooseMoveFunc = mctEval.GetMethod("GetChooseMoveFunc", BindingFlags.Public | BindingFlags.Static);
                var chooseMoveFunc = getChooseMoveFunc.Invoke(null, new object[] { NavigationInfo.OutputMCTMoveEvalutator, NavigationInfo.Trainer });
                var monteCarloEvaluator = constructor.Invoke(
                    new object[]
                    {
                        new MonteCarloTree(NavigationInfo.Game.Copy(), MonteCarloTree.UTCSelection, Math.Sqrt(2), NavigationInfo.OutputMCTMaxDepth, Players.YouOrFirst),
                        chooseMoveFunc, 0, NavigationInfo.AmountOfMCTSimulation, true, NavigationInfo.DepthWeight, Players.YouOrFirst
                    });
                trainer = NavigationInfo.Trainer;
                NavigationInfo.Trainer.GetTrainingOutputs((IEvaluateableTurnBasedGame)monteCarloEvaluator, NavigationInfo.WriteRemainingDataRate, NavigationInfo.TrainingDataPath, NavigationInfo.ParrallelAmount);
                
                NavigationInfo.Trainer.StoreInputOutputs(NavigationInfo.TrainingDataPath);
            }
        }
    }
}
