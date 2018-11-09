using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions
{
    public class IdentityActivationFunction : ActivationFunction
    {
        double min;
        double max;

        public override bool CanUseAbstractDerivative => false;

        public override double Min => min;

        public override double Max => max;

        public IdentityActivationFunction(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public override double Derivative(double x)
        {
            return 1;
        }

        public override double Function(double x)
        {
            return x;
        }

        public override double OutputDerivative(double y)
        {
            throw new NotImplementedException();
        }
    }
}
