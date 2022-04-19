using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class User
    {
        public User()
        {
            Sessions = new HashSet<Session>();
        }

        public Guid UserId { get; set; }
        public string Fio { get; set; }
        public string Email { get; set; }
        public short Mark { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
