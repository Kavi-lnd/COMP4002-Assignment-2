using Microsoft.AspNetCore.Mvc;
using TrackerWebService.Data;

namespace TrackerWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedJobsController : ControllerBase
    {
        public class CompletedJobRequest
        {
            public string PeerId { get; set; } = string.Empty;
            public string JobId { get; set; } = string.Empty;
        }

        [HttpPost]
        public IActionResult RecordCompletedJob(
            [FromBody] CompletedJobRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PeerId) ||
                string.IsNullOrWhiteSpace(request.JobId))
            {
                return BadRequest("Peer ID and Job ID are required.");
            }

            try
            {
                bool recorded = DBManager.RecordCompletedJob(
                    request.PeerId, request.JobId);

                if (recorded)
                {
                    return Ok(new
                    {
                        message = "Completed job recorded successfully."
                    });
                }

                return BadRequest("Could not record completed job.");
            }
            catch (Exception)
            {
                return BadRequest(
                    "Could not record the job. Check that the peer is registered.");
            }
        }


        [HttpGet("count/{peerId}")]
        public IActionResult GetCompletedJobCount(string peerId)
        {
            if (string.IsNullOrWhiteSpace(peerId))
            {
                return BadRequest("Peer ID is required.");
            }

            int count = DBManager.GetCompletedJobCount(peerId);

            return Ok(new
            {
                peerId = peerId,
                completedJobCount = count
            });
        }



        [HttpGet("peer-summary")]
        public IActionResult GetPeerSummary()
        {
            DBManager.RemoveStalePeers();

            var peers = DBManager.GetPeersWithJobCounts();

            return Ok(peers);
        }



    }
}
