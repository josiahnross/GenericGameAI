using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions
{
    public class TanH : ActivationFunction
    {
        double min;
        double max;

        public override bool CanUseOutputDerivative => true;

        public override double Min { get { return min; } protected set { min = value; } }

        public override double Max { get { return max; } protected set { max = value; } }

        public TanH(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public override double Function(double x)
        {
            double eToX = Math.Pow(Math.E, x);
            double eToNegX = Math.Pow(Math.E, -x);
            return (eToX - eToNegX) / (eToX + eToNegX);
        }

        public override double Derivative(double x)
        {
            return 1 - Math.Pow(Function(x), 2);
        }

        public override double OutputDerivative(double y)
        {
            return 1 - Math.Pow(y,2);
        }

        public override ActivationFunction Copy()
        {
            return new TanH(Min, Max);
        }
    }
}
