using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PeerApplication.Models
{
    public class PeerRegistrationRequest
    {
        public string PeerId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int WcfPort { get; set; }
    }
}

