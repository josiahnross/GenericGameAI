using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Dendrite
    {
        [JsonProperty]
        public double Weight { get; set; }
        [JsonIgnore]
        public Neuron Previous { get; set; }
        [JsonProperty]
        public double WeightUpdate { get; set; }
        public Dendrite(double weight, Neuron previous)
        {
            Weight = weight;
            Previous = previous;
            WeightUpdate = 0;
        }
        public double ComputeNextInput(double previousOutput)
        {
            return previousOutput * Weight;
        }
    }
}
