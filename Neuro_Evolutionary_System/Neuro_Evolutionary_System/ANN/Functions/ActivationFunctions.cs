using System;
using Neuro_Evolutionary_System.ANN.Interfaces;

// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace Neuro_Evolutionary_System.ANN.Functions
{
	public class Sigmoid : IActivationFunction
	{
		public double ValueAt(double x)
		{
			double k = Math.Exp(x);
			return k / (1.0f + k);
		}
	}

	public class Adaline : IActivationFunction
	{
		public double ValueAt(double x)
		{
			return x;
		}
	}

	public class Similarity : IAdaptingFunction
	{
		public int Size { get; }
		private readonly double[] _w;
		private readonly double[] _s;

		public Similarity(int size)
		{
			Size = size;
			_w = new double[Size];
			_s = new double[Size];
			ResetWeights();
		}

		public double ValueAt(double[] x)
		{
			if (x.Length != Size)
				throw new ArgumentException(@"Array x must have exact number of elements as there are parameters in similarity function.");

			double sum = 0;
			for (int i = 0; i < Size; i++)
				sum += Math.Abs(x[i] - _w[i]) / Math.Abs(_s[i]);
			return 1.0 / 1.0 + sum;
		}

		private void ResetWeights()
		{
			for (int i = 0; i < Size; i++)
			{
				_w[i] = 0;
				_s[i] = 1;
			}
		}

		public void SetWeights(double[] newWeights)
		{
			if (newWeights.Length != Size * 2)
				throw new ArgumentException(@"Array newWeights must have exact number of elements as there are parameters in similarity function.");
			for (int i = 0; i < Size * 2; i++)
			{
				if(i < Size)
					_w[i] = newWeights[i];
				else
					_s[i - Size] = newWeights[i];
			}
		}

		public double[] GetWeights()
		{
			double[] weights = new double[Size * 2];
			for (int i = 0; i < Size; i++)
			{
				weights[i] = _w[i];
				weights[Size + i] = _s[i];
			}
			return weights;
		}
	}
}