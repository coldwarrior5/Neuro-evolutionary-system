using System.IO;

namespace Neuro_Evolutionary_System.Handlers
{
	public enum Extension
	{
		ResultExtension,
		NetworkConfigurationExtension
	}
	
    public static class FileHandler
    {
	    private const string OutputFolder = "Results/";

		private static class Extensions
	    {
		    private const string ResultExtension = ".res";
		    private const string NetworkConfigurationExtension = ".res";

		    public static string GetExtension(Extension extension)
		    {
			    string result = "";
			    switch (extension)
			    {
				    case Extension.ResultExtension:
					    result = ResultExtension;
					    break;
				    case Extension.NetworkConfigurationExtension:
					    result = NetworkConfigurationExtension;
					    break;
			    }
			    return result;
		    }
	    }

		public static string[] ReadFile(string fileName)
        {
			if(File.Exists(fileName))
				return File.ReadAllLines(fileName);
	        return null;
        }

        public static void SaveFile(string fileName, Extension extension, string[] outputBuffer)
        {
	        fileName = Path.Combine(OutputFolder, fileName);
	        fileName += Extensions.GetExtension(extension);
	        
			if (!(outputBuffer is null))
                File.WriteAllLines(fileName, outputBuffer);
        }
    }
}