using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public static class Extensions
    {
        public static double NextDouble(this Random sender, double min, double max)
        {
            return sender.NextDouble() * (max - min) + min;
        }
    }
}
