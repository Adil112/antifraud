using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.MyModels
{
    public class resultAPI
    {
        public Guid userID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string patronymic { get; set; }
        public int oldMark { get; set; }
        public int newMark { get; set; }
        public int forestMark { get; set; }
        public string mes { get; set; }

    }
}
