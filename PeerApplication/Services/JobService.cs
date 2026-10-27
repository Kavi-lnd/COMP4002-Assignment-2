using PeerApplication.Data;
using PeerApplication.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeerApplication.Services;

namespace PeerApplication.Services
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class JobService : IJobService
    {



         public static readonly JobManager jobManager = new JobManager();


        public List<JobSummary> GetAvailableJobs() {
            List<JobSummary> availableJobs = new List<JobSummary>();
             // whether all the jobs are being managed by the job manager is checked here
            foreach (Job job in jobManager.GetAllJobs()) {
                if (job.Status == "Available") { availableJobs.Add(new JobSummary
                    { JobId = job.JobId,
                        FunctionName = job.FunctionName
                    }); } } return availableJobs;
          }
        // attempts to claim a job for a peer
        public bool ClaimJob(string jobId, string peerId)
           {  return jobManager.ClaimJob(jobId, peerId); }




        // return whether the job exists and belong to the requesting peer
        public Job DownloadJob(string jobId, string peerId)
        {
            Job job = jobManager.GetJobById(jobId);
            if (job == null || job.ClaimedBy != peerId)  {
                return null;
            } return job;
          }


        // Stores the result of a completed job.
        public bool SubmitResult(string jobId, string peerId, string result)
        { try    {
                
                return jobManager.SubmitResult(jobId, peerId, result);
            } catch (Exception ex)  {
                
                System.Diagnostics.Debug.WriteLine( "Error in submitting job result: " + ex.Message);
                 return false;
            }  }
    }
}
