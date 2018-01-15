namespace Neuro_Evolutionary_System.ANN.Interfaces
{
	public interface IAdaptingFunction
	{
		double ValueAt(double[] x);
		void SetWeights(double[] newWeights);
		double[] GetWeights();
	}
}