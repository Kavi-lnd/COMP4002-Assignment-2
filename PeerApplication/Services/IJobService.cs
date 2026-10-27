using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections.Generic;
using PeerApplication.Models;

namespace PeerApplication.Services
{
    [ServiceContract]
    public interface IJobService
    {
        [OperationContract]
        List<JobSummary> GetAvailableJobs();

        [OperationContract]
        bool ClaimJob(string jobId, string peerId);

        [OperationContract]
        Job DownloadJob(string jobId, string peerId);

        [OperationContract]
        bool SubmitResult(string jobId, string peerId, string result);
    }
}