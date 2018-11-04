using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class MinMaxEvaluator: IEvaluateableTurnBasedGame<MinMaxGame, bool>
    {
        MinMaxGame game;
        public ITurnBasedGame<MinMaxGame, bool> Game { get { return game; } }

        private MinMaxEvaluator(MinMaxGame game)
        {
            game = new MinMaxGame(game);
        }
        public MinMaxEvaluator(int depth, Func<double> randomFunc, double[] values = null)
        {
            game = new MinMaxGame(depth, randomFunc, values);
        }

        public IEvaluateableTurnBasedGame<MinMaxGame, bool> CopyEInterface()
        {
            return new MinMaxEvaluator(this.game);
        }

        public double EvaluateCurrentState()
        {
            if (game.Depth - game.CurrentDepth <= 0)
            {
                return game.Nums[game.MinIndex];
            }
            else
            {
                return game.RandomFunc.Invoke();
            }
        }

        public void MakeMove(GameMove<bool> move, int moveIndex)
        {
            game.MakeMove(move);
        }

        public void Restart()
        {
            game.Restart();
        }
    }
}
