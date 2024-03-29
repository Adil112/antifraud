﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GeneratorAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            DataAPI data = new DataAPI();
            int sessionNum = 10;


            

            string json = JsonSerializer.Serialize(data);
            Console.WriteLine(json);
        }
    }
    public class DataAPI
    {
        public List<UserAPI> users { get; set; }
    }
    public class UserAPI
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string FIO { get; set; }
        public List<sessionApi> sessions { get; set; }

    }
    public class sessionApi
    {
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public int country { get; set; }
        public bool pk { get; set; }
        public int value { get; set; }
        public List<formApi> forms { get; set; }
        public List<sectionApi> sections { get; set; }
    }
    public class formApi
    {
        public int formId { get; set; }
        public int time { get; set; }
    }
    public class sectionApi
    {
        public int sectionId { get; set; }
        public int time { get; set; }
    }
}
