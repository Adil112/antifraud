using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Location
    {
        public Location()
        {
            Sessions = new HashSet<Session>();
        }

        public int LocationId { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
