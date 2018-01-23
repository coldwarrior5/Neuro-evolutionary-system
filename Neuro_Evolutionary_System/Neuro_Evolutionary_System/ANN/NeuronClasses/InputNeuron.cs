using System;
using Neuro_Evolutionary_System.ANN.Functions;
using Neuro_Evolutionary_System.ANN.Interfaces;

namespace Neuro_Evolutionary_System.ANN.NeuronClasses
{
	public class InputNeuron : INeuron
	{
		private int InputSize { get; }
		private readonly IActivationFunction _function;

		public InputNeuron()
		{
			InputSize = 1;
			_function = new Adaline();
		}

		public double GetOutput(double[] input)
		{
			if (input.Length != InputSize)
				throw new ArgumentException(@"Array input must have exact number of elements.");
			return _function.ValueAt(input[0]);
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