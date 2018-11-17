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
        public void TrainNeuralNet(int batchSize, double learningRate, double stopError, double minIn, double maxIn, double minOut, double maxOut, bool normilizeOnlyFirstOutput, bool policyNet, string netPath, double testDataPercent = 1, int testRate = 10)
        {
            List<double[]> inputsL = new List<double[]>();
            List<double[]> outputsL = new List<double[]>();
            List<double[]> testInputsL = new List<double[]>();
            List<double[]> testOutputsL = new List<double[]>();
            for (int i = 0; i < inputOutputs.Count; i++)
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
                    if (Funcs.Random.NextDouble() <= testDataPercent)
                    {
                        inputsL.Add(input);
                        outputsL.Add(output);
                    }
                    else
                    {
                        testInputsL.Add(input);
                        testOutputsL.Add(output);
                    }
                }
                else
                {
                    throw new NullReferenceException();
                }
            }

            double[][] inputs = inputsL.ToArray();
            double[][] outputs = outputsL.ToArray();
            double[][] testInputs = testInputsL.ToArray();
            double[][] testOutputs = testOutputsL.ToArray();

            double error = 0;
            Backpropagation trainBackProp = backPropV;
            if(policyNet)
            {
                trainBackProp = backPropPolicy;
            }
            int epoc = 0;
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
                if(testInputs.Length > 0 && epoc %testRate == 0)
                {
                    var testOut = trainBackProp.GetOutputs(testInputs);
                    double testError = trainBackProp.ErrorFunc.Invoke(trainBackProp.Net, testInputs, testOutputs, testOut, 0, testInputs.Length).Average;
                    Console.WriteLine("TestDataError: " + testError);
                }
                epoc++;
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
                    BoardState state = inputOutputs[i].GameInputs.CurrentState.CheckBoardState(inputOutputs[i].GameInputs.Player, false);
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

        public void GetTrainingInputs(ITurnBasedGame<T, T1> game, int amountOfSims, int maxDepth, ChooseMoveEvaluators chooseMoveEvaluator, Players startPlayer = Players.YouOrFirst)
        {
            if (inputOutputs.Count == 0)
            {
                Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, int> chooseMoveFunc = null;
                if(chooseMoveEvaluator == ChooseMoveEvaluators.Random)
                {
                    chooseMoveFunc = RandomChooseMove;
                }
                else if(chooseMoveEvaluator == ChooseMoveEvaluators.NeuarlNet)
                {
                    chooseMoveFunc = NetChooseMoveWithValue;
                }
                else if(chooseMoveEvaluator == ChooseMoveEvaluators.WeightedNeualNet)
                {
                    chooseMoveFunc = NetChooseMoveWithWeightedValue;
                }
                MonteCarloTree<T, T1> tree = new MonteCarloTree<T, T1>(game, MonteCarloTree<T, T1>.UTCSelection, Math.Sqrt(2), chooseMoveFunc, maxDepth, startPlayer);
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

        public int NetChooseMoveWithWeightedValue(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            if(avaialableMoves.Count == 0)
            {
                foreach(var m in avaialableMoves)
                {
                    return m.Key;
                }
            }
            double valMultiplier = 1;
            if(player == Players.OpponentOrSecond)
            {
                valMultiplier = -1;
            }
            double minVal = double.MaxValue;
            double totalVal = 0;
            Dictionary<int, double> values = new Dictionary<int, double>();
            foreach (var m in avaialableMoves)
            {
                var state = game.Copy();
                state.MakeMove(new GameMove<T1>(m.Value, player));
                double[] inputs = state.GetInputs(Funcs.OppositePlayer(player));

                double[] outputs = backPropV.Net.Compute(inputs);

                double value = outputs[0]* valMultiplier;
                if(value < minVal)
                {
                    minVal = value;
                }
                totalVal += value;
                values.Add(m.Key, value);
            }
            double minValDifference = -minVal;
            totalVal += minValDifference * values.Count;
            double random = Funcs.Random.NextDouble();
            random *= totalVal;
            random -= minValDifference;
            double closestValueDifference = double.MaxValue;
            int closestKey = -1;
            foreach(var v in values)
            {
                if(v.Value <= random)
                {
                    if(v.Value == random)
                    {
                        return v.Key;
                    }
                    double difference = random - v.Value;
                    if(difference < closestValueDifference)
                    {
                        closestValueDifference = difference;
                        closestKey = v.Key;
                    }
                }
            }
            return closestKey;
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

        public double NeuralNetEval(ITurnBasedGame<Checkers, CheckersMove> game, Players player)
        {
            double[] input = game.GetInputs(player);
            return backPropV.Net.Compute(input)[0];
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
    public enum ChooseMoveEvaluators
    {
        Random,
        NeuarlNet,
        WeightedNeualNet
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
