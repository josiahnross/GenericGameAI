using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet
{
    public class Backpropagation
    {
        public NeuralNetwork Net { get; private set; }
        public Func<NeuralNetwork, double[][], double[][], double[][], int, int, ErrorInfo> ErrorFunc { get; } 
        public Backpropagation(NeuralNetwork neuralNetwork, Func<NeuralNetwork, double[][], double[][], double[][], int, int, ErrorInfo> errorFunc = null)
        {
            if(errorFunc == null)
            {
                errorFunc = MeanAbsoluteError;
            }
            this.ErrorFunc = errorFunc;
            Net = neuralNetwork;
        }
        
        public double[][] GetOutputs(double[][] inputs)
        {
            double[][] outputs = new double[inputs.Length][];
            for(int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = Net.Compute(inputs[i]);
            }
            return outputs;
        }

        public ErrorInfo TrainEpoch(double[][] inputs, double[][] desiredOutputs, double learningRate)
        {
            return TrainBatch(inputs, desiredOutputs, learningRate, 0, inputs.Length);
        }

        public ErrorInfo TrainBatch(double[][] inputs, double[][] desiredOutputs, double learningRate, int startIndex, int count)
        {
            learningRate = Extensions.Clamp(learningRate, 0, 1);
            double[][] outputs = new double[inputs.Length][];
            for (int i = startIndex; i < count+startIndex && i < inputs.Length; i++)
            {
                outputs[i] = Train(inputs[i], desiredOutputs[i], learningRate);
            }
            ApplyAndClearUpdates();
            
            return ErrorFunc.Invoke(Net, inputs, desiredOutputs, outputs, startIndex, count);
        }

        public static ErrorInfo MeanAbsoluteError(NeuralNetwork net, double[][] inputs, double[][] desiredOutputs, double[][] outputs, int startIndex, int count)
        {
            double mae = 0;
            for (int i = startIndex; i < count + startIndex && i < inputs.Length; i++)
            {
                double[] output = outputs[i];
                mae += desiredOutputs[i].Zip(output, (e, a) => Math.Abs(e - a)).Average();
            }
            return new ErrorInfo(mae / Math.Min(count, inputs.Length - startIndex), mae);
        }

        private double[] Train(double[] input, double[] desiredOutput, double learningRate)
        {
            double[] output = Net.Compute(input);
            CalculateError(desiredOutput);
            CalculateUpdates(input, learningRate);
            return output;
        }

        private void CalculateError(double[] desiredOutput)
        {
            //Output Layer
            Layer outputLayer = Net.Layers[Net.Layers.Length - 1];
            for (int i = 0; i < outputLayer.Neurons.Length; i++)
            {
                Neuron neuron = outputLayer.Neurons[i];
                double error = (desiredOutput[i] - neuron.Output);
                double deriv;
                if(neuron.ActivationFunction.CanUseOutputDerivative)
                {
                    deriv = neuron.ActivationFunction.OutputDerivative(neuron.Output);
                }
                else
                {
                    deriv = neuron.ActivationFunction.Derivative(neuron.Input);
                }
                neuron.Delta = error * deriv;
            }

            //Hidden Layers
            for (int i = Net.Layers.Length - 2; i >= 0; i--)
            {
                Layer currLayer = Net.Layers[i];
                Layer nextLayer = Net.Layers[i + 1];

                for (int j = 0; j < currLayer.Neurons.Length; j++)
                {
                    Neuron neuron = currLayer.Neurons[j];

                    double error = 0.0;
                    for(int k = 0; k < nextLayer.Neurons.Length; k++)
                    {
                        error += nextLayer[k].Delta * nextLayer[k].Dendrites[j].Weight;
                    }
                    double deriv = 1;
                    if (i != 0)//input doesn't use activation function
                    {
                        if (neuron.ActivationFunction.CanUseOutputDerivative)
                        {
                            deriv = neuron.ActivationFunction.OutputDerivative(neuron.Output);
                        }
                        else
                        {
                            deriv = neuron.ActivationFunction.Derivative(neuron.Input);
                        }
                    }
                    neuron.Delta = error * deriv;
                }
            }
        }

        private void CalculateUpdates(double[] input, double learningRate)
        {
            for (int i = 1; i < Net.Layers.Length; i++)
            {
                Layer currLayer = Net.Layers[i];

                for (int j = 0; j < currLayer.Neurons.Length; j++)
                {
                    Neuron neuron = currLayer.Neurons[j];
                    for (int k = 0; k < neuron.Dendrites.Count; k++)
                    {
                        neuron.Dendrites[k].WeightUpdate += learningRate * neuron.Delta * neuron.Dendrites[k].Previous.Output;
                    }
                }
            }
        }

        private void ApplyAndClearUpdates()
        {
            for (int l = 0; l < Net.Layers.Length; l++)
            {
                for(int n = 0; n < Net[l].Neurons.Length; n++)
                {
                    Net[l][n].ApplyAndClearWeightUpdates();
                }
            }
        }
    }
    public struct ErrorInfo
    {
        public double Average { get; set; }
        public double Total { get; set; }
        public ErrorInfo(double average, double total)
        {
            Average = average;
            Total = total;
        }
    }
}
