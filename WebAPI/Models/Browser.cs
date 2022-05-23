using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Browser
    {
        public Browser()
        {
            Sessions = new HashSet<Session>();
        }

        public int BrowserId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
