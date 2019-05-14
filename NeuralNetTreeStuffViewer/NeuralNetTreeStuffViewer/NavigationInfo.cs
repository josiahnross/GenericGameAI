using NeuralNetTreeStuffViewer.NeuralNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public static class NavigationInfo
    {
        public static Type InterfaceWithGenericsType = null;
        public static ITurnBasedGame Game = null;
        public static Form NextForm = null;
        public static Stack<Form> FormOrder = new Stack<Form>();
        public static bool UserPlaysFirst = true;
        public static NeuralNetwork Net = null;
        public static string DebugInfoPath = "debugInfo.txt";
        public static uint MinMaxDepth = 3;
        public static ITrainer Trainer = null;
        public static int AmountOfInputMCTSimulation = 30;
        public static ChooseMoveEvaluators InputMCTMoveEvalutator = ChooseMoveEvaluators.WeightedNeualNet;
        public static bool InputsRemoveDraws = false;
        public static int InputMCTMaxDepth = 100;
        public static int AmountOfMCTSimulation = 8;
        public static ChooseMoveEvaluators OutputMCTMoveEvalutator = ChooseMoveEvaluators.NeuarlNet;
        public static double DepthWeight = 0.002f;
        public static int OutputMCTMaxDepth = 100;
        public static int WriteRemainingDataRate = 1;
        public static int ParrallelAmount = 100;
        public static string NameOfGame = null;
        public static string NetPath = null;
        public static string TrainingDataPath = null;
    }
}
