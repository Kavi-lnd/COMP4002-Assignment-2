using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PeerApplication.Models
{
    public class PeerInfo
    {
        public string PeerId { get; set; }
        public string IpAddress { get; set; }
        public int WcfPort { get; set; }
    }
}

