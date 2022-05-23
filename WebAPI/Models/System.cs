using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class System
    {
        public System()
        {
            Sessions = new HashSet<Session>();
        }

        public int SystemId { get; set; }
        public string Name { get; set; }
        public bool? ComputerAbility { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
