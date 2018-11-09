using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Dendrite
    {
        public double Weight { get; set; }
        public Neuron Previous { get; set; }
        public double WeightUpdate { get; set; }
        public Dendrite(double weight, Neuron previous)
        {
            Weight = weight;
            Previous = previous;
            WeightUpdate = 0;
        }
        public void UpdateWeight()
        {
            Weight += WeightUpdate;
            WeightUpdate = 0;
        }
        public double ComputeNextInput()
        {
            return Previous.Output * Weight;
        }
    }
}
