using NeuralNetTreeStuffViewer.MinMaxAlg;
using NeuralNetTreeStuffViewer.NeuralNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public interface ITrainer
    {
        bool LoadedFromFile { get; }
        bool Parallel { get; set; }
        bool CurrentlyParallel { get; }
        void LoadNeuralNet(string path);
        void LoadPolicyNeualNet(string path);
        void TrainNeuralNet(int batchSize, double learningRate, double stopError, double minIn, double maxIn, double minOut, double maxOut, bool normilizeOnlyFirstOutput, bool policyNet, string netPath, double testDataPercent = 1, int testRate = 10, double ignoreZeroRate = 0);
        void PruneInputOutputs(double maxUnNormalizedOutput);
        void StoreInputOutputs(string path);
        void WipeOutputs();
        void Stop();
        int GetNetInputs();
    }
    public class NeuralNetGameTrainer<T, T1> : ITrainer
        where T : ITurnBasedGame<T, T1>, new()
        where T1 : struct
    {
        public bool Parallel { get; set; }
        public bool CurrentlyParallel { get; private set; }
        public bool LoadedFromFile { get; private set; }
        List<InputOutput<T, T1>> inputOutputs;
        //List<InputOutputDebugInfo> debugInputOutputs;
        Backpropagation backPropV;
        Backpropagation backPropPolicy;
        IEvaluateableTurnBasedGame<T, T1>[] currentEvaluators;
        public NeuralNetGameTrainer(Backpropagation backProp, Backpropagation backPropPolicy = null, string inputOutputPath = null)
        {
            Parallel = true;
            this.backPropV = backProp;
            this.backPropPolicy = backPropPolicy;
            if (inputOutputPath == null || !File.Exists(inputOutputPath))
            {
                inputOutputs = new List<InputOutput<T, T1>>();
            }
            else
            {
                string data = File.ReadAllText(inputOutputPath);
                try
                {
                    inputOutputs = JsonConvert.DeserializeObject<List<InputOutput<T, T1>>>(data);
                }
                catch
                {
                    LoadedFromFile = false;
                    return;
                }
                LoadedFromFile = true;
                if (inputOutputs.Count > 0)
                {
                    inputOutputs[0].InputOutputPair.GameInputs.CurrentState.InitializeStaticVariables();
                    for (int i = 0; i < inputOutputs.Count; i++)
                    {
                        inputOutputs[i].InputOutputPair.GameInputs.CurrentState.DeserializeInit();
                    }
                }
            }
        }
        public int GetNetInputs()
        {
            if(inputOutputs.Count <= 0)
            {
                return 0;
            }
            return inputOutputs[0].InputOutputPair.GameInputs.Inputs.Length;
        }
        public void LoadNeuralNet(string path)
        {
            backPropV = new Backpropagation(NeuralNetwork.Deserialize(File.ReadAllText(path)), backPropV.ErrorFunc);
        }
        public void LoadPolicyNeualNet(string path)
        {
            backPropPolicy = new Backpropagation(NeuralNetwork.Deserialize(File.ReadAllText(path)), backPropPolicy.ErrorFunc);
        }
        public void TrainNeuralNet(int batchSize, double learningRate, double stopError, double minIn, double maxIn, double minOut, double maxOut, bool normilizeOnlyFirstOutput, bool policyNet, string netPath, double testDataPercent = 1, int testRate = 10, double ignoreZeroRate = 0)
        {
            List<double[]> inputsL = new List<double[]>();
            List<double[]> outputsL = new List<double[]>();
            List<double[]> testInputsL = new List<double[]>();
            List<double[]> testOutputsL = new List<double[]>();
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                double[] inputOutput;
                if (policyNet)
                {
                    inputOutput = inputOutputs[i].InputOutputPair.PolicyOutput;
                }
                else
                {
                    inputOutput = inputOutputs[i].InputOutputPair.Output;
                }
                if (inputOutput != null)
                {
                    double[] input = new double[inputOutputs[i].InputOutputPair.GameInputs.Inputs.Length];
                    for (int j = 0; j < input.Length; j++)
                    {
                        input[j] = Normalize(inputOutputs[i].InputOutputPair.GameInputs.Inputs[j], minIn, maxIn);
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
                        if (output[0] == 0 && ignoreZeroRate > 0 && Funcs.Random.NextDouble() > ignoreZeroRate)
                        {
                            inputsL.Add(input);
                            outputsL.Add(output);
                        }
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
            if (policyNet)
            {
                trainBackProp = backPropPolicy;
            }
            int epoc = 0;
            do
            {
                for (int i = 0; i < inputs.Length; i += batchSize)
                {
                    var errorInfo = trainBackProp.TrainBatch(inputs, outputs, learningRate, i, batchSize, false);
                    error += errorInfo.Total;
                }
                error /= inputs.Length;
                Console.WriteLine(error);
                File.WriteAllText(netPath, trainBackProp.Net.Serialize());
                if (testInputs.Length > 0 && epoc % testRate == 0)
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
                double[] outputs = inputOutputs[i].InputOutputPair.Output;
                if (inputOutputs[i].InputOutputPair.Output != null && inputOutputs[i].InputOutputPair.Output[0] > maxUnNormalizedOutput)
                {
                    inputOutputs[i].InputOutputPair.Output[0] = maxUnNormalizedOutput;
                }
                if (inputOutputs[i].InputOutputPair.Output != null && (inputOutputs[i].InputOutputPair.Output[0] == double.MaxValue || inputOutputs[i].InputOutputPair.Output[0] == double.MinValue))
                {
                    if (inputOutputs[i].InputOutputPair.Output[0] == double.MaxValue)
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
                    BoardState state = inputOutputs[i].InputOutputPair.GameInputs.CurrentState.CheckBoardState(inputOutputs[i].InputOutputPair.GameInputs.Player, false);
                    if (state == BoardState.Draw)
                    {
                        outputs[0] = 0;
                    }
                    else if (state == BoardState.Win)
                    {
                        outputs[0] = maxUnNormalizedOutput;
                    }
                    else if (state == BoardState.Draw)
                    {
                        outputs[0] = -maxUnNormalizedOutput;
                    }
                }

                inputOutputs[i] =new InputOutput<T, T1>(new InputOutputPair<T, T1>(inputOutputs[i].InputOutputPair.GameInputs, outputs, inputOutputs[i].InputOutputPair.PolicyOutput, inputOutputs[i].InputOutputPair.Depth),
                                                        new InputOutputDebugInfo(inputOutputs[i].DebugInputOutput.BoardInfo, outputs, inputOutputs[i].DebugInputOutput.PolicyOutput, inputOutputs[i].DebugInputOutput.Player));
            }
        }

        public void StoreInputOutputs(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(inputOutputs));
        }
        public void WipeOutputs()
        {
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                if (inputOutputs[i].InputOutputPair.Output != null)
                {
                    inputOutputs[i] = new InputOutput<T, T1>(new InputOutputPair<T, T1>(inputOutputs[i].InputOutputPair.GameInputs, null, null, inputOutputs[i].InputOutputPair.Depth),
                                                             new InputOutputDebugInfo(inputOutputs[i].DebugInputOutput.BoardInfo, null, null, inputOutputs[i].DebugInputOutput.Player));
                }
            }
        }
        public void GetTrainingOutputs(IEvaluateableTurnBasedGame<T, T1> evaluator, int writeRemainingAamount = -1, string path = null, int parallelBatchAmount = 100)
        {
            int count = 0;
            int realCount = 0;

            for (int j = 0; j < inputOutputs.Count; j += parallelBatchAmount)
            {
                int amount = Math.Min(parallelBatchAmount, inputOutputs.Count - j);
                currentEvaluators = new IEvaluateableTurnBasedGame<T, T1>[amount];
                bool parallelChange = false;
                if (Parallel)
                {
                    CurrentlyParallel = true;
                    System.Threading.Tasks.Parallel.For(0, amount, (i, state) =>
                    {
                        if (InsideLoop(i, j, true, evaluator, ref count, ref realCount, writeRemainingAamount))
                        {
                            state.Break();
                        }
                    });
                    parallelChange = !Parallel;
                }
                else
                {
                    CurrentlyParallel = false;
                    for (int i = 0; i < amount; i++)
                    {
                        InsideLoop(i, j, false, evaluator, ref count, ref realCount, writeRemainingAamount);
                        if (Parallel)
                        {
                            break;
                        }
                    }
                    parallelChange = Parallel;
                }
                if (realCount > 0)
                {
                    if (path != null)
                    {
                        StoreInputOutputs(path);
                    }
                    Console.WriteLine("Stored");
                }
                if (parallelChange)
                {
                    j -= parallelBatchAmount;
                    continue;
                }
            }

        }
        bool InsideLoop(int i, int j, bool inParallel, IEvaluateableTurnBasedGame<T, T1> evaluator, ref int count, ref int realCount, int writeRemainingAamount)
        {
            if ((inParallel && !Parallel) || (!inParallel && Parallel))
            {
                return true;
            }
            int index = i + j;
            if (inputOutputs[index].InputOutputPair.Output == null)
            {
                var input = inputOutputs[index];
                var currentState = input.InputOutputPair.GameInputs.CurrentState;
                IEvaluateableTurnBasedGame<T, T1> currentEval = evaluator.CopyWithNewState(currentState, input.InputOutputPair.GameInputs.Player);
                currentEvaluators[i] = currentEval;
                var info = GetOutputs(input.InputOutputPair, input.DebugInputOutput, currentEval);
                if (info.Item1.Output != null)
                {
                    inputOutputs[index].InputOutputPair = info.Item1;
                    inputOutputs[index].DebugInputOutput = info.Item2;
                    var currentCount = Interlocked.Increment(ref count);
                    Interlocked.Increment(ref realCount);
                    if (currentCount % writeRemainingAamount == 0)
                    {
                        var output = info.Item1.Output[0];
                        Console.WriteLine(inputOutputs.Count - currentCount + " Output: " + output);
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                Interlocked.Increment(ref count);
            }
            return false;
        }

        //List<InputOutputPair<T, T1>> inputs, List<InputOutputDebugInfo> debugInputs, int i,
        (InputOutputPair<T, T1>, InputOutputDebugInfo) GetOutputs(InputOutputPair<T, T1> input, InputOutputDebugInfo debugInfo, IEvaluateableTurnBasedGame<T, T1> evaluator)
        {
            double? v = evaluator.EvaluateCurrentState(/*inputs[i].GameInputs.CurrentState,*/ input.GameInputs.Player, 0);
            var output = new InputOutputPair<T, T1>(input.GameInputs, v == null ? null : new double[] { v.Value }, null, input.Depth);
            var debugOutput = new InputOutputDebugInfo(debugInfo.BoardInfo, v == null ? null : new double[] { v.Value }, null, debugInfo.Player);
            return (output, debugOutput);
        }


        public void GetPolicyOutputs(int storeAmount = -1, string path = null, string debugPath = null)
        {
            int count = 0;
            for (int i = 0; i < inputOutputs.Count; i++)
            {
                var test = inputOutputs[i];
                if (inputOutputs[i].InputOutputPair.PolicyOutput == null)
                {
                    Players p = inputOutputs[i].InputOutputPair.GameInputs.Player;
                    var avaialableMoves = inputOutputs[i].InputOutputPair.GameInputs.CurrentState.AvailableMoves(p);
                    double[] outputs = new double[inputOutputs[i].InputOutputPair.GameInputs.CurrentState.TotalAmountOfMoves];
                    for (int j = 0; j < outputs.Length; j++)
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
                        var state = inputOutputs[i].InputOutputPair.GameInputs.CurrentState.Copy();
                        state.MakeMove(new GameMove<T1>(m.Value, p));
                        double[] inputs = state.GetInputs(Funcs.OppositePlayer(p));

                        double[] output = backPropV.Net.Compute(inputs);
                        outputs[m.Key] = output[0];
                    }
                    inputOutputs[i] = new InputOutput<T, T1>(new InputOutputPair<T, T1>(inputOutputs[i].InputOutputPair.GameInputs, inputOutputs[i].InputOutputPair.Output, outputs, inputOutputs[i].InputOutputPair.Depth),
                                                             new InputOutputDebugInfo(inputOutputs[i].DebugInputOutput.BoardInfo, inputOutputs[i].DebugInputOutput.Output, outputs, inputOutputs[i].DebugInputOutput.Player));
                }
            }
        }

        public void GetTrainingInputs(ITurnBasedGame<T, T1> game, int amountOfSims, int maxDepth, ChooseMoveEvaluators chooseMoveEvaluator, bool removeDraws, Players startPlayer = Players.YouOrFirst)
        {
            if (inputOutputs.Count == 0)
            {
                Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> chooseMoveFunc = null;
                if (chooseMoveEvaluator == ChooseMoveEvaluators.Random)
                {
                    chooseMoveFunc = RandomChooseMove;
                }
                else if (chooseMoveEvaluator == ChooseMoveEvaluators.NeuarlNet)
                {
                    chooseMoveFunc = NetChooseMoveWithValue;
                }
                else if (chooseMoveEvaluator == ChooseMoveEvaluators.WeightedNeualNet)
                {
                    chooseMoveFunc = NetChooseMoveWithWeightedValue;
                }
                MonteCarloTree<T, T1> tree = new MonteCarloTree<T, T1>(game, MonteCarloTree<T, T1>.UTCSelection, Math.Sqrt(2), chooseMoveFunc, maxDepth, startPlayer);
                HashSet<MonteCarloNode<T, T1>> nodes = new HashSet<MonteCarloNode<T, T1>>();
                tree.RunMonteCarloSims(amountOfSims, true, true, true, tree.Root, nodes);
                foreach (var n in nodes)
                {
                    if (!removeDraws || n.GameInfo.Player1Wins + n.GameInfo.Player2Wins != 0)
                    {
                        var player = n.Player;
                        var newInput = new InputOutputPair<T, T1>(new GameInputs<T, T1>(n.CurrentState.GetInputs(player), (T)n.CurrentState, player), null, null, n.Depth);
                        var newDebug = new InputOutputDebugInfo(newInput.GameInputs.GetBoardInfo(), newInput.Output, null, newInput.GameInputs.Player);
                        inputOutputs.Add(new InputOutput<T, T1>(newInput, newDebug));
                    }
                }
            }
        }


        public void GetValuePolicyTrainingOutputs(int maxDepth, ITurnBasedGame<T, T1> game, bool useNetForMCT, int storeAmount = -1, string path = null, string debugPath = null)
        {
            Func<ITurnBasedGame<T, T1>, Dictionary<int, T1>, Players, (int key, ITurnBasedGame<T, T1> newBoardState)> mtcEvalFunc = RandomChooseMove;
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
                if (inputOutputs[i].InputOutputPair.Output == null)
                {
                    var v = evaluator.EvaluateCurrentStateAndGetMove(inputOutputs[i].InputOutputPair.GameInputs.CurrentState, inputOutputs[i].InputOutputPair.GameInputs.Player);
                    if (v.moveKey == null)
                    {
                        inputOutputs.RemoveAt(i);
                        i--;
                        continue;
                    }
                    double[] outputs = new double[inputOutputs[i].InputOutputPair.GameInputs.CurrentState.TotalAmountOfMoves + 1];
                    outputs[0] = v.value;
                    outputs[v.moveKey.Value + 1] = 1;

                    inputOutputs[i] = new InputOutput<T, T1>(new InputOutputPair<T, T1>(inputOutputs[i].InputOutputPair.GameInputs, outputs, null, inputOutputs[i].InputOutputPair.Depth),
                                                             new InputOutputDebugInfo(inputOutputs[i].DebugInputOutput.BoardInfo, outputs, null, inputOutputs[i].DebugInputOutput.Player));
                    if (count > 0 && count % storeAmount == 0)
                    {
                        Console.WriteLine(inputOutputs.Count - i - 1);
                        if (path != null)
                        {
                            StoreInputOutputs(path);
                        }
                    }
                    count++;
                }
            }
        }

        public (int key, ITurnBasedGame<T, T1> newBoardState) NetChooseMoveWithWeightedValue(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            if (avaialableMoves.Count == 0)
            {
                foreach (var m in avaialableMoves)
                {
                    return (m.Key, null);
                }
            }
            double valMultiplier = 1;
            if (player == Players.OpponentOrSecond)
            {
                valMultiplier = -1;
            }
            double minVal = double.MaxValue;
            double totalVal = 0;
            Dictionary<int, double> values = new Dictionary<int, double>();
            Dictionary<int, ITurnBasedGame<T, T1>> createdStates = new Dictionary<int, ITurnBasedGame<T, T1>>();
            foreach (var m in avaialableMoves)
            {
                var state = game.Copy();
                state.MakeMove(new GameMove<T1>(m.Value, player));
                createdStates.Add(m.Key, state);
                double[] inputs = state.GetInputs(Funcs.OppositePlayer(player));

                double[] outputs = backPropV.Net.Compute(inputs);

                double value = outputs[0] * valMultiplier;
                if (value < minVal)
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
            foreach (var v in values)
            {
                if (v.Value <= random)
                {
                    if (v.Value == random)
                    {
                        return (v.Key, createdStates[v.Key]);
                    }
                    double difference = random - v.Value;
                    if (difference < closestValueDifference)
                    {
                        closestValueDifference = difference;
                        closestKey = v.Key;
                    }
                }
            }
            return (closestKey, createdStates[closestKey]);
        }

        int amount = 0;
        public (int key, ITurnBasedGame<T, T1> newBoardState) NetChooseMoveWithValue(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            amount++;
            if (avaialableMoves.Count == 1)
            {
                foreach (var m in avaialableMoves)
                {
                    return (m.Key, null);
                }
            }
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
            Dictionary<int, ITurnBasedGame<T, T1>> newStates = new Dictionary<int, ITurnBasedGame<T, T1>>();
            foreach (var m in avaialableMoves)
            {
                var state = game.Copy();
                state.MakeMove(new GameMove<T1>(m.Value, player));
                newStates.Add(m.Key, state);
                double[] inputs = state.GetInputs(Funcs.OppositePlayer(player));

                double[] outputs = backPropV.Net.Compute(inputs);
                if (Better(maxMove, outputs[0], player))
                {
                    maxMove = outputs[0];
                    index = m.Key;
                }
            }
            return (index, newStates[index]);
        }

        public (int key, ITurnBasedGame<T, T1> newBoardState) NetChooseMoveWithPolicy(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
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
            return (key, null);
        }

        int amountOfRandomValues = 4;
        public (int key, ITurnBasedGame<T, T1> newBoardState) NetChooseMoveOutofRandomWithValue(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            if (avaialableMoves.Count == 1)
            {
                foreach (var m in avaialableMoves)
                {
                    return (m.Key, null);
                }
            }
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
            Dictionary<int, T1> randAvaialableMoves;
            if (avaialableMoves.Count <= amountOfRandomValues)
            {
                randAvaialableMoves = avaialableMoves;
            }
            else
            {
                randAvaialableMoves = new Dictionary<int, T1>(amountOfRandomValues);
                for (int i = 0; i < amountOfRandomValues; i++)
                {
                    var key = avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key;
                    randAvaialableMoves.Add(key, avaialableMoves[key]);
                    avaialableMoves.Remove(key);
                }
                foreach (var m in randAvaialableMoves)
                {
                    avaialableMoves.Add(m.Key, m.Value);
                }
            }
            Dictionary<int, ITurnBasedGame<T, T1>> newStates = new Dictionary<int, ITurnBasedGame<T, T1>>();
            foreach (var m in randAvaialableMoves)
            {
                var state = game.Copy();
                state.MakeMove(new GameMove<T1>(m.Value, player));
                newStates.Add(m.Key, state);
                double[] inputs = state.GetInputs(Funcs.OppositePlayer(player));

                double[] outputs = backPropV.Net.Compute(inputs);
                if (Better(maxMove, outputs[0], player))
                {
                    maxMove = outputs[0];
                    index = m.Key;
                }
            }
            return (index, newStates[index]);
        }

        public static (int key, ITurnBasedGame<T, T1> newBoardState) RandomChooseMove(ITurnBasedGame<T, T1> game, Dictionary<int, T1> avaialableMoves, Players player)
        {
            var val = avaialableMoves.ElementAt(Funcs.Random.Next(0, avaialableMoves.Count)).Key;
            return (val, null);
        }

        public double NeuralNetEval(ITurnBasedGame<T, T1> game, Players player)
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
        bool stop = false;
        public void Stop()
        {
            stop = true;
            for (int i = 0; i < currentEvaluators.Length; i++)
            {
                if (currentEvaluators[i] != null)
                {
                    currentEvaluators[i].Stop(true);
                }
            }
        }
    }

    public enum ChooseMoveEvaluators
    {
        Random,
        NeuarlNet,
        WeightedNeualNet
    }
    
    public class InputOutput<T,T1> where T : ITurnBasedGame<T, T1>, new()
    {
        public InputOutputPair<T,T1> InputOutputPair { get; set; }
        public InputOutputDebugInfo DebugInputOutput { get; set; }
        public InputOutput(InputOutputPair<T, T1> inputOutputPair, InputOutputDebugInfo debugInputOutput)
        {
            InputOutputPair = inputOutputPair;
            DebugInputOutput = debugInputOutput;
        }
    }
    public struct InputOutputPair<T, T1> where T : ITurnBasedGame<T, T1>, new()
    {
        public GameInputs<T, T1> GameInputs { get; set; }
        public double[] Output { get; set; }
        public double[] PolicyOutput { get; set; }
        public int Depth { get; set; }

        public InputOutputPair(GameInputs<T, T1> gameInputs, double[] output, double[] policyOutput, int depth)
        {
            GameInputs = gameInputs;
            Output = output;
            PolicyOutput = policyOutput;
            Depth = depth;
        }
    }
    public struct GameInputs<T, T1> where T : ITurnBasedGame<T, T1>,new()
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
