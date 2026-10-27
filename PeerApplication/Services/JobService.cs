using PeerApplication.Data;
using PeerApplication.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeerApplication.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class JobService : IJobService
    {
        public static readonly JobManager jobManager = new JobManager();

        public List<JobSummary> GetAvailableJobs()
        {
            List<JobSummary> availableJobs = new List<JobSummary>();

            foreach (Job job in jobManager.GetAllJobs())
            {
                if (job.Status == "Available")
                {
                    availableJobs.Add(new JobSummary
                    {
                        JobId = job.JobId,
                        FunctionName = job.FunctionName
                    });
                }
            }

            return availableJobs;
        }

        public bool ClaimJob(string jobId, string peerId)
        {
            return jobManager.ClaimJob(jobId, peerId);
        }

        public Job DownloadJob(string jobId, string peerId)
        {
            Job job = jobManager.GetJobById(jobId);

            if (job == null || job.ClaimedBy != peerId)
            {
                return null;
            }

            return job;
        }

        public bool SubmitResult(string jobId, string peerId, string result)
        {
            return jobManager.SubmitResult(jobId, peerId, result);
        }
    }
}
