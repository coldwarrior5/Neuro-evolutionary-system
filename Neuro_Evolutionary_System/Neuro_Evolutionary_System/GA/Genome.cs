namespace Neuro_Evolutionary_System.GA
{
    public class Genome
    {
        public double[] Genes;
        public double Fitness;

	    public Genome()
	    {
		    Genes = null;
		    Fitness = double.MaxValue;
	    }

		public Genome(double[] genes)
        {
            Genes = genes;
			Fitness = double.MaxValue;
        }

        public Genome(double[] genes, double fitness)
        {
            Genes =  genes;
            Fitness = fitness;
        }

        public void Copy(Genome original)
	    {
		    Genes = DoubleCopy(original.Genes);
		    Fitness = original.Fitness;
	    }
		
		private static double[] DoubleCopy(double[] original)
		{
			double[] newGenes = new double[original.Length];
			for (var i = 0; i < original.Length; i++)
				newGenes[i] = original[i];
			
			return newGenes;
		}
    }
}
