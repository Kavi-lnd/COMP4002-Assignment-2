using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using PeerApplication.Models;

namespace PeerApplication.Services
{     // opertions are defined for the job service (WCF )
    [ServiceContract]
       public interface IJobService  {

        // summary of the available jobs 
        [OperationContract]
        List<JobSummary> GetAvailableJobs();

        // attempts to claim a job 
        [OperationContract]
          bool ClaimJob(string jobId, string peerId);
          // downlaods the job that is calimed(by a peer)
         [OperationContract]
         Job DownloadJob(string jobId, string peerId);
        // Submits the result of a completed job to the server by a peeer
        [OperationContract]
        bool SubmitResult(string jobId, string peerId, string result);
        
    }
}