using Neuro_Evolutionary_System.Handlers;

namespace Neuro_Evolutionary_System.Structures
{
	public enum AlgorithmType
	{
		Generation,
		Elimination
	}

	public static class DecodeAlgorithmType
	{
		public static AlgorithmType? Decode(string choice)
		{
			switch (choice)
			{
				case "e":
				case "E":
				case "Elimination":
				case "Eliminating":
					return AlgorithmType.Elimination;
				case "g":
				case "G":
				case "Generation":
				case "Generational":
					return AlgorithmType.Generation;
				default:
					ErrorHandler.TerminateExecution(ErrorCode.WrongAlgorithmChoice, choice);
					break;
			}
			return null;
		}
	}

	public enum GaVariables
	{
		Mortality,
		PopulationSize,
		MutationProbability,
		MutationProbabilities,
		MaxNoChange,
		MaxIter,
		MinError,
		Sigmas,
		Probabilities,
		Architecture
	}

	public class GaSettings
	{
		public AlgorithmType Type { get; private set; }
		public double Mortality { get; private set; }
		public int PopulationSize { get; private set; }
		public double MutationProbability { get; private set; }
		public double[] MutationProbabilities { get; private set; }
		public double MaxNoChange { get; private set; }
		public double MaxIter { get; private set; }
		public double MinError { get; private set; }
		public double[] Sigmas { get; private set; }
		public double[] Probabilities { get; private set; }
		public int[] Architecture { get; private set; }
		public int TotalParams { get; private set; }

		public GaSettings()
		{
			Type = AlgorithmType.Elimination;
			Mortality = 0.5;
			PopulationSize = 100;
			MutationProbability = 0.01;
			MaxIter = 20000;
			MaxNoChange = 1000;
			MinError = 0.0000001;
			SetSigmas(new []{1.0, 1.0});
			SetMutationProbabilities(new []{0.04, 0.04});
			SetProbablities(new []{0.6});
			SetArchitecture(new []{2,8,3});
		}

		public void SetAlgorithmType(AlgorithmType type)
		{
			Type = type;
		}

		public void SetMortality(double mortality)
		{
			Mortality = mortality;
		}

		public void SetPopulationSize(int populationSize)
		{
			PopulationSize = populationSize;
		}

		public void SetMutationProbability(double mutationProbability)
		{
			MutationProbability = mutationProbability;
		}

		public void SetMutationProbabilities(double[] mutationProbabilities)
		{
			MutationProbabilities = mutationProbabilities;
		}

		public void SetMaxNoChange(int maxNoChange)
		{
			MaxNoChange = maxNoChange;
		}
		
		public void SetMaxIter(int maxIter)
		{
			MaxIter = maxIter;
		}

		public void SetSigmas(double[] sigmas)
		{
			if (Probabilities != null && (Probabilities.Length < sigmas.Length - 1 || Probabilities.Length > sigmas.Length))
				ErrorHandler.TerminateExecution(ErrorCode.SigmaProbabilityMismatch);
			Sigmas = sigmas;
		}

		public void SetProbablities(double[] probablilities)
		{
			if (Sigmas != null && (Sigmas.Length < probablilities.Length || Sigmas.Length - 1 > probablilities.Length))
				ErrorHandler.TerminateExecution(ErrorCode.SigmaProbabilityMismatch);
			Probabilities = probablilities;
		}

		public void SetMinError(double minError)
		{
			MinError = minError;
		}

		public void SetArchitecture(int[] architecture)
		{
			TotalParams = architecture[1] * architecture[0] * 2;
			Architecture = architecture;
			for (int i = 2; i < architecture.Length; i++)
				TotalParams += architecture[i] * (architecture[i - 1] + 1);
		}
	}	
}