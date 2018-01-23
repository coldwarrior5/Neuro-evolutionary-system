using System.Collections.Generic;
using System.Text.RegularExpressions;
using Neuro_Evolutionary_System.Interfaces;
using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System.Handlers
{
    public class Parser : IParser
    {
        private const string Comment = "%%";
        private const string InstanceInfo = "% ";
        private const string EmptyLine = "";

	    private const string Algorithm = "Algorithm type";
	    private const string Mortality = "Mortality";
	    private const string MutationProbabilitiy = "Mutation probability";
		private const string PopulationSize = "Population size";
	    private const string MaxIter = "Maximum number of iterations";
		private const string MaxBetween = "Number of iteration since last found solution";
	    private const string MinimumError = "Minimum desired error";
	    private const string Sigmas = "Sigmas";
	    private const string MutationProbabilities = "Sigma mutation probabilities";
		private const string SigmaProbabilities = "Sigma probabilities";
	    private const string Architecture = "Architecture";

		private readonly InputData _data;
		private List<Sample> _samples;
	    private string[] _classes;
	    private Classes _classHandler;


		public Parser(InputData data)
        {
	        _data = data;
        }
        
        public void ParseData(out GaSettings settings, out Instance instance)
        {
	        settings = ParseSettings();
	        _classHandler = new Classes(settings.Architecture[0], settings.Architecture[settings.Architecture.Length - 1]);
			_samples = new List<Sample>();
			var data = FileHandler.ReadFile(_data.TestDataFileName);
	        
			for (var i = 0; i < data.Length; i++)
                ParseLine(data[i], i + 1);
	        _classes = _classHandler.DefinedClasses();
            instance = new Instance(_samples.ToArray(), _classes);
        }

	    public GaSettings ParseSettings()
	    {
			GaSettings settings = new GaSettings();
		    var data = FileHandler.ReadFile(_data.SettingsFileName);
			if(data is null)
				return settings;

			foreach (string t in data)
			{
				if (ParseLine(t, out double[] result, out GaVariables type))
				{
					switch (type)
					{
						case GaVariables.MaxIter:
							settings.SetMaxIter((int)result[0]);
							break;
						case GaVariables.MaxNoChange:
							settings.SetMaxNoChange((int)result[0]);
							break;
						case GaVariables.Mortality:
							settings.SetMortality(result[0]);
							break;
						case GaVariables.PopulationSize:
							settings.SetPopulationSize((int)result[0]);
							break;
						case GaVariables.MutationProbabilities:
							settings.SetMutationProbabilities(result);
							break;
						case GaVariables.MinError:
							settings.SetMinError(result[0]);
							break;
						case GaVariables.Sigmas:
							settings.SetSigmas(result);
							break;
						case GaVariables.Probabilities:
							settings.SetProbablities(result);
							break;
						case GaVariables.Architecture:
							int[] intField = new int[result.Length];
							for (int i = 0; i < result.Length; i++)
								intField[i] = (int)result[i];
							settings.SetArchitecture(intField);
							break;
						case GaVariables.MutationProbability:
							settings.SetMutationProbability(result[0]);
							break;
					}
				}
				else if (t.Contains(Algorithm))
				{
					var split = t.Split(' ');
					AlgorithmType? gaType = DecodeAlgorithmType.Decode(split[split.Length - 1]);
					if(gaType != null)
						settings.SetAlgorithmType((AlgorithmType) gaType);
				}
			}
		    return settings;
	    }

		public void FormatAndSaveResult(string filename, Solution result)
	    {
			FileHandler.SaveFile(filename, Extension.ResultExtension, FormatData(result));
	    }

	    private static string[] FormatData(Solution input)
	    {
	        string[] result = new string[input.Size];
            if (input.GetTests() is null || input.GetMachines() is null || input.GetTimes() is null)
                return null;
                
	        string[] tests = input.GetTests();
	        string[] machines = input.GetMachines();
	        int[] times = input.GetTimes();
	        
	        for (var i = 0; i < input.Size; i++)
	            result[i] = "'" + tests[i] + "'," + times[i] + ",'" + machines[i] + "'.";
		    return result;
	    }

	    private void ParseLine(string line, int position)
        {
	        var matches = Regex.Matches(line, "[0-9.]+");
	        List<string> values = new List<string>();
			var instances = matches.GetEnumerator();
	        while (instances.MoveNext())
		        values.Add(instances.Current.ToString());
			if(values.Count != _classHandler.NumClasses + _classHandler.NumVariables)
				ErrorHandler.TerminateExecution(ErrorCode.NotEnoughParameters, position.ToString());
			double[] variables = new double[_classHandler.NumVariables];
	        double[] classes = new double[_classHandler.NumClasses];
			for (var i = 0; i < values.Count; i++)
	        {
		        bool success = double.TryParse(values[i], out double temp);
		        if (!success)
			        ErrorHandler.TerminateExecution(ErrorCode.ImproperLine, position.ToString());
		        if (i >= _classHandler.NumVariables)
			        classes[i - _classHandler.NumVariables] = temp;
		        else
			        variables[i] = temp;
	        }
			_samples.Add(new Sample(variables, classes));
		}

	    private bool ParseLine(string line, out double[] result, out GaVariables settings)
	    {
		    settings = GaVariables.MaxNoChange;
		    result = null;
			if (line.StartsWith(Comment) || line is EmptyLine)
			    return false;
		    if (line.StartsWith(InstanceInfo))
		    {
				var matches = Regex.Matches(line, "[0-9.]+");
				if (line.Contains(Mortality) || line.Contains(MinimumError) || line.Contains(MutationProbabilitiy))
			    {
				    if(line.Contains(Mortality))
						settings = GaVariables.Mortality;
				    else if (line.Contains(MutationProbabilitiy))
					    settings = GaVariables.MutationProbability;
				    else
					    settings = GaVariables.MinError;

					var success = double.TryParse(matches[0].Value, out var tempResult);
				    result = new[] { tempResult };
				    return success;
			    }
			    if (line.Contains(MaxBetween) || line.Contains(PopulationSize) || line.Contains(MaxIter))
			    {
				    if (line.Contains(MaxBetween))
					    settings = GaVariables.MaxNoChange;
				    else if (line.Contains(PopulationSize))
					    settings = GaVariables.PopulationSize;
				    else
					    settings = GaVariables.MaxIter;

					var success = int.TryParse(matches[0].Value, out var tempResult);
					result = new double[] { tempResult };
					return success;
			    }
			    if(line.Contains(Sigmas) || line.Contains(SigmaProbabilities) || line.Contains(Architecture) || line.Contains(MutationProbabilities))
			    {
				    var success = false;
				    if (line.Contains(Sigmas))
					    settings = GaVariables.Sigmas;
				    else if (line.Contains(SigmaProbabilities))
					    settings = GaVariables.Probabilities;
					else if (line.Contains(MutationProbabilities))
					    settings = GaVariables.MutationProbabilities;
				    else
					    settings = GaVariables.Architecture;
				    var values = new List<string>();
					var instances = matches.GetEnumerator();
				    while (instances.MoveNext())
					    values.Add(instances.Current.ToString());
					result = new double[values.Count];
					for (var i = 0; i < values.Count; i++)
				    {
					    success = double.TryParse(values[i], out result[i]);
					    if (!success)
						    break;
				    }
				    return success;
				}
		    }
		    return false;
	    }
	}
}