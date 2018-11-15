using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer
{
    public class My2dArray<T>
    {
        [JsonIgnore]
        public int XLength { get; }
        [JsonIgnore]
        public int YLength { get; }
        [JsonIgnore]
        public T this[BoardPosition position] { get => this[position.X, position.Y]; set => this[position.X, position.Y] = value; }
        [JsonIgnore]
        public T this [int x, int y]
        {
            get
            {
                if(x < 0 || x >= XLength || y < 0 || y >= YLength)
                {
                    throw new IndexOutOfRangeException();
                }
                return Array[(y * XLength) + x];
            }
            set
            {
                if (x < 0 || x >= XLength || y < 0 || y >= YLength)
                {
                    throw new IndexOutOfRangeException();
                }
                Array[(y * XLength) + x] = value;
            }
        }
        [JsonProperty]
        public T[] Array { get; private set; }
        public My2dArray(int xLength, int yLength)
        {
            XLength = xLength;
            YLength = yLength;
            Array = new T[XLength * YLength];
        }
        public My2dArray<T> Copy()
        {
            My2dArray<T> newA = new My2dArray<T>(XLength, YLength);
            for(int i = 0; i < Array.Length; i++)
            {
                newA.Array[i] = Array[i];
            }
            return newA;
        }
        public bool ValueEqulas(My2dArray<T> otherArray)
        {
            if(otherArray.XLength != XLength || otherArray.YLength != YLength)
            {
                return false;
            }
            for(int i = 0; i < Array.Length; i++)
            {
                if(!Array[i].Equals(otherArray.Array[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public bool InArray(BoardPosition position)
        {
            return InArray(position.X, position.Y);
        }
        public bool InArray(int x, int y)
        {
            return x >= 0 && y >= 0 && x < XLength && y < YLength;
        }
    }
}
