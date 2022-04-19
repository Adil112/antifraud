using System;
using System.Collections.Generic;

#nullable disable

namespace Diplom
{
    public partial class Session
    {
        public Guid SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public bool Pk { get; set; }
        public byte Country { get; set; }
        public Guid Users { get; set; }
        public int Form { get; set; }
        public int Section { get; set; }
        public int Value { get; set; }

        public virtual Country CountryNavigation { get; set; }
        public virtual FormTime FormNavigation { get; set; }
        public virtual SectionTime SectionNavigation { get; set; }
        public virtual User UsersNavigation { get; set; }
    }
}
