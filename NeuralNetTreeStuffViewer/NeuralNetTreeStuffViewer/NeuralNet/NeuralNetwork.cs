using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class NeuralNetwork
    {
        public Layer[] Layers { get; private set; }
        Neuron biasNeuron;
        public NeuralNetwork(ActivationFunction activationFunction, Func<double, double, double> random, params int[] layerNeurons)
        {
            if (layerNeurons.Length < 2) { throw new IndexOutOfRangeException(); }
            Layers = new Layer[layerNeurons.Length];
            biasNeuron = new Neuron(null);
            biasNeuron.Output = 1;
            for (int i = 0; i < Layers.Length; i++)
            {
                Layer previous = null;
                if(i!= 0)
                {
                    previous = Layers[i - 1];
                }
                if(layerNeurons[i] <= 0) { throw new IndexOutOfRangeException(); }
                Layers[i] = new Layer(layerNeurons[i], previous, activationFunction, random, biasNeuron);
            }
        }

        public double[] Compute(double[] input)
        {
            Layers[0].SetOutputs(input);
            double[] output = null;
            for (int i = 1; i < Layers.Length; i++)
            {
                if (i == Layers.Length - 1)
                {
                    output = Layers[i].Compute();
                }
                else
                {
                    Layers[i].Compute();
                }
            }
            return output;
        }
    }
}
