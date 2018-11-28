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
        public static bool UserPlaysFirst = true;
        public static NeuralNetwork Net = null;
        public static string DebugInfoPath = "debugInfo.txt";
        public static uint MinMaxDepth = 3;
    }
}
