﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Diplom
{
    public partial class User
    {
        public User()
        {
            Sessions = new HashSet<Session>();
        }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public short Mark { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
