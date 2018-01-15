using System;
using System.Threading.Tasks;
using Neuro_Evolutionary_System.Interfaces;

namespace Neuro_Evolutionary_System.GA
{
	public class GenerationGa : Ga
	{
		private readonly Genome[] _tempPopulation;

		public GenerationGa(IParser parser) : base(parser)
		{
			Population = new Genome[Settings.PopulationSize];
			_tempPopulation = new Genome[Settings.PopulationSize];
			RandomPopulation(Settings.TotalParams, _tempPopulation);
		}

		public override Genome Start()
		{
			int i = 0;
			int lastFound = 0;
			Genome lastBest = new Genome(null);
			RandomPopulation();
			Console.Write(i + " iteration. Current best: ");
			Program.PrintParameters(BestGenome.Genes);
			Console.WriteLine("with fitness: " + BestGenome.Fitness.ToString("G10"));
			while (BestGenome.Fitness > Settings.MinError && lastFound < Settings.MaxIter)
			{
				lastBest.Copy(BestGenome);
				_tempPopulation[0].Copy(BestGenome);
				double unused = CalculateFitness();
				Parallel.For(1, Settings.PopulationSize, SingleThread);
				SwapBuffers();
				DeterminePopulationFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness)) continue;
				lastFound = i;
				Console.Write(i + " iteration. Current best: ");
				Program.PrintParameters(BestGenome.Genes);
				Console.WriteLine("with fitness: " + BestGenome.Fitness.ToString("G10"));
			}
			return BestGenome;
		}

		private void SingleThread(int index)
		{
			Random rand = new Random(index + DateTime.Now.Millisecond);
			int firstParentId = RouletteWheelSelection(rand);
			int secondParentId = RouletteWheelSelection(rand);
			while (secondParentId == firstParentId)
			{
				secondParentId = RouletteWheelSelection(rand);
			}
			

			Genome temp = new Genome(new double[5]);
			Crossover(Population[firstParentId], Population[secondParentId], ref temp);
			
			for (int i = 0; i < _tempPopulation[index].Genes.Length; i++)
			{
				if (rand.NextDouble() < Settings.MutationProbability)
					Mutation(ref _tempPopulation[index], i);
			}
			_tempPopulation[index].Copy(temp);
		}

		private void SwapBuffers()
		{
			Parallel.For(0, Settings.PopulationSize, i =>
			{
				Population[i].Copy(_tempPopulation[i]);
			});
		}

		private double CalculateFitness()
		{
			double sum = 0;
			for (int i = 0; i < Population.Length; i++)
			{
				sum += Population[i].Fitness;
			}
			return sum;
		}
	}
}