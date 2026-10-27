namespace TrackerWebService.Models
{   public class PeerRegistrationRequest {

        // Details to register a peer.
        public string PeerId { get; set; } = string.Empty;


        // Peer's IP address
        public string IpAddress { get; set; } = string.Empty;
        // Port (peer's WCF Service)
        public int WcfPort { get; set; }
    }
}
