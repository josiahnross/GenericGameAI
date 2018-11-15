using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Layer
    {
        [JsonIgnore]
        public Neuron this[int i]
        {
            get
            {
                return Neurons[i];
            }
        }
        [JsonProperty]
        public Neuron[] Neurons { get; private set; }
        public Layer(int neuronCount, Layer previousLayer, ActivationFunction activationFunction, Func<double, double, double> Random, Neuron biasNeuron)
        {
            Neurons = new Neuron[neuronCount];
            for(int i = 0; i< Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(activationFunction);
                if(previousLayer != null)
                {
                    for(int j = 0; j < previousLayer.Neurons.Length; j++)
                    {
                        Neurons[i].Dendrites.Add(new Dendrite(Random(activationFunction.Min, activationFunction.Max), previousLayer[j]));
                    }
                    Neurons[i].Dendrites.Add(new Dendrite(Random(activationFunction.Min, activationFunction.Max), biasNeuron));
                }
            }
        }

        public void SetOutputs(double[] outputs)
        {
            for(int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i].Output = outputs[i];
            }
        }

        public double[] Compute()
        {
            double[] output = new double[Neurons.Length];
            for (int i = 0; i < Neurons.Length; i++)
            {
                output[i] = Neurons[i].Compute();
            }
            return output;
        }
    }
}
