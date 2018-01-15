using System;

namespace Neuro_Evolutionary_System.Structures
{
	public class Classes
	{
		public int NumVariables { get; }
		public int NumClasses { get; }
		
		public Classes(int numVariables, int numClasses)
		{
			NumVariables = numVariables;
			NumClasses = numClasses;
		}

		public string[] DefinedClasses()
		{
			string[] ret = new string[NumClasses];
			for (int i = 0; i < NumClasses; i++)
			{
				ret[i] = i.ToString();
			}
			return ret;
		}

		public string ToString(double[] code)
		{
			if (code.Length != NumClasses)
				return "";
			for (int i = 0; i < code.Length; i++)
			{
				if (Math.Abs(code[i] - 1) < Double.Epsilon)
					return i.ToString();
			}
			return "";
		}
	}

	public class Instance
	{
		public int Size { get; }
		public readonly Sample[] Samples;
		public readonly string[] Classes;

		public Instance(Sample[] samples, string[] classes)
		{
			Size = samples.Length;
			Samples = samples;
			Classes = classes;
		}
	}
}
