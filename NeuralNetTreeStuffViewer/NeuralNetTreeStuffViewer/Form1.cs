using NeuralNetTreeStuffViewer.MinMaxAlg;
using Newtonsoft.Json;
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
                ticTacToeEvaluator = new MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition>(new TickTacToe(3), new TickTacToe(3), AIFirst,
                    new MonteCarloEvaluator<TickTacToe, BoardPosition>(MonteCarloTree<TickTacToe, BoardPosition>.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100),
                    9, debugInfoPath);
            }
            else if (false)
            {
                ConnectFour connectFour = new ConnectFour(7, 6);
                connectFour.DisplayGame(gamePanel);
                connectFourEvaluator = new MinMaxTurnBasedGameInterface<ConnectFour, int>(new ConnectFour(7, 6), connectFour, AIFirst,
                    new MonteCarloEvaluator<ConnectFour, int>(MonteCarloTree<ConnectFour, int>.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100),
                    3, debugInfoPath);
            }
            else
            {
                Checkers checkers = new Checkers();
                checkers.DisplayGame(gamePanel);
                checkers.GetInputs(Players.YouOrFirst);
                IEvaulator<Checkers, CheckersMove> eval;
                uint minMaxDepth;
                if(false)
                {
                    eval = new MonteCarloEvaluator<Checkers, CheckersMove>(MonteCarloTree<Checkers, CheckersMove>.UTCSelection,
                    Math.Sqrt(2), RandomMoveSelectionFunc, 50, 25);
                    minMaxDepth = 3;
                }
                else
                {
                    eval = new JustEvaluator<Checkers, CheckersMove>(g => (g.Game.AmountOfFirstPlayerCheckers - g.Game.AmountOfSecondPlayerCheckers + ((g.Game.AmountOfFirstPlayerKings - g.Game.AmountOfSecondPlayerKings)*1.5f)));
                    minMaxDepth = 6;
                }
                if (true)
                {
                    checkersEvaluator = new MinMaxTurnBasedGameInterface<Checkers, CheckersMove>(new Checkers(), checkers, 
                        AIFirst, eval, minMaxDepth, debugInfoPath);
                }
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
    public static class Funcs
    {
        public static void WriteBoard(object o, string path)
        {
            File.WriteAllText(path, GetBoardInfoJson(o));
        }
        public static string GetBoardInfoJson(object o)
        {
            return GetBoardInfoJson(GetBoardInfo(o));
        }
        public static string GetBoardInfoJson(BoardInfo info)
        {
            return JsonConvert.SerializeObject(info);
        }
        public static BoardInfo GetBoardInfo(object o)
        {
            return new BoardInfo(o.ToString(), o.GetType().Name);
        }
    }
}
