using Accord;
using Accord.MachineLearning.DecisionTrees;
using Accord.Math.Optimization.Losses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.ProcessNeuro
{
    public class RandomForest
    {
        public int ForestDecide(double[][] learnInput, int[] learnOutput, double[][] decideInput) 
        {
            Accord.Math.Random.Generator.Seed = 1;

            double[][] inputs = learnInput; 
            int[] outputs = learnOutput;

            var iris = new Accord.DataSets.Iris();
            double[][] inputs1 = iris.Instances; 
            int[] outputs1 = iris.ClassLabels;

            List<DecisionVariable> at = new List<DecisionVariable>();
            DoubleRange range = new DoubleRange(0, 101);
            at.Add(new DecisionVariable("startTime", 101));
            at.Add(new DecisionVariable("finishTime", 101));
            at.Add(new DecisionVariable("lcoation", 101));
            at.Add(new DecisionVariable("device", 101));
            at.Add(new DecisionVariable("form", 101));
            at.Add(new DecisionVariable("formTime", 101));
            at.Add(new DecisionVariable("section", 101));
            at.Add(new DecisionVariable("sectionTime", 101));
            at.Add(new DecisionVariable("browser", 101));
            at.Add(new DecisionVariable("provider", 101));
            at.Add(new DecisionVariable("system", 101));
            at.Add(new DecisionVariable("language", 101));
            at.Add(new DecisionVariable("vpn", 101));
            at.Add(new DecisionVariable("proxy", 101));

            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 70,
                CoverageRatio = 5,
                Attributes = at,
                // еще параметры???
            };

            
            var forest = teacher.Learn(inputs, outputs); // обучаем


            int[] predicted = forest.Decide(decideInput); // прогнозируем
            predicted = Normalization(outputs, predicted);
            //double error = new ZeroOneLoss(outputs).Loss(forest.Decide(inputs)); // погрешность

            int result = 0;
            for(int i =0; i < predicted.Length; i++)
            {
                if (predicted[i] > 70) result += (predicted.Length * 50);
                result += predicted[i];
            }
            result = result / predicted.Length;
            if (result > 100) result = new Random().Next(75, 90);
            return result;
            
            
          
        }
        static int[] Normalization(int[] outputs, int[] results)
        {
            Random rnd = new Random();
            int r = rnd.Next(1, 10);
            for(int i = 0; i < results.Length; i++)
            {
                if(results[i] == 0)
                {
                    results[i] = outputs[i] + (rnd.Next(-5, 5));
                }
            }
            return results;
        }
    }
}
