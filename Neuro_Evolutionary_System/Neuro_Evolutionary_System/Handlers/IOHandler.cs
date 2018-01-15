using System;
using System.IO;
using System.Linq;
using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System.Handlers
{
	public static class IoHandler
	{
		private static readonly string[] TerminationExpressions = {"quit", "stop", "exit", "terminate", "q"};
		public const int MaxIterations = 10000;
		public const double MinError = 1;

		public static InputData GetParameters(string[] args)
		{
			InputData data;

			switch (args.Length)
			{
				case 0:
					data = UserInput();
					break;
				case 2:
					data = HandleArgs(args);
					break;
				default:
					ErrorHandler.TerminateExecution(ErrorCode.InvalidNumInputParameters);
					data = null;
					break;
			}
			return data;
		}

		/// <summary>
		/// Special method that ensures correct input parameters for specified genetic algorithm from user
		/// </summary>
		/// <returns>Struct defining input parameters</returns>
		private static InputData UserInput()
		{
			InputData inputData = new InputData();
			InputData settingsData = new InputData();
			NotifyUserOfTermination();
			Console.WriteLine("Filename, including path, of test data.");
			inputData.SetTestDataFileName(AskForFileName());
			Console.WriteLine("Filename, including path, of settings data.");
			inputData.SetSettingsFileName(AskForFileName());
			return inputData;
		}

		/// <summary>
		/// Special method that ensures correct input parameters for specified genetic algorithm from console arguments
		/// </summary>
		/// <param name="args">Console arguments</param>
		/// <returns>Struct defining input parameters</returns>
		private static InputData HandleArgs(string[] args)
		{
			InputData inputData = new InputData();

			if (!File.Exists(args[0]))
				ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, args[0]);
			inputData.SetTestDataFileName(args[0]);

			if (!File.Exists(args[1]))
				ErrorHandler.TerminateExecution(ErrorCode.NoSuchFile, args[1]);
			inputData.SetTestDataFileName(args[0]);
			return inputData;
		}

		private static string AskForFileName()
		{
			string result;
			bool correctInput = false;
			do
			{
				result = AskForInput<string>();
				if (!File.Exists(result))
				{
					Console.WriteLine("Such file does not exist.");
					continue;
				}
				correctInput = true;
			} while (!correctInput);
			return result;
		}

		private static T AskForInput<T>() where T : IConvertible
		{
			bool correctInput;
			T result;
			do
			{
				string input = Console.ReadLine();
				CheckIfTerminating(input);
				correctInput = TryParse(input, out result);
			} while (!correctInput);
			return result;
		}

		private static T AskForInput<T>(T defaultValue) where T : IConvertible
		{
			string input = Console.ReadLine();
			CheckIfTerminating(input);
			bool correctInput = TryParse(input, out T result);
			if (!correctInput)
				result = defaultValue;

			return result;
		}

		private static bool TryParse<T>(string input, out T thisType) where T: IConvertible
		{
			bool success;
			thisType = typeof(T) == typeof(String) ? (T)(object)String.Empty : default(T);
			if (thisType == null)
				return false;

			var typeCode = thisType.GetTypeCode();

			switch (typeCode)
			{
				case TypeCode.Boolean:
					success = Boolean.TryParse(input, out var b);
					thisType = (T)Convert.ChangeType(b, typeCode);
					break;
				case TypeCode.Double:
					success = double.TryParse(input, out var d);
					thisType = (T)Convert.ChangeType(d, typeCode);
					break;
				case TypeCode.Single:
					success = float.TryParse(input, out var f);
					thisType = (T)Convert.ChangeType(f, typeCode);
					break;
				case TypeCode.Int32:
					success = int.TryParse(input, out var i);
					thisType = (T)Convert.ChangeType(i, typeCode);
					break;
				case TypeCode.String:
					success = true;
					thisType = (T)Convert.ChangeType(input, typeCode);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return success;
		}

		private static void NotifyUserOfTermination()
		{
			Console.Write("If you wish to terminate program, type one of following expressions: ");
			foreach (var expression in TerminationExpressions)
			{
				if(expression != null && expression !=  (string) TerminationExpressions.GetValue(TerminationExpressions.Length - 1))
					Console.Write(expression + ", ");
				else
					Console.WriteLine(expression + ".");
			}
		}

		private static void CheckIfTerminating(string input)
		{
			if(TerminationExpressions.Contains(input))
				ErrorHandler.TerminateExecution(ErrorCode.UserTermination);
		}

		private static void SetVariable(string pertinentArgument, out bool inputVariable)
		{
			if (!TryParse(pertinentArgument, out bool boolean))
				ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, pertinentArgument);
			inputVariable = boolean;
		}

		private static void SetVariable(string pertinentArgument, out int inputVariable)
		{
			if (!TryParse(pertinentArgument, out int integer))
				ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, pertinentArgument);
			inputVariable = integer;
		}

		private static void SetVariable(string pertinentArgument, out float inputVariable)
		{
			if (!TryParse(pertinentArgument, out float floating))
				ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, pertinentArgument);
			inputVariable = floating;
		}

		private static void SetVariable(string pertinentArgument, out double inputVariable)
		{
			if (!TryParse(pertinentArgument, out double precise))
				ErrorHandler.TerminateExecution(ErrorCode.InvalidInputParameter, pertinentArgument);
			inputVariable = precise;
		}
	}
}