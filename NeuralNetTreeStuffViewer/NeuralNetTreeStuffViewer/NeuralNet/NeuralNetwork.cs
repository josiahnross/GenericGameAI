using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class NeuralNetwork
    {
        [JsonIgnore]
        public Layer this [int i]
        {
            get
            {
                return Layers[i];
            }
        }
        [JsonProperty]
        public Layer[] Layers { get; private set; }
        [JsonProperty]
        Neuron biasNeuron;

        [JsonConstructor]
        private NeuralNetwork()
        {

        }
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

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static NeuralNetwork Deserialize(string json)
        {
            var net = JsonConvert.DeserializeObject<NeuralNetwork>(json);

            for(int l = 1; l < net.Layers.Length; l++)
            {
                for(int n = 0; n < net[l].Neurons.Length; n++)
                {
                    net[l][n].GetActivationFunctionFromInfo();
                    for(int d = 0; d < net[l][n].Dendrites.Count; d++)
                    {
                        if(d < net[l-1].Neurons.Length)
                        {
                            net[l][n].Dendrites[d].Previous = net[l - 1][d];
                        }
                        else
                        {
                            net[l][n].Dendrites[d].Previous = net.biasNeuron;
                        }
                    }
                }
            }
            net.biasNeuron.GetActivationFunctionFromInfo();

            return net;
        }

        public double[] Compute(double[] input)
        {
            //Layers[0].SetOutputs(input);
            double[] output = null;
            for (int i = 1; i < Layers.Length; i++)
            {
                if (i == Layers.Length - 1)
                {
                    output = Layers[i].Compute(input);
                }
                else
                {
                    input = Layers[i].Compute(input);
                }
            }
            return output;
        }
    }
}
