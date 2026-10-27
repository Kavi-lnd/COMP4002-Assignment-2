
using Microsoft.AspNetCore.Mvc;
using TrackerWebService.Data;
using TrackerWebService.Models;

namespace TrackerWebService.Controllers
{
    [ApiController]
    
    // Peers that are registered are all retrieved here
    [Route("api/[controller]")]
    public class PeersController : ControllerBase   {

        [HttpGet]
         public IActionResult GetPeers()
         {
            // Remove the peers who are not sending heartbeats
            DBManager.RemoveStalePeers();
            // any remaining peers arer returned
            return Ok(DBManager.GetAllPeers());
           }





            // a new peer is Registered using the deatils that are iin the req body
         [HttpPost]
        public IActionResult RegisterPeer( [FromBody] PeerRegistrationRequest request)
        {     // the peer deatils and the validity of the WCF prts are checked
            if (string.IsNullOrWhiteSpace(request.PeerId) ||  
                string.IsNullOrWhiteSpace(request.IpAddress) ||
                request.WcfPort < 1 || request.WcfPort > 65535)
                 {
               // Invalid details
                return BadRequest("Invalid peer registration details.");
            } try {   // initial heartbeat is in UTC
                     string heartbeat = DateTime.UtcNow.ToString("O");
                // save the peer deails in hte database
                bool registered = DBManager.RegisterPeer(
                    request.PeerId,request.IpAddress,
                    request.WcfPort,
                      heartbeat);

                if (registered)  {
                    // correct successful registration
                      return CreatedAtAction(
                        nameof(GetPeers),
                        new { },
                        new { message = "Peer has been registered successfully!" });
                }

                
                // registrtion fails 
                return BadRequest("Peer registration has failed!");
            }  catch (Exception)
              {
                 return Conflict("This peer may already be registered!");
              }
        }


        // heartbeat time is updated for the peer with the a exiting peerId

        [HttpPut("{peerId}/heartbeat")]
          public IActionResult UpdateHeartbeat(string peerId)
          {
            // latest UTC time of the heartbeat
               string heartbeat = DateTime.UtcNow.ToString("O");
              bool updated = DBManager.UpdateHeartbeat(peerId, heartbeat);
               if (!updated) {
                  return NotFound("Peer not found!");
              }

           
            return Ok(new { message = "Heartbeat is updated successfully!" });
        }



    }
}
