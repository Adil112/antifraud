using System;
using System.Collections.Generic;

#nullable disable

namespace Diplom
{
    public partial class Language
    {
        public Language()
        {
            Sessions = new HashSet<Session>();
        }

        public int LanguageId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
    }
}
