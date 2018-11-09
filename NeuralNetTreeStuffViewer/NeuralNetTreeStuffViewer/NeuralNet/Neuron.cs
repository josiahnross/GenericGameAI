using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Neuron
    {
        public List<Dendrite> Dendrites { get; set; }
        public double Output { get; set; }
        public double Input { get; set; }
        public ActivationFunction ActivationFunction { get; }
        public Neuron(ActivationFunction activationFunction)
        {
            ActivationFunction = activationFunction;
            Dendrites = new List<Dendrite>();
        }

        public double Compute()
        {
            double input = 0;
            for (int i = 0; i < Dendrites.Count; i++)
            {
                input += Dendrites[i].ComputeNextInput();
            }
            Input = input;
            Output = ActivationFunction.Function(input);
            return Output;
        }
    }
}
