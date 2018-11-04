using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{

    public struct GameMove<T>
    {
        public T Move { get; set; }
        public Players Player { get; set; }
        public GameMove(T move, Players player)
        {
            Move = move;
            Player = player;
        }
    }
}
