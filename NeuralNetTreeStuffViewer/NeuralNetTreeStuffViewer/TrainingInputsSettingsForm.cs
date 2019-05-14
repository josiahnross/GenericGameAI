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
    public partial class TrainingInputsSettingsForm : Form
    {
        public TrainingInputsSettingsForm()
        {
            InitializeComponent();
            evaluatorComboBox.Items.Clear();
            foreach (var item in Form1.EvaluatorNames)
            {
                evaluatorComboBox.Items.Add(item);
            }
            simulationNumeric.Value = NavigationInfo.AmountOfInputMCTSimulation;
            evaluatorComboBox.SelectedIndex = evaluatorComboBox.Items.IndexOf(NavigationInfo.InputMCTMoveEvalutator);
            depthNumeric.Value = NavigationInfo.InputMCTMaxDepth;
            removeDrawsCheck.Checked = NavigationInfo.InputsRemoveDraws;
            SetContinueEnabled();
        }

        private void simulationNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(simulationNumeric, NavigationInfo.AmountOfInputMCTSimulation);
        }

        private void depthNumeric_ValueChanged(object sender, EventArgs e)
        {
            PositiveNumeric(depthNumeric, NavigationInfo.InputMCTMaxDepth);
        }
        void PositiveNumeric(NumericUpDown numericUpDown, int newVal)
        {
            if (numericUpDown.Value <= 0)
            {
                numericUpDown.Value = newVal;
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.InputMCTMaxDepth = (int)depthNumeric.Value;
            NavigationInfo.InputsRemoveDraws = removeDrawsCheck.Checked;
            NavigationInfo.AmountOfInputMCTSimulation = (int)simulationNumeric.Value;
            NavigationInfo.InputMCTMoveEvalutator = (ChooseMoveEvaluators)evaluatorComboBox.Items[evaluatorComboBox.SelectedIndex];
            if (NavigationInfo.TrainingDataPath == null || NavigationInfo.TrainingDataPath == "")
            {
                NavigationInfo.TrainingDataPath = folderNameTextBox.Text + @"\" + fileNameTextBox.Text + ".train";
            }
            GetTrainingInputsForm m = new GetTrainingInputsForm();
            m.Show();
            Hide();
        }

        private void TrainingInputsSettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void getFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "")
            {
                folderNameTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
            SetContinueEnabled();
        }
        void SetContinueEnabled()
        {
            continueButton.Enabled = (NavigationInfo.TrainingDataPath != null && NavigationInfo.TrainingDataPath != "") || (folderNameTextBox.Text != "" && fileNameTextBox.Text != "");
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            SetContinueEnabled();
        }
    }
}
