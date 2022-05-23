using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Provider
    {
        public Provider()
        {
            Sessions = new HashSet<Session>();
        }

        public int ProviderId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
