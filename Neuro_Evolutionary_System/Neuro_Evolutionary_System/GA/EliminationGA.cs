using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neuro_Evolutionary_System.Interfaces;

namespace Neuro_Evolutionary_System.GA
{
	public class EliminationGa : Ga
	{
		public EliminationGa(IParser parser) : base(parser)
		{
			Population = new Genome[Settings.PopulationSize];
		}

		public override Genome Start()
		{
			int i = 0;
			int lastFound = 0;
			int howManyDies = (int)(Settings.Mortality * Settings.PopulationSize);
			Genome lastBest = new Genome(null);
			RandomPopulation();
			Console.Write(i + " iteration. Current best: ");
			Program.PrintParameters(BestGenome.Genes);
			Console.WriteLine("with fitness: " + BestGenome.Fitness.ToString("G10"));
			while (BestGenome.Fitness > Settings.MinError && lastFound < Settings.MaxIter)
			{
				lastBest.Copy(BestGenome);
				Parallel.For(0, howManyDies, ThreeTournament);	// Mortality determines how many times we should do the Tournaments
				DetermineBestFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness)) continue;
				lastFound = i;
				Console.Write(i + " iteration. Current best: ");
				Program.PrintParameters(BestGenome.Genes);
				Console.WriteLine("with fitness: " +  BestGenome.Fitness.ToString("G10"));
			}
			return BestGenome;
		}

		private void ThreeTournament(int index)
		{
			Random rnd = new Random(index + DateTime.Now.Millisecond);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(1, Settings.PopulationSize);
				if (choices.Contains(randNum)) continue;
				choices.Add(randNum);
				if(choices.Count == 3)
					break;
			}

			List<Genome> order = new List<Genome>(3);
			for (int i = 0; i < 3; i++)
			{
				Genome choice = Population[choices[i]];
				order.Add(choice);
			}

			Genome temp = new Genome(order[2].Genes);
			Order(order);

			Crossover(order[0], order[1], ref temp);
			for (int i = 0; i < temp.Genes.Length; i++)
			{
				if (Rand.NextDouble() < Settings.MutationProbability)
					Mutation(ref temp, i);
			}
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
		}
	}
}