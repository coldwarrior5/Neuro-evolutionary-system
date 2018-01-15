using Neuro_Evolutionary_System.Structures;

namespace Neuro_Evolutionary_System.Interfaces
{
    public interface IParser
    {
        void ParseData(out GaSettings settings, out Instance instance);
	    GaSettings ParseSettings();
		void FormatAndSaveResult(string filename, Solution result);
    }
}