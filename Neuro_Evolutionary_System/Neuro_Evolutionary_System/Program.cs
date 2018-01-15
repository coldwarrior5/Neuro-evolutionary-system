using System;
using Neuro_Evolutionary_System.GA;
using Neuro_Evolutionary_System.Handlers;
using Neuro_Evolutionary_System.Interfaces;
using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System
{
	static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Genetic algorithm");
	        Genome result = Start(args);
			Console.WriteLine("Proces is finished");
			Console.Write("Best result is: " + result.Fitness + ", with parameters: ");
			PrintParameters(result.Genes);
			Console.WriteLine("with fitness: " + result.Fitness.ToString("G10"));
        }

	    static Genome Start(string[] args)
	    {
		    InputData inputData = IoHandler.GetParameters(args);
			IParser parser = new Parser(inputData);
		    GaSettings settings = parser.ParseSettings();
		    Ga algorithm = null;
		    switch (settings.Type)
		    {
				case AlgorithmType.Generation:
					algorithm = new GenerationGa(parser);
					break;
				case AlgorithmType.Elimination:
					algorithm = new EliminationGa(parser);
					break;
			}
		    Genome bestSolution = algorithm?.Start();
			return bestSolution;
	    }

	    public static void PrintParameters(double[] param)
	    {
		    foreach (double f in param)
		    {
				Console.Write(f.ToString("G10") + ", ");
		    }
	    }
    }
}
