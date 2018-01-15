namespace Neuro_Evolutionary_System.Structures
{
	public class Sample
	{
		public readonly double[] Variables;
		public readonly double[] Classes;

		public Sample(double[] variables, double[] classes)
		{
			Variables = variables;
			Classes = classes;
		}
	}
}