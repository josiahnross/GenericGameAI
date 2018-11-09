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
        List<GameInputs<T, T1>> inputs;
        uint minMaxDepth;
        public NeuralNetGameTrainer(uint minMaxDepth)
        {
            inputs = new List<GameInputs<T, T1>>();
            this.minMaxDepth = minMaxDepth;
        }

        public void GetTrainingOutputs()
        {
            MinMaxNode<T, T1> node;
            MinMaxNode<T, T1> nextMoveChild = null;



            node = MinMaxAlgorithm<T, T1>.EvaluateMoves(minMaxDepth, this, AiFirst);
            nextMoveChild = null;
            foreach (var n in node.Children)
            {
                if (n.Value == node.Value)
                {
                    nextMoveChild = n;
                    break;
                }
            }
        }

        public void GetTrainingInputs(ITurnBasedGame<T, T1> game, int amountOfSims, string path)
        {
            MonteCarloTree<T, T1> tree = new MonteCarloTree<T, T1>(game, MonteCarloTree<T, T1>.UTCSelection, Math.Sqrt(2), NetChooseMove);
            HashSet<MonteCarloNode<T, T1>> nodes = new HashSet<MonteCarloNode<T, T1>>();
            tree.RunMonteCarloSims(amountOfSims, tree.Root, nodes);
            foreach (var n in nodes)
            {
                inputs.Add(new GameInputs<T, T1>(n.CurrentState.GetInputs(Funcs.OppositePlayer(n.Player)), n.CurrentState));
            }
            if (path != null && path.Length > 0)
            {
                File.WriteAllText(JsonConvert.SerializeObject(inputs), path);
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
    public struct GameInputs<T, T1> where T : ITurnBasedGame<T, T1>
    {
        public double[] Inputs { get; set; }
        public ITurnBasedGame<T, T1> CurrentState { get; set; }
        public GameInputs(double[] inputs, ITurnBasedGame<T, T1> currentState)
        {
            Inputs = inputs;
            CurrentState = currentState;
        }
    }
}
