using Microsoft.AspNetCore.Mvc;
using TrackerWebService.Data;

namespace TrackerWebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompletedJobsController : ControllerBase

         // Peer id and the job id that are recieved in a req is stored
    {   public class CompletedJobRequest
         {
                public string PeerId { get; set; } = string.Empty;
             public string JobId { get; set; } = string.Empty;
         }


         // Record whether a job has been completed(req body)
        [HttpPost]
        public IActionResult RecordCompletedJob(
            [FromBody] CompletedJobRequest request)
        {   // peer id and job id are provided or not checked
            if (string.IsNullOrWhiteSpace(request.PeerId) ||  string.IsNullOrWhiteSpace(request.JobId))
            {   // if value is not given
                return BadRequest("Peer ID & Job ID are required!");
            }

            try
            {
                bool recorded = DBManager.RecordCompletedJob( request.PeerId, request.JobId);
                if (recorded)
                {
                    return Ok(new
                    {
                        message = "Completed job has been recorded successfully!"
                    });
                } return BadRequest("Could not record completed job...");
            }
            catch (Exception)
            {
                 
                return BadRequest(  "Could not record job... Check that the peer is registered.");
            }
        }



        // No.of jobs that are completed by a peer is retrived


        [HttpGet("count/{peerId}")]
        public IActionResult GetCompletedJobCount(string peerId)
         {
            // chek if peer id is given or not
            if (string.IsNullOrWhiteSpace(peerId))
            { return BadRequest("Peer ID is required."); }
            // No.of completed jobs (database)
            int count = DBManager.GetCompletedJobCount(peerId);


            return Ok(new   {
                peerId = peerId,
                completedJobCount = count
            });  }


        // Peer info and jobs that are completed are retrieved
           [HttpGet("peer-summary")]
        public IActionResult GetPeerSummary()
          {
            // remove the peers (hearbeats are stale)
            DBManager.RemoveStalePeers();
            var peers = DBManager.GetPeersWithJobCounts();


            
            return Ok(peers);


        }



    }
}
