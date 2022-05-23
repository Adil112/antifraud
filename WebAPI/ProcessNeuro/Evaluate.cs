using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI;
using WebAPI.MyModels;
using WebAPI.Neuro;

namespace WebAPI.ProcessNeuro
{
    public class Evaluate
    {
        private const int parametrCount = 14;
        private static List<Tuple<int, int>> map = new List<Tuple<int, int>>();
        public resultAPI[] EvaluateData(Guid[] users) // возвращает результаты оценивания сессии за посл 10 минут
        {
            Random rnd = new Random();
            antifraudContext db = new antifraudContext();
            resultAPI[] result = new resultAPI[users.Length];
            for (int i = 0; i < result.Length; i++)
            {
                resultAPI r = new resultAPI();
                result[i] = r;
            }
            for (int i = 0; i < users.Length; i++)
            {
                Normalize norm = new Normalize();
                NormalizedData[] actualData = norm.NormalizeData(users[i], true);
                NormalizedData[] historyData = norm.NormalizeData(users[i], false);



                var newUser = db.Users.Where(s => s.UserId == users[i]).FirstOrDefault();
                result[i].name = newUser.Name;
                result[i].surname = newUser.Surname;
                result[i].patronymic = newUser.Patronymic;
                result[i].userID = newUser.UserId;
                if (historyData.Length < 10) // если исторических данных мало, то не оценивается
                {
                    result[i].oldMark = newUser.Mark;
                    result[i].newMark = 0;
                    result[i].mes = "Оценка невозможна из-за недостатка исторических записей";
                    continue;
                }

                var datasetActual = new List<Tuple<double, double[]>>();
                var datasetHistory = new List<Tuple<double, double[]>>();

                for (int j = 0; j < actualData.Length; j++) // приводим к нужному типу актуальные
                {
                    double[] actualD = new double[14];
                    actualD[0] = actualData[j].startTime;
                    actualD[1] = actualData[j].finishTime;
                    actualD[2] = actualData[j].location;
                    actualD[3] = actualData[j].device;
                    actualD[4] = actualData[j].form;
                    actualD[5] = actualData[j].formTime;
                    actualD[6] = actualData[j].section;
                    actualD[7] = actualData[j].sectionTime;
                    actualD[8] = actualData[j].browser;
                    actualD[9] = actualData[j].provider;
                    actualD[10] = actualData[j].system;
                    actualD[11] = actualData[j].language;
                    actualD[12] = actualData[j].vpn;
                    actualD[13] = actualData[j].proxy;


                    Tuple<double, double[]> t1 = new Tuple<double, double[]>(0.1, actualD);
                    datasetActual.Add(t1);
                }
               

                for (int j = 0; j < historyData.Length; j++) // приводим к нужному типу исторические
                {
                    double[] historyD = new double[14];
                    historyD[0] = historyData[j].startTime;
                    historyD[1] = historyData[j].finishTime;
                    historyD[2] = historyData[j].location;
                    historyD[3] = historyData[j].device;
                    historyD[4] = historyData[j].form;
                    historyD[5] = historyData[j].formTime;
                    historyD[6] = historyData[j].section;
                    historyD[7] = historyData[j].sectionTime;
                    historyD[8] = historyData[j].browser;
                    historyD[9] = historyData[j].provider;
                    historyD[10] = historyData[j].system;
                    historyD[11] = historyData[j].language;
                    historyD[12] = historyData[j].vpn;
                    historyD[13] = historyData[j].proxy;

                    double r = 0;
                    if (historyData[j].value == 0) r = 0.01 * rnd.Next(2, 60);
                    else r = historyData[j].value;
                    Tuple<double, double[]> t1 = new Tuple<double, double[]>(r, historyD);
                    datasetHistory.Add(t1);
                }
                int forestResult = 0;
                if(datasetActual.Count > 0) // random forest
                {
                    double[][] learnInput = new double[datasetHistory.Count][];
                    int[] learnOutput = new int[datasetHistory.Count];
                    double[][] decideInput = new double[datasetActual.Count][];
                    for (int e =0; e<datasetHistory.Count; e++)
                    {
                        learnInput[e] = new double[parametrCount];
                    }
                    for (int e = 0; e < datasetActual.Count; e++)
                    {
                        decideInput[e] = new double[parametrCount];
                    }
                    int historyCounter = 0;
                    foreach(var t in datasetHistory)
                    {
                        for(int k =0; k < parametrCount; k++)
                        {
                            learnInput[historyCounter][k] = t.Item2[k];
                        }
                        learnOutput[historyCounter] = (int)(t.Item1 * 100);
                        historyCounter++;
                    }
                    InsertionSort(learnOutput, learnInput); // сортировка

                    int actualCounter = 0;
                    foreach (var t in datasetActual)
                    {
                        for (int k = 0; k < parametrCount; k++)
                        {
                            decideInput[actualCounter][k] = t.Item2[k];
                        }
                        actualCounter++;
                    }
                    RandomForest rf = new RandomForest();
                    forestResult = rf.ForestDecide(learnInput, learnOutput, decideInput);
                }
                result[i].forestMark = forestResult;

                var topology = new Topology(15, 1, 0.3, 10); // задаем параметры   !!!если рез-тат плохой поменять 
                var neuralNetwork = new NeuralNetworks(topology);
                var difference = neuralNetwork.Learn(datasetHistory, 150); // обучаем

                var results = new List<double>();
                foreach (var data in datasetActual)
                {
                    results.Add(neuralNetwork.FeedForward(data.Item2).Output); // используем
                }
                double res = 0;
                double mark = 0;
                for (int ii = 0; ii < results.Count; ii++) // собираем результаты
                {
                    var expected = Math.Round(datasetActual[ii].Item1, 4);
                    var actual = Math.Round(results[ii], 4);
                    mark += actual;
                    double promres = (Math.Pow(expected - actual, 2) / expected) * 100;
                    res += promres;
                }
                var updateUser = db.Users.Where(s => s.UserId == users[i]).FirstOrDefault();
                var lastMark = updateUser.Mark;
                result[i].oldMark = lastMark;
                //($"Общая погрешность: {res / results.Count}");
                double newMark = 0;
                
                if (datasetActual.Count != 0)
                {
                    newMark = mark / results.Count; // Усредненная оценка взлома
                    newMark = newMark * 100;              
                }
                else
                {
                    result[i].mes = "Записей о сессиях за последние 10 минут отсутсвуют";
                    result[i].newMark = lastMark;
                }
                


                

                var sess = db.Sessions.Where(s => s.Users == updateUser.UserId).Where(s => s.StartTime >= DateTime.Now.AddMinutes(-10)).ToList();
                foreach (var t in sess)
                {
                    if (t.Value == 1) newMark = rnd.Next(60, 90);
                }
                if (newMark >= 0 && newMark <= 100)
                {
                    
                    result[i].mes = "Оценка взлома изменена с " + lastMark + "% на " + (int)newMark + "%";
                    updateUser.Mark = (short)newMark;
                    result[i].newMark = (int)newMark;

                    foreach (var t in sess)
                    {
                        t.Value = (int)newMark;
                    }
                }
                else
                {
                    result[i].mes = "Ошибка вычислений: " + (int)newMark;
                    result[i].newMark = lastMark;
                }

                

                Notification.Notify n = new Notification.Notify();
                if (updateUser.Mark > 80) n.Notificate(updateUser.Email, (updateUser.Surname + ' ' + updateUser.Name));
                db.SaveChanges();
            }
            return result;
        }

        static void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        static void SwapD(double[][] array, int i, int j)
        {
            double[] temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        static void InsertionSort(int[] inArray, double[][] input)
        {
            int x;
            int j;
            double[] xx;
            for (int i = 1; i < inArray.Length; i++)
            {
                x = inArray[i];
                xx = input[i];
                j = i;
                while (j > 0 && inArray[j - 1] > x)
                {
                    Swap(inArray, j, j - 1);
                    SwapD(input, j, j - 1);
                    j -= 1;
                }
                inArray[j] = x;
                input[j] = xx;
            }
        }
        static int[] Mapper(int[] ar)
        {
            List<int> nums = new List<int>();
            for (int i = 0; i < ar.Length; i++)
            {
                if (nums.Contains(ar[i]) == false) nums.Add(ar[i]); 
            }
            int counter = 0;
            foreach(var t in nums)
            {
                var tuple = new Tuple<int, int>(counter, t); // 1 - псевдо, 2 - настоящий
                map.Add(tuple);
                for(int i = 0; i< ar.Length; i++)
                {
                    if (ar[i] == t) ar[i] = counter;
                }
                counter++;
            }
            return ar;
        }
        static int ReverseMapper(int res)
        {
            foreach(var t in map)
            {
                if (t.Item1 == res) res = t.Item2;
            }
            return res;
        }
    }
}
