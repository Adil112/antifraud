using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI;
using WebAPI.Models;

namespace WebAPI.Neuro
{
    public class Evaluate
    {
        public resultAPI[] EvaluateData(Guid[] users) // возвращает результаты оценивания сессии за посл 10 минут
        {
            Random rnd = new Random();
            antifraudContext db = new antifraudContext();
            resultAPI[] result = new resultAPI[users.Length];
            for(int i =0; i< result.Length; i++)
            {
                resultAPI r = new resultAPI();
                result[i] = r;
            }
            for(int i =0; i< users.Length; i++)
            {
                Normalize norm = new Normalize();
                NormalizedData[] actualData = norm.NormalizeData(users[i], true);
                NormalizedData[] historyData = norm.NormalizeData(users[i], false);

                var newUser = db.Users.Where(s => s.UserId == users[i]).FirstOrDefault();
                result[i].FIO = newUser.Fio;
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
                    double[] actualD = new double[8];
                    actualD[0] = actualData[j].startTime;
                    actualD[1] = actualData[j].finishTime;
                    actualD[2] = actualData[j].country;
                    actualD[3] = actualData[j].pk;
                    actualD[4] = actualData[j].form;
                    actualD[5] = actualData[j].formTime;
                    actualD[6] = actualData[j].section;
                    actualD[7] = actualData[j].sectionTime;

                    Tuple<double, double[]> t1 = new Tuple<double, double[]> (0.1, actualD);
                    datasetActual.Add(t1);
                }

                for (int j = 0; j < historyData.Length; j++) // приводим к нужному типу исторические
                {
                    double[] historyD = new double[8];
                    historyD[0] = historyData[j].startTime;
                    historyD[1] = historyData[j].finishTime;
                    historyD[2] = historyData[j].country;
                    historyD[3] = historyData[j].pk;
                    historyD[4] = historyData[j].form;
                    historyD[5] = historyData[j].formTime;
                    historyD[6] = historyData[j].section;
                    historyD[7] = historyData[j].sectionTime;

                    double r = 0;
                    if (historyData[j].value == 0) r = 0.01 * rnd.Next(1, 30);
                    else r = historyData[j].value;
                    Tuple<double, double[]> t1 = new Tuple<double, double[]>(r, historyD);
                    datasetHistory.Add(t1);
                }

                var topology = new Topology(8,1,0.3,2,2); // задаем параметры
                var neuralNetwork = new NeuralNetworks(topology);
                var difference = neuralNetwork.Learn(datasetHistory, 1000); // обучаем

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
                //($"Общая погрешность: {res / results.Count}");
                double newMark =  mark / results.Count; // Усредненная оценка взлома
                newMark = newMark * 100;
                
                
                var updateUser = db.Users.Where(s => s.UserId == users[i]).FirstOrDefault();
                var lastMark = updateUser.Mark;
                result[i].oldMark = lastMark;
               
                var sess = db.Sessions.Where(s => s.Users == updateUser.UserId).Where(s => s.StartTime >= DateTime.Now.AddMinutes(-10)).ToList();
                foreach(var t in sess)
                {
                    if(t.Value == 1) newMark = rnd.Next(50, 85);
                }
                

                if (newMark > 0 && newMark < 100)
                {
                    result[i].mes = "Оценка взлома изменена с " + lastMark + " на " + (int)newMark;
                    updateUser.Mark = (short)newMark;
                    result[i].newMark = (int)newMark;

                    foreach(var t in sess)
                    {
                        t.Value = (int)newMark;
                    }
                }
                else
                {
                    result[i].mes = "Ошибка вычислений";
                    result[i].newMark = lastMark;
                }

                if (datasetActual.Count == 0)
                {
                    result[i].mes = "Записей о сессиях за последние 10 минут отсутсвуют";
                    result[i].newMark = lastMark;
                }

                Notification.Notify n = new Notification.Notify();
                if (updateUser.Mark > 80) n.Notificate(updateUser.Email, updateUser.Fio);
                db.SaveChanges();
            }
            return result;
        }
    }
}
