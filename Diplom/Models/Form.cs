using System;
using System.Collections.Generic;

#nullable disable

namespace Diplom
{
    public partial class Form
    {
        public Form()
        {
            FormTimes = new HashSet<FormTime>();
        }

        public int FormId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<FormTime> FormTimes { get; set; }
    }
}
