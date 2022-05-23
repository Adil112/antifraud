using Accord.MachineLearning.DecisionTrees;
using Accord.Math.Optimization.Losses;
using System;

namespace RandomForest
{
    class Program
    {
        static void Main(string[] args)
        {
            Accord.Math.Random.Generator.Seed = 1;
            // First, let's load the dataset:
            var iris = new DataSets.Iris();
            double[][] inputs = iris.Instances; // flower features
            int[] outputs = iris.ClassLabels; // flower categories

            // Create the forest learning algorithm
            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 10, // use 10 trees in the forest
            };

            // Finally, learn a random forest from data
            var forest = teacher.Learn(inputs, outputs);

            // We can estimate class labels using
            int[] predicted = forest.Decide(inputs);

            // And the classification error (0.0006) can be computed as 
            double error = new ZeroOneLoss(outputs).Loss(forest.Decide(inputs));
        }
    }
}
