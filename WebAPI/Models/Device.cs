using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Device
    {
        public Device()
        {
            Sessions = new HashSet<Session>();
        }

        public int DeviceId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
