using NeuralNetTreeStuffViewer.MinMaxAlg;
using NeuralNetTreeStuffViewer.NeuralNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Backpropagation backPropV;
        Backpropagation backPropPolicy;
        public NeuralNetGameTrainer(Backpropagation backProp, Backpropagation backPropPolicy = null, string inputOutputPath = null, string inputOutputDebugPath = null)
        {
            this.backPropV = backProp;
            this.backPropPolicy = backPropPolicy;
            if (inputOutputPath == null || !File.Exists(inputOutputPath))
            {
                inputOutputs = new List<InputOutputPair<T, T1>>();
            }
            else
            {
                string data = File.ReadAllText(inputOutputPath);
                inputOutputs = JsonConvert.DeserializeObject<List<InputOutputPair<T, T1>>>(data);
                if (inputOutputs.Count > 0)
                {
                    inputOutputs[0].GameInputs.CurrentState.InitializeStaticVariables();
                    for (int i = 0; i < inputOutputs.Count; i++)
                    {
                        inputOutputs[i].GameInputs.CurrentState.DeserializeInit();
                    }
                }
            }

            if (inputOutputDebugPath == null || !File.Exists(inputOutputDebugPath))
            {
                debugInputOutputs = new List<InputOutputDebugInfo>();
            }
            else
            {
                debugInputOutputs = JsonConvert.DeserializeObject<List<InputOutputDebugInfo>>(File.ReadAllText(inputOutputDebugPath));
            }
        }
        public void LoadNeuralNet(string path)
        {
            backPropV = new Backpropagation(NeuralNetwork.Deserialize(File.ReadAllText(path)), backPropV.ErrorFunc);
        }
        public void LoadPolicyNeualNet(string path)
        {
            backPropPolicy = new Backpropagation(NeuralNetwork.Deserialize(File.ReadAllText(path)), backPropPolicy.ErrorFunc);
        }
        public void TrainNeuralNet(int batchSize, double learningRate, double stopError, double minIn, double maxIn, double minOut, double maxOut, bool normilizeOnlyFirstOutput, bool policyNet, string netPath)
        {
            double[][] inputs = new double[inputOutputs.Count][];
            double[][] outputs = new double[inputs.Length][];
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] inputOutput;
                if(policyNet)
                {
                    inputOutput = inputOutputs[i].PolicyOutput;
                }
                else
                {
                    inputOutput = inputOutputs[i].Output;
                }
                if (inputOutput != null)
                {
                    double[] input = new double[inputOutputs[i].GameInputs.Inputs.Length];
                    for (int j = 0; j < input.Length; j++)
                    {
                        input[j] = Normalize(inputOutputs[i].GameInputs.Inputs[j], minIn, maxIn);
                    }
                    double[] output = new double[inputOutput.Length];
                    if (normilizeOnlyFirstOutput)
                    {
                        output[0] = Normalize(inputOutput[0], minOut, maxOut);
                    }
                    for (int j = 0; j < output.Length; j++)
                    {
                        if (normilizeOnlyFirstOutput && j != 0)
                        {
                            output[j] = inputOutput[j];
                        }
                        else
                        {
                            output[j] = Normalize(inputOutput[j], minOut, maxOut);
                        }
                    }
                    inputs[i] = input;
                    outputs[i] = output;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }

            double error = 0;
            Backpropagation trainBackProp = backPropV;
            if(policyNet)
            {
                trainBackProp = backPropPolicy;
            }
            do
            {
                for (int i = 0; i < inputs.Length; i += batchSize)
                {
                    var errorInfo = trainBackProp.TrainBatch(inputs, outputs, learningRate, i, batchSize);
                    error += errorInfo.Total;
                }
                error /= inputs.Length;
                Console.WriteLine(error);
                File.WriteAllText(netPath, trainBackProp.Net.Serialize());
            } while (error >= stopError);
        }
        

        public double Normalize(double value, double min, double max)
        {
            return (value - min) / max;
        }

        public void PruneInputOutputs(double maxUnNormalizedOutput)
        {
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                double[] outputs = inputOutputs[i].Output;
                if (inputOutputs[i].Output != null && (inputOutputs[i].Output[0] == double.MaxValue || inputOutputs[i].Output[0] == double.MinValue))
                {
                    if(inputOutputs[i].Output[0] == double.MaxValue)
                    {
                        outputs[0] = maxUnNormalizedOutput;
                    }
                    else
                    {
                        outputs[0] = -maxUnNormalizedOutput;
                    }
                }
                else
                {
                    BoardState state = inputOutputs[i].GameInputs.CurrentState.CheckBoardState();
                    if (state == BoardState.Draw)
                    {
                        outputs[0] = 0;
                    }
                    else if(state == BoardState.Win)
                    {
                        outputs[0] = maxUnNormalizedOutput;
                    }
                    else if(state == BoardState.Draw)
                    {
                        outputs[0] = -maxUnNormalizedOutput;
                    }
                }

                inputOutputs[i] = new InputOutputPair<T, T1>(inputOutputs[i].GameInputs, outputs, inputOutputs[i].PolicyOutput);
                debugInputOutputs[i] = new InputOutputDebugInfo(debugInputOutputs[i].BoardInfo, outputs, debugInputOutputs[i].PolicyOutput, debugInputOutputs[i].Player);
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
                var test = inputOutputs[i];
                if (inputOutputs[i].Output == null)
                {
                    double v = evaluator.EvaluateCurrentState(inputOutputs[i].GameInputs.CurrentState, inputOutputs[i].GameInputs.Player);
                    inputOutputs[i] = new InputOutputPair<T, T1>(inputOutputs[i].GameInputs, new double[] { v }, null);
                    debugInputOutputs[i] = new InputOutputDebugInfo(debugInputOutputs[i].BoardInfo, new double[] { v }, null, debugInputOutputs[i].Player);
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
                        Console.WriteLine(inputOutputs.Count - i - 1);
                    }
                    count++;
                }
            }
        }
        public void GetPolicyOutputs(int storeAmount = -1, string path = null, string debugPath = null)
        {
            int count = 0;
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                var test = inputOutputs[i];
                if (inputOutputs[i].PolicyOutput == null)
                {
                    Players p = inputOutputs[i].GameInputs.Player;
                    var avaialableMoves = inputOutputs[i].GameInputs.CurrentState.AvailableMoves(p);
                    double[] outputs = new double[inputOutputs[i].GameInputs.CurrentState.TotalAmountOfMoves];
                    for(int j = 0; j < outputs.Length; j++)
                    {
                        if (p == Players.YouOrFirst)
                        {
                            outputs[j] = -1;
                        }
                        else
                        {
                            outputs[j] = 1;
                        }
                    }
                    foreach (var m in avaialableMoves)
                    {
                        var state = inputOutputs[i].GameInputs.CurrentState.Copy();
                        state.MakeMove(new GameMove<T1>(m.Value, p));
                        double[] inputs = state.GetInputs(Funcs.OppositePlayer(p));

                        double[] output = backPropV.Net.Compute(inputs);
                        outputs[m.Key] = output[0];
                    }
                    inputOutputs[i] = new InputOutputPair<T, T1>(inputOutputs[i].GameInputs, inputOutputs[i].Output, outputs);
                    debugInputOutputs[i] = new InputOutputDebugInfo(debugInputOutputs[i].BoardInfo, debugInputOutputs[i].Output, outputs, debugInputOutputs[i].Player);
                }
            }
        }

        public void GetValuePolicyTrainingOutputs(int maxDepth, ITurnBasedGame<T, T1> game, bool useNetForMCT, int storeAmount = -1, string path = null, string debugPath = null)
        {
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> mtcEvalFunc = RandomChooseMove;
            if (useNetForMCT)
            {
                mtcEvalFunc = NetChooseMoveWithPolicy;
            }
            var monteCarloEvaluator = new MonteCarloEvaluator<T, T1>(MonteCarloTree<T, T1>.UTCSelection,
                   Math.Sqrt(2), mtcEvalFunc, 0, 25, game, maxDepth, true);//
            MinMaxEvaluator<T, T1> evaluator = new MinMaxEvaluator<T, T1>(game.Copy(), monteCarloEvaluator, 2, null);

            int count = 0;
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                var test = inputOutputs[i];
                if (inputOutputs[i].Output == null)
                {
                    var v = evaluator.EvaluateCurrentStateAndGetMove(inputOutputs[i].GameInputs.CurrentState, inputOutputs[i].GameInputs.Player);
                    if (v.moveKey == null)
                    {
                        inputOutputs.RemoveAt(i);
                        debugInputOutputs.RemoveAt(i);
                        i--;
                        continue;
                    }
                    double[] outputs = new double[inputOutputs[i].GameInputs.CurrentState.TotalAmountOfMoves + 1];
                    outputs[0] = v.value;
                    outputs[v.moveKey.Value + 1] = 1;

                    inputOutputs[i] = new InputOutputPair<T, T1>(inputOutputs[i].GameInputs, outputs, null);
                    debugInputOutputs[i] = new InputOutputDebugInfo(debugInputOutputs[i].BoardInfo, outputs, null, debugInputOutputs[i].Player);
                    if (count > 0 && count % storeAmount == 0)
                    {
                        Console.WriteLine(inputOutputs.Count - i - 1);
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

        public void GetTrainingInputs(ITurnBasedGame<T, T1> game, int amountOfSims, int maxDepth, Players startPlayer = Players.YouOrFirst)
        {
            if (inputOutputs.Count == 0)
            {
                MonteCarloTree<T, T1> tree = new MonteCarloTree<T, T1>(game, MonteCarloTree<T, T1>.UTCSelection, Math.Sqrt(2), NetChooseMoveWithValue, maxDepth, startPlayer);
                HashSet<MonteCarloNode<T, T1>> nodes = new HashSet<MonteCarloNode<T, T1>>();
                tree.RunMonteCarloSims(amountOfSims, true, tree.Root, nodes);
                foreach (var n in nodes)
                {
                    var player = n.Player;
                    var newInput = new InputOutputPair<T, T1>(new GameInputs<T, T1>(n.CurrentState.GetInputs(player), (T)n.CurrentState, player), null, null);
                    inputOutputs.Add(newInput);

                    debugInputOutputs.Add(new InputOutputDebugInfo(newInput.GameInputs.GetBoardInfo(), newInput.Output, null, newInput.GameInputs.Player));
                }
            }
        }

        public int NetChooseMoveWithValue(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            double maxMove;
            int index = int.MinValue;
            if (player == Players.YouOrFirst)
            {
                maxMove = double.MinValue;
            }
            else
            {
                maxMove = double.MaxValue;
            }
            foreach (var m in avaialableMoves)
            {
                var state = game.Copy();
                state.MakeMove(new GameMove<T1>(m.Value, player));
                double[] inputs = state.GetInputs(Funcs.OppositePlayer(player));

                double[] outputs = backPropV.Net.Compute(inputs);
                if (Better(maxMove, outputs[0], player))
                {
                    maxMove = outputs[0];
                    index = m.Key;
                }

            }
            return index;
        }

        public int NetChooseMoveWithPolicy(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            double[] inputs = game.GetInputs(Funcs.OppositePlayer(player));

            double[] outputs = backPropV.Net.Compute(inputs);
            int key = -1;
            double topMoveVal = double.MinValue;
            int outputsOffset = 1;
            foreach (var m in avaialableMoves)
            {
                double currentMoveVal = outputs[m.Key + outputsOffset];
                if (currentMoveVal > topMoveVal)
                {
                    topMoveVal = currentMoveVal;
                    key = m.Key;
                }
            }
            return key;
        }

        public static bool Better(double current, double value, Players player)
        {
            if (player == Players.YouOrFirst)
            {
                if (current < value)
                {
                    return true;
                }
                return false;
            }
            else if (player == Players.OpponentOrSecond)
            {
                if (current > value)
                {
                    return true;
                }
                return false;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        public int RandomChooseMove(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            var val = avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key;
            return val;
        }
    }
    public struct InputOutputPair<T, T1> where T : ITurnBasedGame<T, T1>
    {
        public GameInputs<T, T1> GameInputs { get; set; }
        public double[] Output { get; set; }
        public double[] PolicyOutput { get; set; }
        

        public InputOutputPair(GameInputs<T, T1> gameInputs, double[] output, double[] policyOutput)
        {
            GameInputs = gameInputs;
            Output = output;
            PolicyOutput = policyOutput;
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
        public double[] Output { get; set; }
        public BoardInfo BoardInfo { get; set; }
        public Players Player { get; set; }
        public double[] PolicyOutput { get; set; }
        public InputOutputDebugInfo(BoardInfo boardInfo, double[] output, double[] policyOutput, Players player)
        {
            PolicyOutput = policyOutput;
            Output = output;
            BoardInfo = boardInfo;
            Player = player;
        }
    }
}
