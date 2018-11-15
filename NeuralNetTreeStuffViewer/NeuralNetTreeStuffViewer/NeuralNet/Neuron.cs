using NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Neuron
    {
        [JsonProperty]
        public List<Dendrite> Dendrites { get; set; }
        [JsonProperty]
        public double Output { get; set; }
        [JsonProperty]
        public double Input { get; set; }
        [JsonProperty]
        ActivationFunctionInfo? activationFunctionInfo { get; set; }
        [JsonIgnore]
        public ActivationFunction ActivationFunction { get; private set; }
        [JsonProperty]
        public double Delta { get; set; }
        public Neuron(ActivationFunction activationFunction)
        {
            ActivationFunction = activationFunction;
            if (ActivationFunction != null)
            {
                activationFunctionInfo = ActivationFunction.ActivationFunctionInfo;
            }
            else
            {
                activationFunctionInfo = null;
            }
            Dendrites = new List<Dendrite>();
            Delta = 0;
        }

        public void GetActivationFunctionFromInfo()
        {
            ActivationFunction = ActivationFunction.ActivationFunctionFromInfo(activationFunctionInfo);
        }

        public void ApplyAndClearWeightUpdates()
        {
            Delta = 0;
            for(int i = 0; i < Dendrites.Count; i++)
            {
                Dendrites[i].Weight += Dendrites[i].WeightUpdate;
                Dendrites[i].WeightUpdate = 0;
            }
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
