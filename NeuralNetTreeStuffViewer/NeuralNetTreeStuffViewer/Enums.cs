using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public enum ConnectFourToken
    {
        None,
        Red,
        Blue
    }
    public enum Players
    {
        None,
        YouOrFirst,
        OpponentOrSecond
    }
    public enum BoardState
    {
        Win,
        Loss,
        Draw,
        Continue,
        IllegalMove,
    }
    
}
