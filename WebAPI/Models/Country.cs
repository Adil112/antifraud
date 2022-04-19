using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Country
    {
        public Country()
        {
            Sessions = new HashSet<Session>();
        }

        public byte CountryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
