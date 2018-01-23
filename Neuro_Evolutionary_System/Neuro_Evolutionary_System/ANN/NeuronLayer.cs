using System;
using Neuro_Evolutionary_System.ANN.Interfaces;
using Neuro_Evolutionary_System.ANN.NeuronClasses;
using Neuro_Evolutionary_System.ANN.Structures;

namespace Neuro_Evolutionary_System.ANN
{
	public class NeuronLayer : INeuronLayer
	{
		public LayerInfo Architecture { get; }
		private INeuron[] _neurons;

		public NeuronLayer(LayerInfo architecture)
		{
			Architecture = architecture;
			InitNeuron();
		}

		private void InitNeuron()
		{
			_neurons = new INeuron[Architecture.NumberOfNeurons];
			for (int i = 0; i < Architecture.NumberOfNeurons; i++)
			{
				switch (Architecture.Type)
				{
					case NeuronType.Input:
						_neurons[i] = new InputNeuron();
						break;
					case NeuronType.Type1Neuron:
						_neurons[i] = new Type1Neuron(Architecture.InputSize);
						break;
					case NeuronType.Type2Neuron:
						_neurons[i] = new Type2Neuron(Architecture.InputSize);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(Architecture.Type), Architecture.Type, null);
				}
			}
		}

		public double[] GetOutputs(double[] inputs)
		{
			if (_neurons is null)
				return null;

			double[] outputs = new double[Architecture.NumberOfNeurons];
			for (int i = 0; i < Architecture.NumberOfNeurons; i++)
			{
				outputs[i] = Architecture.InputSize == 1 ? _neurons[i].GetOutput(new[] { inputs[i] }) : _neurons[i].GetOutput(inputs);
			}
			return outputs;
		}

		public void SetWeights(double[] newWeights)
		{
			var index = 0;
			for (var i = 0; i < Architecture.NumberOfNeurons; i++)
			{
				switch (Architecture.Type)
				{
					case NeuronType.Type1Neuron:
					{
						int length = Architecture.InputSize * 2;
						double[] newData = SubArray(newWeights, index, length);
						index += length;
						_neurons[i].SetWeights(newData);
						break;
					}
					case NeuronType.Type2Neuron:
					{
						int length = Architecture.InputSize + 1;
						double[] newData = SubArray(newWeights, index, length);
						index += length;
						_neurons[i].SetWeights(newData);
						break;
					}
				}
			}
		}

		private static T[] SubArray<T>(T[] data, int index, int length)
		{
			T[] result = new T[length];
			Array.Copy(data, index, result, 0, length);
			return result;
		}

		public double[] GetWeights()
		{
			double[] allWeights = new double[Architecture.NumberOfParameters];
			int index = 0;
			for (int i = 0; i < Architecture.NumberOfNeurons; i++)
			{
				double[] weights = _neurons[i].GetWeights();
				foreach (double t in weights)
					allWeights[index++] = t;
			}
			return allWeights;
		}
	}
}