using System;
using Neuro_Evolutionary_System.ANN.Interfaces;
using Neuro_Evolutionary_System.ANN.Structures;

namespace Neuro_Evolutionary_System.ANN
{
	public class NeuralNetwork
	{
		private int NumberOfLayers { get; }
		private int _numberOfParameters;
		private readonly int[] _architectureSchema;
		private readonly INeuronLayer[] _layers;
		private LayerInfo[] _architecture;
		
		public NeuralNetwork(int[] architecture)
		{
			NumberOfLayers = architecture.Length;
			_architectureSchema = architecture;
			_layers = new INeuronLayer[NumberOfLayers];
			InitNetwork();
		}

		private void InitNetwork()
		{
			DefineArchitecture(out LayerInfo[] architecture);
			if (!(_architecture is null) && _architecture.Equals(architecture)) return;
			_architecture = architecture;
			for (int i = 0; i < NumberOfLayers; i++)
			{
				_layers[i] = new NeuronLayer(architecture[i]);
				_numberOfParameters += architecture[i].NumberOfParameters;
			}
		}

		private void DefineArchitecture(out LayerInfo[] architecture)
		{
			architecture = new LayerInfo[NumberOfLayers];
			for (int i = 0; i < NumberOfLayers; i++)
			{
				switch (i)
				{
					case 0:
						architecture[i] = new LayerInfo(1, _architectureSchema[i], NeuronType.Input);
						break;
					case 1:
						architecture[i] = new LayerInfo(_architectureSchema[i - 1], _architectureSchema[i], NeuronType.Type1Neuron);
						break;
					default:
						architecture[i] = new LayerInfo(_architectureSchema[i - 1], _architectureSchema[i], NeuronType.Type2Neuron);
						break;
				}
			}
		}
		
		public void SetWeights(double[] newWeights)
		{
			var index = 0;
			for (var i = 1; i < NumberOfLayers; i++)
			{
				switch (_architecture[i].Type)
				{
					case NeuronType.Type1Neuron:
					{
						int length = _architecture[i].NumberOfNeurons * _architecture[i].InputSize * 2;
						double[] newData = SubArray(newWeights, index, length);
						index += length;
						_layers[i].SetWeights(newData);
						break;
					}
					case NeuronType.Type2Neuron:
					{
						int length = _architecture[i].NumberOfNeurons * (_architecture[i].InputSize + 1);
						double[] newData = SubArray(newWeights, index, length);
						index += length;
						_layers[i].SetWeights(newData);
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
			double[] allWeights = new double[_numberOfParameters];
			int index = 0;
			for (int i = 1; i < NumberOfLayers; i++)
			{
				double[] weights = _layers[i].GetWeights();
				foreach (double t in weights)
					allWeights[index++] = t;
			}
			return allWeights;
		}

		public double[] GetOutput(double[] inputs)
		{
			double[] tempInputs = inputs;
			for (var i = 0; i < NumberOfLayers; i++)
			{
				try
				{
					tempInputs = _layers[i].GetOutputs(tempInputs);
				}
				catch(Exception)
				{
					return null;
				}
			}
			return tempInputs;
		}
	}
}