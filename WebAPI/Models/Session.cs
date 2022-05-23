using System;
using System.Collections.Generic;

#nullable disable

namespace WebAPI
{
    public partial class Session
    {
        public Guid SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public int? Device { get; set; }
        public int? Location { get; set; }
        public Guid Users { get; set; }
        public int? Form { get; set; }
        public int? Section { get; set; }
        public int? Value { get; set; }
        public int? Browser { get; set; }
        public int? Provider { get; set; }
        public int? System { get; set; }
        public int? Language { get; set; }
        public bool? Vpn { get; set; }
        public bool? Proxy { get; set; }

        public virtual Browser BrowserNavigation { get; set; }
        public virtual Device DeviceNavigation { get; set; }
        public virtual FormTime FormNavigation { get; set; }
        public virtual Language LanguageNavigation { get; set; }
        public virtual Location LocationNavigation { get; set; }
        public virtual Provider ProviderNavigation { get; set; }
        public virtual SectionTime SectionNavigation { get; set; }
        public virtual System SystemNavigation { get; set; }
        public virtual User UsersNavigation { get; set; }
    }
}
