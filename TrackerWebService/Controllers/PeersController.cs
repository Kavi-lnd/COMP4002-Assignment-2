
using Microsoft.AspNetCore.Mvc;
using TrackerWebService.Data;
using TrackerWebService.Models;

namespace TrackerWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeersController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetPeers()
        {
            DBManager.RemoveStalePeers();

            return Ok(DBManager.GetAllPeers());
        }


        [HttpPost]
        public IActionResult RegisterPeer(
            [FromBody] PeerRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PeerId) ||
                string.IsNullOrWhiteSpace(request.IpAddress) ||
                request.WcfPort < 1 ||
                request.WcfPort > 65535)
            {
                return BadRequest("Invalid peer registration details.");
            }

            try
            {
                string heartbeat = DateTime.UtcNow.ToString("O");

                bool registered = DBManager.RegisterPeer(
                    request.PeerId,
                    request.IpAddress,
                    request.WcfPort,
                    heartbeat);

                if (registered)
                {
                    return CreatedAtAction(
                        nameof(GetPeers),
                        new { },
                        new { message = "Peer registered successfully." });
                }

                return BadRequest("Peer registration failed.");
            }
            catch (Exception)
            {
                return Conflict("This peer may already be registered.");
            }
        }


        [HttpPut("{peerId}/heartbeat")]
        public IActionResult UpdateHeartbeat(string peerId)
        {
            string heartbeat = DateTime.UtcNow.ToString("O");

            bool updated = DBManager.UpdateHeartbeat(peerId, heartbeat);

            if (!updated)
            {
                return NotFound("Peer not found.");
            }

            return Ok(new { message = "Heartbeat updated successfully." });
        }

    }
}
