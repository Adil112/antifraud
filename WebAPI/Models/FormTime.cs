using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class FormTime
    {
        public FormTime()
        {
            Sessions = new HashSet<Session>();
        }

        public int FormTimeId { get; set; }
        public int Time { get; set; }
        public int Form { get; set; }

        public virtual Form FormNavigation { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
