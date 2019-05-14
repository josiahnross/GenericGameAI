﻿using NeuralNetTreeStuffViewer.MinMaxAlg;
using NeuralNetTreeStuffViewer.NeuralNet;
using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public static List<ChooseMoveEvaluators> EvaluatorNames;
        public static Dictionary<string, (ITurnBasedGame game, int[] layers, Type interfaceType, Type trainerType, Type mctEvalType)> namesOfGames;
        public Form1()
        {
            InitializeComponent();
            ActivationFunction.Init();
            if (EvaluatorNames == null)
            {
                EvaluatorNames = new List<ChooseMoveEvaluators>();
                foreach (var value in Enum.GetValues(typeof(ChooseMoveEvaluators)))
                {
                    EvaluatorNames.Add((ChooseMoveEvaluators)value);
                }
            }
        }
        MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition> ticTacToeEvaluator;
        MinMaxTurnBasedGameInterface<ConnectFour, int> connectFourEvaluator;
        MinMaxTurnBasedGameInterface<Checkers, CheckersMove> checkersEvaluator;

        string debugInfoPath = "debugInfo.txt";
        string netPath = "minMaxGen3Net.net";
        string policyNetPath = "policyGen0Net.txt";
        bool AIFirst = false;
        NeuralNetwork checkersNet;

        public double NeuralNetEval(ITurnBasedGame<Checkers, CheckersMove> game, Players player)
        {
            double[] input = game.GetInputs(player);
            return checkersNet.Compute(input)[0];
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            Funcs.Random = new Random(4);
            NavigationInfo.FormOrder.Push(this);
            if (namesOfGames == null)
            {
                namesOfGames = new Dictionary<string, (ITurnBasedGame game, int[] layers, Type interfaceType, Type trainerType, Type mctEvalType)>();
                namesOfGames.Add("Tick Tac Toe", (new TickTacToe(), new int[] { 19, 5, 3, 1 },
                    typeof(MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition>), typeof(NeuralNetGameTrainer<TickTacToe, BoardPosition>),
                    typeof(MonteCarloEvaluator<TickTacToe, BoardPosition>)));
                namesOfGames.Add("Connect Four", (new ConnectFour(), new int[] { 85, 50, 25, 10, 1 },
                    typeof(MinMaxTurnBasedGameInterface<ConnectFour, int>), typeof(NeuralNetGameTrainer<ConnectFour, int>),
                    typeof(MonteCarloEvaluator<ConnectFour, int>)));
                namesOfGames.Add("Checkers", (new Checkers(), new int[] { 97, 60, 35, 20, 10, 5, 1 },
                    typeof(MinMaxTurnBasedGameInterface<Checkers, CheckersMove>), typeof(NeuralNetGameTrainer<Checkers, CheckersMove>),
                    typeof(MonteCarloEvaluator<Checkers, CheckersMove>)));
                namesOfGames.Add("Chess", (new Chess(), new int[] { 321, 250, 100, 50, 30, 20, 10, 1 },
                    typeof(MinMaxTurnBasedGameInterface<Chess, ChessMove>), typeof(NeuralNetGameTrainer<Chess, ChessMove>),
                    typeof(MonteCarloEvaluator<Chess, ChessMove>)));
            }
            foreach (var g in namesOfGames)
            {
                gamesComboBox.Items.Add(g.Key);
            }
            gamesComboBox.SelectedIndex = 0;
            //Chess chess = new Chess();
            if (false)
            {
                TickTacToe tickTacToe = new TickTacToe(3);
                //tickTacToe.DisplayGame(gamePanel);
                ticTacToeEvaluator = new MinMaxTurnBasedGameInterface<TickTacToe, BoardPosition>(new TickTacToe(3), new TickTacToe(3), AIFirst, 9, debugInfoPath);
                ticTacToeEvaluator.SetEvaluator(new MonteCarloEvaluator<TickTacToe, BoardPosition>(MonteCarloTree.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100, ticTacToeEvaluator.Game, int.MaxValue, false));
            }
            else if (false)
            {
                ConnectFour connectFour = new ConnectFour(7, 6);
                //connectFour.DisplayGame(gamePanel);
                connectFourEvaluator = new MinMaxTurnBasedGameInterface<ConnectFour, int>(new ConnectFour(7, 6), connectFour, AIFirst, 3, debugInfoPath);
                connectFourEvaluator.SetEvaluator(new MonteCarloEvaluator<ConnectFour, int>(MonteCarloTree.UTCSelection, Math.Sqrt(2), RandomMoveSelectionFunc, 50, 100, connectFourEvaluator.Game, int.MaxValue, false));
            }
            else if (false)
            {
                Checkers checkers = new Checkers();
                //checkers.DisplayGame(gamePanel);
                checkers.GetInputs(Players.YouOrFirst);
                IEvaluateableTurnBasedGame<Checkers, CheckersMove> eval = null;
                uint minMaxDepth;
                bool monteCarloEval = true;
                if (monteCarloEval)
                {
                    minMaxDepth = 3;
                }
                else
                {
                    minMaxDepth = 6;
                }
                if (true)
                {
                    checkersEvaluator = new MinMaxTurnBasedGameInterface<Checkers, CheckersMove>(new Checkers(), checkers,
                        AIFirst, minMaxDepth, debugInfoPath);
                }
                if (monteCarloEval)
                {
                    eval = new MonteCarloEvaluator<Checkers, CheckersMove>(MonteCarloTree.UTCSelection,
                    Math.Sqrt(2), RandomMoveSelectionFunc, 50, 25, checkersEvaluator.Game, int.MaxValue, false);
                }
                else
                {
                    eval = new JustEvaluator<Checkers, CheckersMove>((g, p) => (g.Game.AmountOfFirstPlayerCheckers - g.Game.AmountOfSecondPlayerCheckers + ((g.Game.AmountOfFirstPlayerKings - g.Game.AmountOfSecondPlayerKings) * 1.5f)), checkersEvaluator);
                }
                checkersEvaluator.SetEvaluator(eval);
            }
            else if (false)
            {
                int maxDepth = 100;

                string path = "inputOutputsMinMax4.txt";
                NeuralNetwork net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 97, 60, 35, 20, 10, 5, 1);
                Backpropagation backProp = new Backpropagation(net);
                NeuralNetwork policyNet = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 97, 100, 150, 200, Checkers.StatocTotalAmountOfMoves);
                Backpropagation policyBackProp = new Backpropagation(policyNet, PolicyError);
                NeuralNetGameTrainer<Checkers, CheckersMove> trainer = new NeuralNetGameTrainer<Checkers, CheckersMove>(backProp, policyBackProp, path);

                var monteCarloEvaluator = new MonteCarloEvaluator<Checkers, CheckersMove>(MonteCarloTree.UTCSelection,
                 Math.Sqrt(2), trainer.NetChooseMoveWithValue, 0, 8, new Checkers(), maxDepth, true);//trainer.NetChooseMove
                MinMaxEvaluator<Checkers, CheckersMove> minMaxEvaluator = new MinMaxEvaluator<Checkers, CheckersMove>(new Checkers(), null, 5, null);
                IEvaluateableTurnBasedGame<Checkers, CheckersMove> eval = new JustEvaluator<Checkers, CheckersMove>(trainer.NeuralNetEval, minMaxEvaluator);
                minMaxEvaluator.Evaluator = eval;
                double maxOut = 1;
                //if (false)
                //{
                trainer.LoadNeuralNet(netPath);
                trainer.GetTrainingInputs(new Checkers(), 50, maxDepth, ChooseMoveEvaluators.WeightedNeualNet, false);
                trainer.GetTrainingOutputs(minMaxEvaluator, 40, path);
                trainer.PruneInputOutputs(maxOut);
                //trainer.GetPolicyOutputs(0, path, debugPath);
                trainer.StoreInputOutputs(path);
                //}
                //else if (true)
                //{
                //trainer.LoadNeuralNet(netPath);
                trainer.TrainNeuralNet(10000, .0000005f, 0.03f, 0, 1, 0, maxOut, true, false, netPath, 0.8f);
                //}
                if (false)
                {
                    trainer.LoadPolicyNeualNet(policyNetPath);
                    trainer.TrainNeuralNet(25, .001f, 0.06, 0, 1, 0, 1, false, true, policyNetPath, 1);
                }
            }
            else if (false)
            {
                int maxDepth = 100;

                string netPath = "netPgen0";
                string path = "inputOutputsP.txt";
                string debugPath = "debugInputOutputsP.txt";
                Checkers temp = new Checkers();
                NeuralNetwork net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 97, 150, 100, 200, temp.TotalAmountOfMoves + 1);
                Backpropagation backProp = new Backpropagation(net, FirstIndexAverageError);
                NeuralNetGameTrainer<Checkers, CheckersMove> trainer = new NeuralNetGameTrainer<Checkers, CheckersMove>(backProp, null, path);

                if (false)
                {
                    trainer.GetTrainingInputs(new Checkers(), 50, maxDepth, ChooseMoveEvaluators.WeightedNeualNet, false);
                    trainer.GetValuePolicyTrainingOutputs(maxDepth, new Checkers(), false, 50, path, debugPath);
                    trainer.PruneInputOutputs(50);
                    trainer.StoreInputOutputs(path);
                }
                else
                {
                    //trainer.LoadNeuralNet(netPath);
                    trainer.TrainNeuralNet(100, .0001f, 0.06, 0, 1, 0, 50, true, false, netPath, 1);
                }
            }
            else if (false)
            {
                double[][] inputs = new double[100][];
                double[][] outputs = new double[100][];
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = new double[] { Funcs.Random.NextDouble(0, 1), Funcs.Random.NextDouble(0, 1) };
                    outputs[i] = new double[] { (Extensions.Distance(0.5, 0.5, inputs[i][0], inputs[i][1]) < .25) ? 1 : 0 };
                }
                NeuralNetwork net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 2, 5, 5, 1);
                Backpropagation backProp = new Backpropagation(net);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Reset();
                stopwatch.Start();
                while (true)
                {
                    Console.SetCursorPosition(0, 0);
                    /*
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        string line = "(";
                        for (int j = 0; j < inputs[i].Length; j++)
                        {
                            if (j != 0)
                            {
                                line += ", ";
                            }
                            line += inputs[i][j];
                        }
                        line += "): (";
                        double[] netOut = net.Compute(inputs[i]);
                        for (int j = 0; j < netOut.Length; j++)
                        {
                            if (j != 0)
                            {
                                line += ", ";
                            }
                            line += Math.Round(netOut[j], 4);
                        }
                        line += ")";
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write(line);
                        Console.WriteLine("                     ");
                    }
                    */
                    double error = 0;
                    int batchSize = inputs.Length;
                    for (int i = 0; i < inputs.Length; i += batchSize)
                    {
                        var errorI = backProp.TrainBatch(inputs, outputs, .0005f, i, batchSize);
                        error += errorI.Total;
                    }
                    double averageError = error / inputs.Length;
                    Console.WriteLine(Math.Round(averageError, 10));
                    Console.WriteLine();

                    if (averageError <= 0.05 || double.IsNaN(averageError) || double.IsPositiveInfinity(averageError) || double.IsNegativeInfinity(averageError))
                    {
                        break;
                    }
                }
                stopwatch.Stop();
                Console.Clear();
                Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000f);
            }
            else if (false)
            {
                checkersNet = NeuralNetwork.Deserialize(File.ReadAllText(netPath));
                Checkers checkers = new Checkers();
                //checkers.DisplayGame(gamePanel);
                checkers.GetInputs(Players.YouOrFirst);
                uint minMaxDepth = 6;
                checkersEvaluator = new MinMaxTurnBasedGameInterface<Checkers, CheckersMove>(new Checkers(), checkers,
                    AIFirst, minMaxDepth, debugInfoPath);

                IEvaluateableTurnBasedGame<Checkers, CheckersMove> eval = new JustEvaluator<Checkers, CheckersMove>(NeuralNetEval, checkersEvaluator);
                checkersEvaluator.SetEvaluator(eval);
            }
            else if (false)
            {
                Chess chess = new Chess();
                //chess.DisplayGame(gamePanel);
                int maxDepth = 100;

                netPath = "chessGen2NetV2.net";
                string path = "chessInputOutputsMinMax2V2.txt";
                NeuralNetwork net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 321, 250, 100, 50, 30, 20, 10, 1);
                Backpropagation backProp = new Backpropagation(net);
                var chessTrainer = new NeuralNetGameTrainer<Chess, ChessMove>(backProp, null, path);
                //iTrainer = chessTrainer;

                var monteCarloEvaluator = new MonteCarloEvaluator<Chess, ChessMove>(MonteCarloTree.UTCSelection,
                 Math.Sqrt(2), chessTrainer.NetChooseMoveWithValue, 0, 8, new Chess(), maxDepth, true, .0002f);
                //MinMaxEvaluator<Chess, ChessMove> minMaxEvaluator = new MinMaxEvaluator<Chess, ChessMove>(new Chess(), null, 0, null);
                //IEvaluateableTurnBasedGame<Chess, ChessMove> eval = new JustEvaluator<Chess, ChessMove>(trainer.NeuralNetEval, minMaxEvaluator);
                //minMaxEvaluator.Evaluator = monteCarloEvaluator;
                double maxOut = 50;

                chessTrainer.LoadNeuralNet("chessGen1NetV2.net");
                await Task.Run(() =>
                {
                    chessTrainer.GetTrainingInputs(new Chess(), 25, maxDepth, ChooseMoveEvaluators.WeightedNeualNet, false);
                    chessTrainer.GetTrainingOutputs(monteCarloEvaluator, 1, path);
                    return;
                    chessTrainer.PruneInputOutputs(maxOut);
                    chessTrainer.StoreInputOutputs(path);

                    //put prune in train neural net
                    chessTrainer.TrainNeuralNet(10000, .0002f, 0.01f, 0, 1, 0, maxOut, true, false, netPath, 0.8f, 3, .5f);

                });
            }
            else if (false)
            {
                netPath = "chessGen1NetV2.txt";
                NeuralNetwork net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, 321, 250, 100, 50, 30, 20, 10, 1);
                Backpropagation backProp = new Backpropagation(net);
                NeuralNetGameTrainer<Chess, ChessMove> trainer = new NeuralNetGameTrainer<Chess, ChessMove>(backProp);
                trainer.LoadNeuralNet(netPath);

                Chess chess = new Chess();
                //chess.DisplayGame(gamePanel);
                uint minMaxDepth = 2;
                var chessEvaluator = new MinMaxTurnBasedGameInterface<Chess, ChessMove>(new Chess(), chess,
                    AIFirst, minMaxDepth, debugInfoPath);

                IEvaluateableTurnBasedGame<Chess, ChessMove> eval = new JustEvaluator<Chess, ChessMove>(trainer.NeuralNetEval, chessEvaluator);
                chessEvaluator.SetEvaluator(eval);
            }
        }


        public (int key, ITurnBasedGame<TickTacToe, BoardPosition> newBoardState) RandomMoveSelectionFunc(ITurnBasedGame<TickTacToe, BoardPosition> game, Dictionary<int, BoardPosition> avaialableMoves, Players player)
        {
            return (avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key, null);//TO DO: make more efficent
        }
        public (int key, ITurnBasedGame<ConnectFour, int> newBoardState) RandomMoveSelectionFunc(ITurnBasedGame<ConnectFour, int> game, Dictionary<int, int> avaialableMoves, Players player)
        {
            return (avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key, null);//TO DO: make more efficent
        }
        public (int key, ITurnBasedGame<Checkers, CheckersMove> newBoardState) RandomMoveSelectionFunc(ITurnBasedGame<Checkers, CheckersMove> game, Dictionary<int, CheckersMove> avaialableMoves, Players player)
        {
            return (avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key, null);//TO DO: make more efficent
        }

        double RandomFunc()
        {
            return Funcs.Random.Next(0, 100);
        }
        int counter = 0;
        double CounterFunc()
        {
            return counter++;
        }

        public static ErrorInfo FirstIndexAverageError(NeuralNetwork net, double[][] inputs, double[][] desiredOutputs, double[][] outputs, int startIndex, int count)
        {
            double error = 0;
            for (int i = startIndex; i < count + startIndex && i < inputs.Length; i++)
            {
                double[] output = outputs[i];
                error += Math.Abs(desiredOutputs[i][0] - output[0]);
            }
            return new ErrorInfo(error / Math.Min(count, inputs.Length - startIndex), error);
        }

        public static ErrorInfo PolicyError(NeuralNetwork net, double[][] inputs, double[][] desiredOutputs, double[][] outputs, int startIndex, int count)
        {
            double error = 0;
            for (int i = startIndex; i < count + startIndex && i < inputs.Length; i++)
            {
                Players p = Players.YouOrFirst;
                if (inputs[i][inputs[i].Length - 1] == 0)
                {
                    p = Players.OpponentOrSecond;
                }
                double[] output = outputs[i];
                double thisError = 0;
                int errorCount = 0;
                for (int j = 0; j < desiredOutputs[i].Length; j++)
                {
                    if ((p == Players.YouOrFirst && desiredOutputs[i][j] != -1) || (p == Players.OpponentOrSecond && desiredOutputs[i][j] != 1))
                    {
                        thisError += Math.Abs(desiredOutputs[i][j] - output[j]);
                        errorCount++;
                    }
                }
                error += thisError / errorCount;
            }
            return new ErrorInfo(error / Math.Min(count, inputs.Length - startIndex), error);
        }

        public static bool BetterVal(double current, double newVal, Players p)
        {
            if (p == Players.YouOrFirst)
            {
                return current < newVal;
            }
            return current > newVal;
        }

        private void gamesComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!namesOfGames.ContainsKey(gamesComboBox.Text))
            {
                gamesComboBox.Text = gamesComboBox.Items[0].ToString();
                gamesComboBox.SelectedIndex = 0;
            }
        }

        private void playWithNetButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.Game = namesOfGames[gamesComboBox.Text].game;
            NavigationInfo.NameOfGame = gamesComboBox.Text;
            NavigationInfo.NextForm = new GameForm();
            if (SetNavigationInfo())
            {

                NavigationInfo.InterfaceWithGenericsType = namesOfGames[gamesComboBox.Text].interfaceType;
                GameSettingsForm m = new GameSettingsForm();
                m.Show();
                Hide();
            }
        }

        private void playWith2PlayerButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.Game = namesOfGames[gamesComboBox.Text].game;
            NavigationInfo.NameOfGame = gamesComboBox.Text;
            NavigationInfo.Net = null;
            GameForm m = new GameForm();
            m.Show();
            Hide();
        }

        private void openNetFileButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string file = openFileDialog1.FileName;
            neuralNetTextBox.Text = file;
            NeuralNetwork net = null;
            try
            {
                net = NeuralNetwork.Deserialize(File.ReadAllText(neuralNetTextBox.Text));
            }
            catch
            {
                net = null;
                neuralNetTextBox.Text = "";
            }
            NavigationInfo.Net = net;
        }

        private void neuralNetTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void opneTrainingDataButtonClick_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            string file = openFileDialog2.FileName;
            trainingDataTextBox.Text = file;
        }

        private void generateInputDataButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.Game = namesOfGames[gamesComboBox.Text].game;
            NavigationInfo.NameOfGame = gamesComboBox.Text;
            if (SetNavigationInfo())
            {
                if (SetTrainer())
                {
                    TrainingInputsSettingsForm m = new TrainingInputsSettingsForm();
                    m.Show();
                    Hide();
                }
            }
        }

        bool SetNavigationInfo()
        {
            if (NavigationInfo.Net == null)
            {
                if (neuralNetTextBox.Text == "")
                {
                    NavigationInfo.Net = new NeuralNetwork(new TanH(-1, 1), Funcs.Random.NextDouble, namesOfGames[gamesComboBox.Text].layers);
                    NavigationInfo.NetPath = null;
                }
                else
                {
                    NeuralNetwork net = null;
                    try
                    {
                        net = NeuralNetwork.Deserialize(File.ReadAllText(neuralNetTextBox.Text));
                    }
                    catch
                    {
                        NavigationInfo.Net = null;
                        playWithNetButton.Enabled = false;
                        neuralNetTextBox.Text = "";
                        return false;
                    }
                    NavigationInfo.Net = net;
                    NavigationInfo.NetPath = neuralNetTextBox.Text;
                }
            }
            if (NavigationInfo.Net.Layers[0].Neurons.Length != NavigationInfo.Game.GetInputs(Players.YouOrFirst).Length)
            {
                NavigationInfo.NetPath = null;
                NavigationInfo.Net = null;
                neuralNetTextBox.Text = "";
                return false;
            }
            return true;
        }

        bool SetTrainer()
        {
            ITrainer trainer =
                        (ITrainer)namesOfGames[gamesComboBox.Text].trainerType.GetConstructor(
                            new Type[] { typeof(Backpropagation), typeof(Backpropagation), typeof(string) })
                            .Invoke(new object[] { new Backpropagation(NavigationInfo.Net), null, trainingDataTextBox.Text == "" ? null : trainingDataTextBox.Text });
            int inputAmount = namesOfGames[gamesComboBox.Text].layers[0];
            if ((trainer.LoadedFromFile || trainingDataTextBox.Text == "") && (trainer.InputOutputCount == 0 || trainer.GetNetInputs() == inputAmount))
            {
                NavigationInfo.Trainer = trainer;
                NavigationInfo.TrainingDataPath = trainingDataTextBox.Text == "" ? null : trainingDataTextBox.Text;
                return true;
            }
            else
            {
                NavigationInfo.TrainingDataPath = null;
                NavigationInfo.Trainer = null;
                trainingDataTextBox.Text = "";
                return false;
            }
        }

        private void generateOutputDataButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.Game = namesOfGames[gamesComboBox.Text].game;
            NavigationInfo.NameOfGame = gamesComboBox.Text;
            if (SetNavigationInfo())
            {
                if (SetTrainer())
                {
                    TrainingOutputSettingsForm m = new TrainingOutputSettingsForm();
                    m.Show();
                    Hide();
                }
            }
        }

        private void trainNeuralNetButton_Click(object sender, EventArgs e)
        {
            NavigationInfo.Game = namesOfGames[gamesComboBox.Text].game;
            NavigationInfo.NameOfGame = gamesComboBox.Text;
            if (SetNavigationInfo())
            {
                if (SetTrainer())
                {
                    TrainNeuralNetForm m = new TrainNeuralNetForm();
                    m.Show();
                    Hide();
                }
            }
        }
    }
    public static class Funcs
    {
        public static Random Random { get; set; }
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
        public static Players OppositePlayer(Players player)
        {
            if (player == Players.YouOrFirst)
            {
                return Players.OpponentOrSecond;
            }
            else if (player == Players.OpponentOrSecond)
            {
                return Players.YouOrFirst;
            }
            return Players.None;
        }
        public static Players GetPlayerFromBool(bool player)
        {
            return player ? Players.YouOrFirst : Players.OpponentOrSecond;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }
}
