namespace TrackerWebService.Models
{
    public class Peer
    {
        public string PeerId { get; set; } = string.Empty;

        public string IPAddress { get; set; } = string.Empty;

        public int WcfPort { get; set; }

        public string LastHeartbeat { get; set; } = string.Empty;
    }
}
