using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class SectionTime
    {
        public SectionTime()
        {
            Sessions = new HashSet<Session>();
        }

        public int SectionTimeId { get; set; }
        public int Time { get; set; }
        public int Section { get; set; }

        public virtual Section SectionNavigation { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
