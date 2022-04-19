using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class resultAPI
    {
        public Guid userID { get; set; }
        public string FIO { get; set; }
        public int oldMark { get; set; }
        public int newMark { get; set; }
        public string mes { get; set; }
    }
}
