using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.MyModels
{
    public class DataNeuro
    {
        public DateTime startTime {get; set;}
        public DateTime finishTime { get; set; }
        public int location { get; set; }
        public int device { get; set; }
        public int form { get; set; }
        public int formTime { get; set; }
        public int section { get; set; }
        public int sectionTime { get; set; }
        public int value { get; set; }
        public int browser { get; set; }
        public int provider { get; set; }
        public int system { get; set; }
        public int language { get; set; }
        public bool vpn { get; set; }
        public bool proxy { get; set; }


    }
}
