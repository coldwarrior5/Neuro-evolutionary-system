namespace Neuro_Evolutionary_System.ANN.Interfaces
{
	public interface INeuron
	{
		double GetOutput(double[] input);
		void SetWeights(double[] newWeights);
		double[] GetWeights();
	}
}