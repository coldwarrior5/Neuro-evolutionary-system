using System;

namespace Neuro_Evolutionary_System.ANN.Functions
{
	public static class Error
	{
		public static double Evaluate(double[][] givenOutputs, double[][] expectedOutputs)
		{
			if (givenOutputs.Length != expectedOutputs.Length)
				throw new Exception("Arrays must be of same size!");

			double sum = 0;
			for (int i = 0; i < givenOutputs.Length; i++)
			{
				sum += EvaluateSingleSample(givenOutputs[i], expectedOutputs[i]);
			}
			return sum / givenOutputs.Length;
		}

		private static double EvaluateSingleSample(double[] givenOutputs, double[] expectedOutputs)
		{
			if(givenOutputs.Length != expectedOutputs.Length)
				throw new Exception("Arrays must be of same size!");

			double sum = 0;
			for (int i = 0; i < givenOutputs.Length; i++)
			{
				sum += Math.Pow(expectedOutputs[i] - givenOutputs[i], 2);
			}
			return sum;
		}
	}
}