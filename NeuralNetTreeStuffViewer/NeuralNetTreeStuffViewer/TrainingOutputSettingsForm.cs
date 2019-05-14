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
    public partial class TrainingOutputSettingsForm : Form
    {
        public TrainingOutputSettingsForm()
        {
            InitializeComponent();
            evaluatorComboBox.Items.Clear();
            foreach (var item in Form1.EvaluatorNames)
            {
                evaluatorComboBox.Items.Add(item);
            }
            simulationNumeric.Value = NavigationInfo.AmountOfMCTSimulation;
            evaluatorComboBox.SelectedIndex = evaluatorComboBox.Items.IndexOf(NavigationInfo.OutputMCTMoveEvalutator);
            depthNumeric.Value = NavigationInfo.OutputMCTMaxDepth;
            writeRateNumeric.Value = NavigationInfo.WriteRemainingDataRate;
            parallelBatchNumeric.Value = NavigationInfo.ParrallelAmount;
            depthWeightTextBox.Text = NavigationInfo.DepthWeight.ToString();
        }

        private void simulationNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(simulationNumeric, NavigationInfo.AmountOfMCTSimulation, 0);
        }

        private void depthNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(depthNumeric, NavigationInfo.OutputMCTMaxDepth, 0);
        }
        void PositiveNumeric(NumericUpDown numericUpDown, int newVal, int min)
        {
            if (numericUpDown.Value <= min)
            {
                numericUpDown.Value = newVal;
            }
        }

        private void writeRateNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(writeRateNumeric, NavigationInfo.WriteRemainingDataRate, -1);
        }

        private void parallelBatchNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(parallelBatchNumeric, NavigationInfo.ParrallelAmount, 0);
        }

        private void depthWeightTextBox_TextChanged(object sender, EventArgs e)
        {
            double value = 0;
            if (!double.TryParse(depthWeightTextBox.Text, out value))
            {
                depthWeightTextBox.Text = NavigationInfo.DepthWeight.ToString();
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.InputMCTMaxDepth = (int)depthNumeric.Value;
            NavigationInfo.AmountOfInputMCTSimulation = (int)simulationNumeric.Value;
            NavigationInfo.InputMCTMoveEvalutator = (ChooseMoveEvaluators)evaluatorComboBox.Items[evaluatorComboBox.SelectedIndex];
            NavigationInfo.WriteRemainingDataRate = (int)writeRateNumeric.Value;
            NavigationInfo.ParrallelAmount = (int)parallelBatchNumeric.Value;
            NavigationInfo.DepthWeight = double.Parse(depthWeightTextBox.Text);
            GetTrainingOutputsForm m = new GetTrainingOutputsForm();
            m.Show();
            Hide();
        }

        private void TrainingOutputSettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
