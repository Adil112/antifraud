using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Section
    {
        public Section()
        {
            SectionTimes = new HashSet<SectionTime>();
        }

        public int SectionId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SectionTime> SectionTimes { get; set; }
    }
}
