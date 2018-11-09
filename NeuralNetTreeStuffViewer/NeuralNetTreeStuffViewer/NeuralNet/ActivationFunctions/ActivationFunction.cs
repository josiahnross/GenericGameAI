using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions
{
    public abstract class ActivationFunction
    {
        public abstract bool CanUseAbstractDerivative { get; }
        public abstract double Min { get; }
        public abstract double Max { get; }
        public abstract double Function(double x);
        public abstract double Derivative(double x);
        public abstract double OutputDerivative(double y);
    }
}
