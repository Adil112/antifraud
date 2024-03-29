﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Neuro
{
    public class NeuralNetworks
    {
        public Topology Topology { get; }
        public List<Layer> Layers { get; }

        public NeuralNetworks(Topology topology)
        {
            Topology = topology;
            Layers = new List<Layer>();

            CreateInputLayers();
            CreateHiddenLayers();
            CreateOutputLayers();
        }

        public Neuron FeedForward(params double[] inputSignals)
        {
            SendSignalsToInputNeurons(inputSignals);
            FeedForwardAllLayersAfterInput();

            return Layers.Last().Neurons[0];//возвращаем послдений нейрон в сети

        }

        public double Learn(List<Tuple<double, double[]>> dataset, int epoch)
        {
            var error = 0.0;

            for (int i = 0; i < epoch; i++)
            {
                var promer = 0.0;
                foreach (var data in dataset)
                {
                    error += Backpropagation(data.Item1, data.Item2);
                    promer += Backpropagation(data.Item1, data.Item2);
                }
                //Console.WriteLine($"Номер эпохи: {i}");
                //Console.WriteLine($"Среднеквадратичная ошибка: {promer}");
            }
            var result = error / epoch;
            return result;
        }

        private double Backpropagation(double expected, params double[] inputs)
        {
            var actual = FeedForward(inputs).Output;
            var difference = actual - expected;
            //var difference = expected - actual;
            foreach (var neuron in Layers.Last().Neurons) // обучаем выходной нейрон
            {
                neuron.Learn(difference, Topology.LearningRate);
            }
            for (int j = Layers.Count - 2; j >= 0; j--) // обучаем все остальные
            {
                var layer = Layers[j];
                var previousLayer = Layers[j + 1];

                for (int i = 0; i < layer.NeuronCount; i++)
                {
                    var neuron = layer.Neurons[i];

                    for (int k = 0; k < previousLayer.NeuronCount; k++)
                    {
                        var previousNeuron = previousLayer.Neurons[k];

                        var error = previousNeuron.Weights[i] * previousNeuron.Delta;
                        neuron.Learn(error, Topology.LearningRate);
                    }
                }
            }

            return difference * difference; // возвращаем квадратичную ошибку
        }

        private void FeedForwardAllLayersAfterInput()
        {
            for (int i = 1; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                var previousLayerSignals = Layers[i - 1].GetSignals();

                foreach (var neuron in layer.Neurons)
                {
                    neuron.FeedForward(previousLayerSignals);
                }
            }
        }

        private void SendSignalsToInputNeurons(params double[] inputSignals)
        {
            for (int i = 0; i < inputSignals.Length; i++)
            {
                var signal = new List<double> { inputSignals[i] };
                var neuron = Layers[0].Neurons[i];

                neuron.FeedForward(signal);
            }
        }

        private void CreateOutputLayers()
        {
            var outputNeurons = new List<Neuron>();
            var lastLayer = Layers.Last();
            for (int i = 0; i < Topology.OutputCount; i++)
            {
                var neuron = new Neuron(lastLayer.NeuronCount, NeuronType.Output);
                outputNeurons.Add(neuron);
            }
            var outputLayer = new Layer(outputNeurons, NeuronType.Output);
            Layers.Add(outputLayer);
        }

        private void CreateHiddenLayers()
        {
            for (int j = 0; j < Topology.HiddenLayers.Count; j++)
            {
                var hiddenNeurons = new List<Neuron>();
                var lastLayer = Layers.Last();
                for (int i = 0; i < Topology.HiddenLayers[j]; i++)
                {
                    var neuron = new Neuron(lastLayer.NeuronCount);
                    hiddenNeurons.Add(neuron);
                }
                var hiddenLayer = new Layer(hiddenNeurons);
                Layers.Add(hiddenLayer);
            }

        }

        private void CreateInputLayers()
        {
            var inputNeurons = new List<Neuron>();
            for (int i = 0; i < Topology.InputCount; i++)
            {
                var neuron = new Neuron(1, NeuronType.Input);
                inputNeurons.Add(neuron);
            }
            var inputLayer = new Layer(inputNeurons, NeuronType.Input);
            Layers.Add(inputLayer);
        }
    }
}
