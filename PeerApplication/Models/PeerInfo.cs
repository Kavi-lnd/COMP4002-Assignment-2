using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PeerApplication.Models
{
      // details of the peer taht is registered is stored here (also with the tracker)
    public class PeerInfo
    {
        
        public string PeerId { get; set; }
        public string IpAddress { get; set; }
        public int WcfPort { get; set; }
    }
}

