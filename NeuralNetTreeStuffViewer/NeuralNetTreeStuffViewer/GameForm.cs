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
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }
        ITurnBasedGame displayGame;
        private void GameForm_Load(object sender, EventArgs e)
        {
            NavigationInfo.FormOrder.Push(this);
            Restart();
        }
        public static double NeuralNetEval(ITurnBasedGame game, Players player)
        {
            double[] input = game.GetInputs(player);
            return NavigationInfo.Net.Compute(input)[0];
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (NavigationInfo.FormOrder.Count > 1)
            {
                NavigationInfo.FormOrder.Pop();
                Form form = NavigationInfo.FormOrder.Peek();
                form.Show();
                Hide();
            }
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            Restart();
        }

        void Restart()
        {
            displayGame = NavigationInfo.Game.Copy();
            displayGame.DisplayGame(gamePanel);
            if (NavigationInfo.Net != null)
            {
                MinMaxTurnBasedGameInterface nonGenInterface = new MinMaxTurnBasedGameInterface(NavigationInfo.Game.Copy(), displayGame, !NavigationInfo.UserPlaysFirst, NavigationInfo.MinMaxDepth, NavigationInfo.DebugInfoPath);
                JustEvaluator evalutator = new JustEvaluator(NeuralNetEval, null);
                nonGenInterface.Evaluator = evalutator;
                var constructor = NavigationInfo.InterfaceWithGenericsType.GetConstructor(new Type[] { nonGenInterface.GetType() });
                object gameInterface = constructor.Invoke(new object[] { nonGenInterface });
            }
        }
    }
}
