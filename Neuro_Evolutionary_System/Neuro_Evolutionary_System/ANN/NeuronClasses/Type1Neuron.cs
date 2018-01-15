using System;
using Neuro_Evolutionary_System.ANN.Functions;
using Neuro_Evolutionary_System.ANN.Interfaces;

namespace Neuro_Evolutionary_System.ANN.NeuronClasses
{
	public class Type1Neuron : INeuron
	{
		public int InputSize { get; }
		private readonly IAdaptingFunction _function;

		public Type1Neuron(int inputSize)
		{
			InputSize = inputSize;
			_function = new Similarity(inputSize);
		}

		public double GetOutput(double[] input, int position)
		{
			if (input.Length != InputSize)
				throw new ArgumentException("This neuron accepts exactly " + InputSize + " numbers.");
			
			return _function.ValueAt(input);
		}

		public void SetWeights(double[] newWeights)
		{
			_function.SetWeights(newWeights);
		}

		public double[] GetWeights()
		{
			return _function.GetWeights();
		}
	}
}