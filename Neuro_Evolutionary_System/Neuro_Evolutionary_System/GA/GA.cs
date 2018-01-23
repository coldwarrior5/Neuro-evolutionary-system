using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuro_Evolutionary_System.ANN;
using Neuro_Evolutionary_System.ANN.Functions;
using Neuro_Evolutionary_System.Interfaces;
using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System.GA
{
	public abstract class Ga
	{
		protected readonly GaSettings Settings;
		private readonly Instance _instance;
		private double[][] _expectedOutput;

		protected Genome[] Population;
		protected readonly Genome BestGenome;
		private readonly Random _rand;
		private readonly NeuralNetwork _ann;

		protected Ga(IParser parser)
		{
			parser.ParseData(out Settings, out _instance);
			BestGenome = new Genome();
			_rand = new Random();
			_ann = new NeuralNetwork(Settings.Architecture);
			InitExpectedOutput();
		}

		private void InitExpectedOutput()
		{
			_expectedOutput = new double[_instance.Size][];
			for (int i = 0; i < _instance.Size; i++)
			{
				_expectedOutput[i] = new double[_instance.Classes.Length];
				for (int j = 0; j < _instance.Classes.Length; j++)
					_expectedOutput[i][j] = _instance.Samples[i].Classes[j];
			}
		}

		public void Test(out double[][] givenClasses, out double[][] expectedClasses)
		{
			expectedClasses = _expectedOutput;
			givenClasses = new double[_instance.Size][];
			var syncObject = new object();
			lock (syncObject)
				_ann.SetWeights(BestGenome.Genes);

			for (int i = 0; i < _instance.Size; i++)
			{
				givenClasses[i] = new double[_instance.Classes.Length];
				var output = _ann.GetOutput(_instance.Samples[i].Variables);
				for (int j = 0; j < _instance.Classes.Length; j++)
				{
					givenClasses[i][j] = output[j] < 0.5 ? 0 : 1;
				}
			}
		}

		public abstract Genome Start();

		protected void DeterminePopulationFitness()
		{
			for (int i = 0; i < Population.Length; i++)
			{
				DetermineGenomeFitness(ref Population[i]);
				if (Population[i].Fitness < BestGenome.Fitness)
					BestGenome.Copy(Population[i]);
			}
			/*
			Parallel.ForEach(Population, ()=> new Genome(), (genome, loopState, localState) =>
			{
				DetermineGenomeFitness(ref genome);
				return genome.Fitness < localState.Fitness ? genome : localState;
			},
			localState =>
			{
				lock (syncObject)
				{
					if(localState.Fitness < BestGenome.Fitness)
						BestGenome.Copy(localState);
				} 
			});
			*/
		}
		
		protected void DetermineBestFitness()
		{
			foreach (Genome t in Population)
			{
				if(t.Fitness < BestGenome.Fitness)
					BestGenome.Copy(t);
			}
		}

        protected void DetermineGenomeFitness(ref Genome genome)
		{
			double[][] givenOutput = new double[_instance.Size][];
			var genes = genome.Genes;
			_ann.SetWeights(genes);
			for (int i = 0; i < _instance.Size; i++)
			{
				givenOutput[i] = new double[_instance.Samples[i].Variables.Length];
				givenOutput[i] = _ann.GetOutput(_instance.Samples[i].Variables);
			}
			genome.Fitness = Error.Evaluate(givenOutput, _expectedOutput);
		}

		protected void Crossover(Genome first, Genome second, ref Genome child)
		{
			int which =_rand.Next(0, 3);
			switch (which)
			{
				case 0:
					CrossoverMethods.DiscreteRecombination(first, second, ref child, _rand);
					break;
				case 1:
					CrossoverMethods.BetterParent(first, second, ref child, _rand);
					break;
				case 2:
					CrossoverMethods.WholeArithmeticRecombination(first, second, ref child, _rand);
					break;
				default:
					child = null;
					break;
			}
		}
		
		protected void Mutation(ref Genome gene, int index)
		{
			bool first = _rand.NextDouble() < Settings.Probabilities[0];
			if (first)
				MutationMethods.SlightMutation(ref gene, index, Settings.Sigmas[0], Settings.MutationProbabilities[0], _rand);
			else
				MutationMethods.Replace(ref gene, index, Settings.Sigmas[1], Settings.MutationProbabilities[1], _rand);
		}

		protected int RouletteWheelSelection(Random rand)
		{
			double totalFitness = 0;

			foreach (Genome t in Population)
			{
				totalFitness += 1.0/t.Fitness;
			}
			double value = rand.NextDouble() * totalFitness;
			for (int i = 0; i < Population.Length; i++)
			{
				value -= 1.0/Population[i].Fitness;
				if (value <= 0)
					return i;
			}
			// When rounding errors occur, we return the last item's index 
			return Population.Length - 1;
		}

		// ReSharper disable once RedundantAssignment
		protected static void Order(List<Genome> order)
		{
			Genome temp;
			Genome temp2;
			double worstFitness = float.MinValue;
			int worstIndex = 2;
			double bestFitness = float.MaxValue;
			int bestIndex = 0;

			for (int i = 0; i < 3; i++)
			{
				if (order[i].Fitness > worstFitness)
				{
					worstFitness = order[i].Fitness;
					worstIndex = i;
				}

				if (order[i].Fitness < bestFitness)
				{
					bestFitness = order[i].Fitness;
					bestIndex = i;
				}
			}

			switch (bestIndex)
			{
				case 0 when worstIndex == 2:
					return;
				case 0:
					temp = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp;
					break;
				case 1 when worstIndex == 2:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					break;
				case 1:
					temp = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp;
					temp2 = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp2;
					break;
				case 2 when worstIndex == 0:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					break;
				case 2:
					temp = order[bestIndex];
					order[bestIndex] = order[0];
					order[0] = temp;
					temp2 = order[worstIndex];
					order[worstIndex] = order[2];
					order[2] = temp2;
					break;
			}
		}

		protected void RandomPopulation()
		{
			/*
			Parallel.For(0, Population.Length, i =>
			{
				double[] field = new double[Settings.TotalParams];
				for (int j = 0; j < Settings.TotalParams; j++)
				{
					do
					{
						field[j] = _rand.NextDouble() * 2 - 1;
					} while (field[j] < double.Epsilon);
				}
				Population[i] = new Genome(field);
			});
			*/
			for (int i = 0; i < Population.Length; i++)
			{
				double[] field = new double[Settings.TotalParams];
				for (int j = 0; j < Settings.TotalParams; j++)
				{
					do
					{
						field[j] = _rand.NextDouble() / 2;
					} while (field[j] < double.Epsilon);
				}
				Population[i] = new Genome(field);
			}
			DeterminePopulationFitness();
		}
		
		protected static void RandomPopulation(int paramSize, Genome[] population)
		{
			Random rand = new Random();
			Parallel.For(0, population.Length, i =>
			{
				double[] field = new double[paramSize];
				for (int j = 0; j < paramSize; j++)
				{
					do
					{
						field[j] = rand.NextDouble() /2;
					} while (field[j] < double.Epsilon);
				}
				population[i] = new Genome(field);
			});
		}
	}

	public static class CrossoverMethods
	{
		public static void DiscreteRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = Single.MaxValue;
			for (int i = 0; i < firstChild.Genes.Length; i++)
			{
				firstChild.Genes[i] = rand.NextDouble() < 0.5 ? firstParent.Genes[i] : secondParent.Genes[i];
			}
		}

		public static void SimpleArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			int location = rand.Next(0, firstChild.Genes.Length);
			firstChild.Copy(firstParent);
			firstChild.Fitness = Single.MaxValue;
			for (int i = location; i < firstParent.Genes.Length; i++)
			{
				firstChild.Genes[i] = Average(firstParent.Genes[i], secondParent.Genes[i]);
			}
		}

		public static void BetterParent(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Copy(firstParent);
		}

		public static void WholeArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			firstChild.Fitness = Single.MaxValue;
			for (int i = 0; i < firstParent.Genes.Length; i++)
			{
				firstChild.Genes[i] = Average(firstParent.Genes[i], secondParent.Genes[i]);
			}
		}

		private static double Average(double first, double second)
		{
			return (first + second) / 2;
		}
	}

	public static class MutationMethods
	{
		public static void SlightMutation(ref Genome gene, int index, double stdDev, double mutationProbability, Random rand)
		{
			if (rand.NextDouble() >= mutationProbability) return;
			double delta = NormalDistribution(rand, 0, stdDev);
			gene.Genes[index] += delta;
		}

		public static void Replace(ref Genome gene, int index, double stdDev, double mutationProbability, Random rand)
		{
			if (rand.NextDouble() >= mutationProbability) return;
			double delta = NormalDistribution(rand, 0, stdDev);
			gene.Genes[index] = delta;
		}

		private static double NormalDistribution(Random rand, double mean, double stdDev)
		{
			double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
			double u2 = 1.0 - rand.NextDouble();
			double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
			                       Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
			double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
			return randNormal;
		}
	}
}