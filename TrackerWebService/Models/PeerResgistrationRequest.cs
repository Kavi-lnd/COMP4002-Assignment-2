namespace TrackerWebService.Models
{
    public class PeerRegistrationRequest
    {
        public string PeerId { get; set; } = string.Empty;

        public string IpAddress { get; set; } = string.Empty;

        public int WcfPort { get; set; }
    }
}
