namespace Neuro_Evolutionary_System.ANN.Interfaces
{
	public interface INeuron
	{
		double GetOutput(double[] input, int position);
		void SetWeights(double[] newWeights);
		double[] GetWeights();
	}
}