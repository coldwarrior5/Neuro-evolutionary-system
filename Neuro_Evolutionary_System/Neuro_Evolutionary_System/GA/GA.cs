using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuro_Evolutionary_System.ANN;
using Neuro_Evolutionary_System.Interfaces;
using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System.GA
{
	public abstract class Ga
	{
		protected readonly GaSettings Settings;
		protected readonly Instance Instance;
		private double[] _expectedOutput;
		private double[] _inputs;

		protected Genome[] Population;
		protected readonly Genome BestGenome;
		protected readonly Random Rand;
		private NeuralNetwork _ann;

		protected Ga(IParser parser)
		{
			parser.ParseData(out Settings, out Instance);
			BestGenome = new Genome(new double[Settings.TotalParams]);
			Rand = new Random();
			_ann = new NeuralNetwork(Settings.Architecture);
			InitExpectedOutput();
		}

		private void InitExpectedOutput()
		{
			_inputs = new double[Instance.Size * Instance.Samples[0].Variables.Length];
			_expectedOutput = new double[Instance.Size * Instance.Classes.Length];
			int indexInput = 0;
			int indexClass = 0;
			for (int i = 0; i < Instance.Size; i++)
			{
				for (int j = 0; j < Instance.Classes.Length; j++)
				{
					_expectedOutput[indexClass++] = Instance.Samples[i].Classes[j];
				}
				for (int j = 0; j < Instance.Samples[0].Variables.Length; j++)
				{
					_expectedOutput[indexInput++] = Instance.Samples[i].Variables[j];
				}
			}
		}

		public abstract Genome Start();

		protected void DeterminePopulationFitness()
		{
			object syncObject = new object();

			Parallel.ForEach(Population, ()=> new Genome(new double[]{}), (genome, loopState, localState) =>
			{
				DetermineGenomeFitness(ref genome);
				return genome.Fitness > localState.Fitness ? genome : localState;
			},
			localState =>
			{
				lock (syncObject)
				{
					if(localState.Fitness > BestGenome.Fitness)
						BestGenome.Copy(localState);
				} 
			});
		}
		
		protected void DetermineBestFitness()
		{
			object syncObject = new object();
			foreach (Genome t in Population)
			{
				lock (syncObject)
				{
					if(t.Fitness > BestGenome.Fitness)
						BestGenome.Copy(t);
				}
			}
		}

        protected void DetermineGenomeFitness(ref Genome genome)
		{
			var genes = genome.Genes;
			_ann.SetWeights(genes);
			var givenOutput = _ann.GetOutput(_inputs);
			genome.Fitness = FitnessFunctions.Fitness1(_expectedOutput, givenOutput);
		}

		protected void Crossover(Genome first, Genome second, ref Genome child)
		{
			int which =Rand.Next(0, 4);
			switch (which)
			{
				case 0:
					CrossoverMethods.DiscreteRecombination(first, second, ref child, Rand);
					break;
				case 1:
					CrossoverMethods.SimpleArithmeticRecombination(first, second, ref child, Rand);
					break;
				case 2:
					CrossoverMethods.SingleArithmeticRecombination(first, second, ref child, Rand);
					break;
				case 3:
					CrossoverMethods.WholeArithmeticRecombination(first, second, ref child, Rand);
					break;
				default:
					child = null;
					break;
			}
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
				value -= 1.0 / Population[i].Fitness;
				if (value <= 0)
					return i;
			}
			// When rounding errors occur, we return the last item's index 
			return Population.Length - 1;
		}

		protected void Mutation(ref Genome gene, int index)
		{
			bool first = Rand.NextDouble() < Settings.Probabilities[0];
			double stdDev = first ? Settings.Sigmas[0] : Settings.Sigmas[1];
			MutationMethods.SlightMutation(ref gene, index, stdDev, Rand);
		}

		// ReSharper disable once RedundantAssignment
		protected static void Order(List<Genome> order)
		{
			Genome temp;
			Genome temp2;
			double worstFitness = float.MaxValue;
			int worstIndex = 2;
			double bestFitness = float.MinValue;
			int bestIndex = 0;

			for (int i = 0; i < 3; i++)
			{
				if (order[i].Fitness < worstFitness)
				{
					worstFitness = order[i].Fitness;
					worstIndex = i;
				}

				if (order[i].Fitness > bestFitness)
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
			Parallel.For(0, Population.Length, i =>
			{
				double[] field = new double[Settings.TotalParams];
				for (int j = 0; j < Settings.TotalParams; j++)
				{
					do
					{
						field[j] = Rand.NextDouble() * 2 - 1;
					} while (field[j] < Double.Epsilon);
				}
				Population[i] = new Genome(field);
			});
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
						field[j] = rand.NextDouble() * 2 - 1;
					} while (field[j] < Double.Epsilon);
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
				firstChild.Genes[i] = rand.NextDouble() > 0.5 ? firstParent.Genes[i] : secondParent.Genes[i];
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

		public static void SingleArithmeticRecombination(Genome firstParent, Genome secondParent, ref Genome firstChild, Random rand)
		{
			int location = rand.Next(0, firstChild.Genes.Length);
			firstChild.Copy(firstParent);
			firstChild.Fitness = Single.MaxValue;
			firstChild.Genes[location] = Average(firstParent.Genes[location], secondParent.Genes[location]);
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
			return first + (second - first) / 2;
		}
	}

	public static class MutationMethods
	{
		public static void SlightMutation(ref Genome gene, int index, double stdDev, Random rand)
		{
			double delta = NormalDistribution(rand, 0, stdDev);
			gene.Genes[index] += delta;
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