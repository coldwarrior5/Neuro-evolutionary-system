using System;

namespace Neuro_Evolutionary_System.Handlers
{
    public enum ErrorCode
    {
		EarlyExit = -1,
		UserTermination = 0,
        InvalidNumInputParameters,
        InvalidInputParameter,
		SigmaProbabilityMismatch,
        NoSuchFile,
		NoFileGiven,
	    WrongAlgorithmChoice,
        ImproperLine,
	    NotEnoughParameters
    }
    
    public static class ErrorHandler
    {
        public static void TerminateExecution(ErrorCode code, string explanation = "")
        {
	        if (code < 0)
	        {
				Console.WriteLine(explanation);
		        code = 0;
	        }
			else
				Console.WriteLine("\nApplication stopped.\n  Reason: " + ErrorMessage(code) + " " + explanation);
            Environment.Exit((int) code);
        }

        private static string ErrorMessage(ErrorCode code)
        {
            string explanation;
            
            switch (code)
            {
                case ErrorCode.InvalidNumInputParameters:
                    explanation = "Invalid number of input parameters.";
                    break;
                case ErrorCode.UserTermination:
                    explanation = "User has terminated the application.";
                    break;
                case ErrorCode.NoSuchFile:
                    explanation = "File not found.";
                    break;
                case ErrorCode.InvalidInputParameter:
                    explanation = "Invalid input parameter.";
                    break;
                case ErrorCode.ImproperLine:
                    explanation = "File contains irregular line.";
                    break;
	            case ErrorCode.EarlyExit:
		            explanation = "";
		            break;
	            case ErrorCode.NoFileGiven:
					explanation = "Filename was not defined.";
		            break;
	            case ErrorCode.WrongAlgorithmChoice:
		            explanation = "Such algorithm does not exist.";
					break;
	            case ErrorCode.SigmaProbabilityMismatch:
		            explanation = "There needs to be one less or exactly right probabilities as are sigmas.";
		            break;
	            case ErrorCode.NotEnoughParameters:
		            explanation = "Not enough parameters in a line.";
					break;
	            default:
                    throw new ArgumentException("Such error is non existant.");
            }
            return explanation;
        }
    }
}