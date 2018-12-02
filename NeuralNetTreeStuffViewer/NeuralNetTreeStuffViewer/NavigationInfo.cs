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
        public static int WriteRemainingDataRate = 1;
        public static int ParrallelAmount = 100;
        public static int AmountOfMCTSimulation = 8;
        public static int AmountOfInputMCTSimulation = 30;
        public static ChooseMoveEvaluators InputMCTMoveEvalutator = ChooseMoveEvaluators.WeightedNeualNet;
        public static ChooseMoveEvaluators OutputMCTMoveEvalutator = ChooseMoveEvaluators.NeuarlNet;
        public static bool InputsRemoveDraws = false;
        public static int MCTMaxDepth = 100;
        public static double DepthWieght = 0.002f;
    }
}
