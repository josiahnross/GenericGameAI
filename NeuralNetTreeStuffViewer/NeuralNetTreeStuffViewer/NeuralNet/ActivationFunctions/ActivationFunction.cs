using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetTreeStuffViewer.NeuralNet.ActivationFunctions
{
    public abstract class ActivationFunction
    {
        static Dictionary<string, ActivationFunction> activationFunctionNames;
        public static void Init()
        {
            activationFunctionNames = new Dictionary<string, ActivationFunction>();
            activationFunctionNames.Add("IdentityActivationFunction", new IdentityActivationFunction(0, 0));
            activationFunctionNames.Add("Sigmoid", new Sigmoid(0, 0));
            activationFunctionNames.Add("TanH", new TanH(0, 0));
        }
        public abstract ActivationFunction Copy();
        public abstract bool CanUseOutputDerivative { get; }
        public abstract double Min { get; protected set; }
        public abstract double Max { get; protected set; }
        public abstract double Function(double x);
        public abstract double Derivative(double x);
        public abstract double OutputDerivative(double y);
        public ActivationFunctionInfo ActivationFunctionInfo { get { return new ActivationFunctionInfo(this); } }
        public static ActivationFunction ActivationFunctionFromInfo(ActivationFunctionInfo? info)
        {
            if(info == null)
            {
                return null;
            }
            var func = activationFunctionNames[info.Value.Name].Copy();
            func.Min = info.Value.Min;
            func.Max = info.Value.Max;
            return func;
        }
    }
    public struct ActivationFunctionInfo
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Name { get; set; }
        public ActivationFunctionInfo(ActivationFunction activationFunction)
        {
            Min = activationFunction.Min;
            Max = activationFunction.Max;
            Name = activationFunction.GetType().Name;
        }
    }
}
