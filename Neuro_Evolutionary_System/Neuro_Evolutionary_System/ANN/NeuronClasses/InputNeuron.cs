using Neuro_Evolutionary_System.ANN.Functions;
using Neuro_Evolutionary_System.ANN.Interfaces;

namespace Neuro_Evolutionary_System.ANN.NeuronClasses
{
	public class InputNeuron : INeuron
	{
		public int InputSize { get; }
		private readonly IActivationFunction _function;

		public InputNeuron()
		{
			InputSize = 1;
			_function = new Adaline();
		}

		public double GetOutput(double[] input, int position)
		{
			return _function.ValueAt(input[position]);
		}

		public void SetWeights(double[] newWeights)
		{
			
		}

		public double[] GetWeights()
		{
			return null;
		}
	}
}