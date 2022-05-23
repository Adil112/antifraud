using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratorData.Models
{
    public class DataAPI
    {
        public List<UserAPI> users { get; set; }
    }


    public class UserAPI
    {
        public Guid userID { get; set; }
        public string email { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public List<sessionApi> sessions { get; set; }
      
    }
    public class sessionApi
    {
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public int location { get; set; }
        public int device { get; set; }
        public int browser { get; set; }
        public int provider { get; set; }
        public int system { get; set; }
        public int language { get; set; }
        public bool vpn { get; set; }
        public bool proxy { get; set; }
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
