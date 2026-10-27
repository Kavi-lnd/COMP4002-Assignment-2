namespace TrackerWebService.Models
{
    public class Peer
    {
           // Unique id to idwntify the peer
         public string PeerId { get; set; } = string.Empty;
        // Ip of the peer
        public string IPAddress { get; set; } = string.Empty;

         
        // Port 
        public int WcfPort { get; set; }

        // time that last heartbeat is sent by the peer
        public string LastHeartbeat { get; set; } = string.Empty;
    
    
    }


}
