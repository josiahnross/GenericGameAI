using NeuralNetTreeStuffViewer.MinMaxAlg;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class NeuralNetGameTrainer<T, T1>
        where T : ITurnBasedGame<T, T1>
        where T1 : struct
    {
        List<InputOutputPair<T, T1>> inputOutputs;
        List<InputOutputDebugInfo> debugInputOutputs;
        public NeuralNetGameTrainer(string inputOutputPath = null, string inputOutputDebugPath = null)
        {
            if (inputOutputPath == null)
            {
                inputOutputs = new List<InputOutputPair<T, T1>>();
            }
            else
            {
                string data = File.ReadAllText(inputOutputPath);
                inputOutputs = JsonConvert.DeserializeObject<List<InputOutputPair<T, T1>>>(data);
                if(inputOutputs.Count > 0)
                {
                    inputOutputs[0].GameInputs.CurrentState.InitializeStaticVariables();
                }
            }
            if (inputOutputDebugPath == null)
            {
                debugInputOutputs = new List<InputOutputDebugInfo>();
            }
            else
            {
                debugInputOutputs = JsonConvert.DeserializeObject<List<InputOutputDebugInfo>>(File.ReadAllText(inputOutputDebugPath));
            }
        }
        public void StoreInputOutputs(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(inputOutputs));
        }
        public void StoreDebugInputOutputs(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(debugInputOutputs));
        }
        public void GetTrainingOutputs(IEvaluateableTurnBasedGame<T, T1> evaluator, int storeAmount = -1, string path = null, string debugPath = null)
        {
            int count = 0;
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                if (inputOutputs[i].Output == null)
                {
                    double v = evaluator.EvaluateCurrentState(inputOutputs[i].GameInputs.CurrentState, inputOutputs[i].GameInputs.Player);
                    inputOutputs[i] = new InputOutputPair<T, T1>(inputOutputs[i].GameInputs, v);
                    debugInputOutputs[i] = new InputOutputDebugInfo(debugInputOutputs[i].BoardInfo, v, debugInputOutputs[i].Player);
                    if (count > 0 && count % storeAmount == 0)
                    {
                        if (path != null)
                        {
                            StoreInputOutputs(path);
                        }
                        if (debugPath != null)
                        {
                            StoreDebugInputOutputs(debugPath);
                        }
                    }
                    count++;
                }
            }
        }

        public void GetTrainingInputs(ITurnBasedGame<T, T1> game, int amountOfSims, Players startPlayer = Players.YouOrFirst)
        {
            MonteCarloTree<T, T1> tree = new MonteCarloTree<T, T1>(game, MonteCarloTree<T, T1>.UTCSelection, Math.Sqrt(2), NetChooseMove, startPlayer);
            HashSet<MonteCarloNode<T, T1>> nodes = new HashSet<MonteCarloNode<T, T1>>();
            tree.RunMonteCarloSims(amountOfSims, tree.Root, nodes);
            foreach (var n in nodes)
            {
                var player = n.Player;
                var newInput = new InputOutputPair<T, T1>(new GameInputs<T, T1>(n.CurrentState.GetInputs(player), (T)n.CurrentState, player), null);
                inputOutputs.Add(newInput);
                debugInputOutputs.Add(new InputOutputDebugInfo(newInput.GameInputs.GetBoardInfo(), newInput.Output, newInput.GameInputs.Player));
            }
        }

        public static int NetChooseMove(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            return RandomChooseMove(game, avaialableMoves, player);
        }
        public static int RandomChooseMove(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            return avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key;
        }
    }
    public struct InputOutputPair<T, T1> where T : ITurnBasedGame<T, T1>
    {
        public GameInputs<T, T1> GameInputs { get; set; }
        public double? Output { get; set; }

        public InputOutputPair(GameInputs<T, T1> gameInputs, double? output)
        {
            GameInputs = gameInputs;
            Output = output;
        }
    }
    public struct GameInputs<T, T1> where T : ITurnBasedGame<T, T1>
    {
        public double[] Inputs { get; set; }
        public T CurrentState { get; set; }
        public Players Player { get; set; }
        public GameInputs(double[] inputs, T currentState, Players player)
        {
            Player = player;
            Inputs = inputs;
            CurrentState = currentState;
        }
        public BoardInfo GetBoardInfo()
        {
            return new BoardInfo(CurrentState.ToString(), CurrentState.GetType().Name);
        }
    }
    public struct InputOutputDebugInfo
    {
        public double? Output { get; set; }
        public BoardInfo BoardInfo { get; set; }
        public Players Player { get; set; }
        public InputOutputDebugInfo(BoardInfo boardInfo, double? output, Players player)
        {
            Output = output;
            BoardInfo = boardInfo;
            Player = player;
        }
    }
}
