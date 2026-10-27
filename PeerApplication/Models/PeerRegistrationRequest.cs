using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PeerApplication.Models
{
    public class PeerRegistrationRequest
    {
            //uniuque id of peer
          public string PeerId { get; set; } = string.Empty;
        
        
        // Ip adr of the peer
        public string IpAddress { get; set; } = string.Empty;
          
        
        
        
          // port (WCF service of the peer)
        public int WcfPort { get; set; }
    }
}

