using System;

namespace Neuro_Evolutionary_System.GA
{
	public static class FitnessFunctions
	{
		public static double Fitness1(double[] desiredOutput, double[] givenOutput)
		{
			double error = 0;

			if(desiredOutput.Length != givenOutput.Length)
				throw new Exception("Arrays of given output is different size than that of the desired output");

			// ReSharper disable once LoopCanBeConvertedToQuery Speed is of essence here
			for (int i = 0; i < desiredOutput.Length; i++)
			{
				error += Math.Pow(desiredOutput[i] - givenOutput[i], 2);
			}
			return error/desiredOutput.Length;
		}
	}
}