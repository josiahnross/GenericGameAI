using NeuralNetTreeStuffViewer.MinMaxAlg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetTreeStuffViewer
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }
        Random random;
        MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition> ticTacToeEvaluator;
        MinMaxTurnBasedGameInterface<ConnectFour, int> connectFourEvaluator;
        MinMaxTurnBasedGameInterface<Checkers, CheckersMove> checkersEvaluator;


        string debugInfoPath = "debugInfo.txt";
        bool AIFirst = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            random = new Random(1);

            if (false)
            {
                TickTacToe tickTacToe = new TickTacToe(3);
                tickTacToe.DisplayGame(gamePanel);
                ticTacToeEvaluator = new MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition>(new TickTacToe(3), AIFirst,
                    new MonteCarloEvaluator<TickTacToe, BoardPosition>(MonteCarloTree<TickTacToe, BoardPosition>.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100),
                    9, debugInfoPath);
            }
            else if(false)
            {
                ConnectFour connectFour = new ConnectFour(7, 6);
                connectFour.DisplayGame(gamePanel);
                connectFourEvaluator = new MinMaxTurnBasedGameInterface<ConnectFour, int>(connectFour, AIFirst,
                    new MonteCarloEvaluator<ConnectFour, int>(MonteCarloTree<ConnectFour, int>.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100),
                    3, debugInfoPath);
            }
            else
            {
                Checkers checkers = new Checkers();
                checkers.DisplayGame(gamePanel);
                checkersEvaluator = new MinMaxTurnBasedGameInterface<Checkers, CheckersMove>(checkers, AIFirst,
                    new MonteCarloEvaluator<Checkers, CheckersMove>(MonteCarloTree<Checkers, CheckersMove>.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 25),
                    6, debugInfoPath);
            }
        }

        public int RandomMoveSelectionFunc(ITurnBasedGame<TickTacToe, BoardPosition> game, Dictionary<int, BoardPosition> avaialableMoves, Players player)
        {
            return avaialableMoves.ElementAt(random.Next(0, avaialableMoves.Count)).Key;//TO DO: make more efficent
        }
        public int RandomMoveSelectionFunc(ITurnBasedGame<ConnectFour, int> game, Dictionary<int, int> avaialableMoves, Players player)
        {
            return avaialableMoves.ElementAt(random.Next(0, avaialableMoves.Count)).Key;//TO DO: make more efficent
        }
        public int RandomMoveSelectionFunc(ITurnBasedGame<Checkers, CheckersMove> game, Dictionary<int, CheckersMove> avaialableMoves, Players player)
        {
            return avaialableMoves.ElementAt(random.Next(0, avaialableMoves.Count)).Key;//TO DO: make more efficent
        }

        double RandomFunc()
        {
            return random.Next(0, 100);
        }
        int counter = 0;
        double CounterFunc()
        {
            return counter++;
        }

        private void testButton_Click(object sender, EventArgs e)
        {
        }
        
        
    }
}
