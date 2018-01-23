using System;
using Neuro_Evolutionary_System.ANN.Functions;
using Neuro_Evolutionary_System.ANN.Interfaces;

namespace Neuro_Evolutionary_System.ANN.NeuronClasses
{
	public class Type2Neuron : INeuron
	{
		private int InputSize { get; }
		private double[] _weights;
		private IActivationFunction _function;

		public Type2Neuron(int inputSize)
		{
			InputSize = inputSize;
			if (inputSize != 0) InitWeights();
			ChangeFunction(new Sigmoid());
		}

		public Type2Neuron(int inputSize, IActivationFunction function)
		{
			InputSize = inputSize;
			if (inputSize != 0) InitWeights();
			ChangeFunction(function);
		}

		private void InitWeights()
		{
			_weights = new double[InputSize + 1];
			ZeroWeights();
		}

		private void ZeroWeights()
		{
			for (var i = 0; i < InputSize + 1; i++)
				_weights[i] = 0;
		}

		public void ChangeFunction(IActivationFunction function)
		{
			_function = function;
		}

		public double GetOutput(double[] input)
		{
			if (input.Length != InputSize)
				throw new ArgumentException(@"Array input must have exact number of elements.");

			double sum = _weights[0];
			for (int i = 0; i < input.Length; i++)
			{
				sum += _weights[i + 1] * input[i];
			}
			return _function.ValueAt(sum);
		}

		public void SetWeights(double[] newWeights)
		{
			if (newWeights.Length != InputSize + 1)
				throw new ArgumentException(@"Array newWeights must have exact number of elements.");
			for (int i = 0; i < InputSize + 1; i++)
				_weights[i] = newWeights[i];
		}

		public double[] GetWeights()
		{
			return _weights;
		}
	}
}