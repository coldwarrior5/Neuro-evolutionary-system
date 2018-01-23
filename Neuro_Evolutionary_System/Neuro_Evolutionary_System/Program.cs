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
            Console.WriteLine("Genetic algorithm\n");
	        Start(args);
	        Console.WriteLine("\nApplication finished execution");
        }

	    static void Start(string[] args)
	    {
		    InputData inputData = IoHandler.GetParameters(args);
			IParser parser = new Parser(inputData);
		    GaSettings settings = parser.ParseSettings();
		    Ga algorithm ;
		    switch (settings.Type)
		    {
				case AlgorithmType.Generation:
					algorithm = new GenerationGa(parser);
					break;
				case AlgorithmType.Elimination:
					algorithm = new EliminationGa(parser);
					break;
			    default:
				    throw new ArgumentOutOfRangeException();
		    }
		    Genome bestSolution = algorithm.Start();
		    Console.WriteLine("\n____________Proces is finished____________\n");
		    if (bestSolution != null)
		    {
			    Console.Write("Best result is: " + bestSolution.Fitness + ", with parameters: ");
			    PrintParameters(bestSolution.Genes);
			}
		    Console.WriteLine("\n____________Classification____________\n");
		    algorithm.Test(out double[][] givenClasses, out double[][]expectedOutput);
			bool[] correct = new bool[expectedOutput.Length];
		    for (int i = 0; i < expectedOutput.Length; i++)
		    {
			    correct[i] = true;
			    Console.Write("Expected classes: ");
			    for (int j = 0; j < expectedOutput[i].Length; j++)
			    {
				    Console.Write(expectedOutput[i][j].ToString("G0") + " ");
			    }
			    Console.Write("\tGiven classes: ");
			    for (int j = 0; j < givenClasses[i].Length; j++)
			    {
				    Console.Write(givenClasses[i][j].ToString("G0") + " ");
				    if (!(Math.Abs(expectedOutput[i][j] - givenClasses[i][j]) < double.Epsilon))
						correct[i] = false;
				    
				    
			    }
			    Console.WriteLine(correct[i] ? "\tCorrect classification." : "\tIncorrect classification.");
		    }
		    int correctlyClassified = 0;
		    int incorrectlyClassified = 0;
		    for (int i = 0; i < correct.Length; i++)
		    {
			    if (correct[i])
				    correctlyClassified++;
			    else
				    incorrectlyClassified++;
		    }
			Console.WriteLine("Correctly classified samples: " + correctlyClassified + ", incorrectly classified: " + incorrectlyClassified);
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
