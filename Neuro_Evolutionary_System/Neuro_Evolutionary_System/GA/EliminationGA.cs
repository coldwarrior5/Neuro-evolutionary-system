using System;
using System.Collections.Generic;
using Neuro_Evolutionary_System.Handlers;
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
			int cursorPosition = Console.CursorTop;
			int i = 0;
			int lastFound = 0;
			int howManyDies = (int)(Settings.Mortality * Settings.PopulationSize);
			Genome lastBest = new Genome(null);
			RandomPopulation();
			Console.Write(i + " iteration. Current best: ");
			Program.PrintParameters(BestGenome.Genes);
			Console.Write("with fitness: " + BestGenome.Fitness.ToString("G10"));

			while (BestGenome.Fitness > Settings.MinError && i++ - lastFound < Settings.MaxNoChange && i < Settings.MaxIter)
			{
				lastBest.Copy(BestGenome);
				for (int j = 0; j < howManyDies; j++)
				{
					ThreeTournament(j);
				}
				//Parallel.For(0, howManyDies, ThreeTournament);
				DetermineBestFitness();
				if (!(BestGenome.Fitness < lastBest.Fitness)) continue;
				lastFound = i;
				IoHandler.ClearCurrentConsoleLine(cursorPosition);
				Console.Write(i + " iteration. Current best: ");
				Program.PrintParameters(BestGenome.Genes);
				Console.Write("with fitness: " +  BestGenome.Fitness.ToString("G10"));
			}
			return BestGenome;
		}

		private void ThreeTournament(int index)
		{
			Random rnd = new Random(index + DateTime.Now.Millisecond);
			List<int> choices = new List<int>(3);
			while (true)
			{
				int randNum = rnd.Next(0, Settings.PopulationSize);
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
			if (rnd.NextDouble() < Settings.MutationProbability)
			{
				for (int i = 0; i < temp.Genes.Length; i++)
					Mutation(ref temp, i);
			}
			
			DetermineGenomeFitness(ref temp);
			order[2].Copy(temp);
		}
	}
}