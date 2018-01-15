namespace Neuro_Evolutionary_System.ANN.Interfaces
{
	public interface INeuronLayer
	{
		double[] GetOutputs(double[] inputs);
		void SetWeights(double[] newWeights);
		double[] GetWeights();
	}
}