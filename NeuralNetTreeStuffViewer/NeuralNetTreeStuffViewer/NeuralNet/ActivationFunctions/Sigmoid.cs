using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions
{
    public class Sigmoid : ActivationFunction
    {
        double min;
        double max;

        public override bool CanUseOutputDerivative => true;

        public override double Min { get { return min; } protected set { min = value; } }

        public override double Max { get { return max; } protected set { max = value; } }

        public Sigmoid(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public override double Function(double x)
        {
            return 1 / (1 + Math.Pow(Math.E, -x));
        }

        public override double Derivative(double x)
        {
            return Function(x)*(1-Function(x));
        }

        public override double OutputDerivative(double y)
        {
            return y * (1 - y);
        }

        public override ActivationFunction Copy()
        {
            return new Sigmoid(Min, Max);
        }
    }
}
