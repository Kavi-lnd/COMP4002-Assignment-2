using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeerApplication.Models;

namespace PeerApplication.Data
{
    public class JobManager
    {      // Store the jobs that arre managed by this specific instance
          private readonly List<Job> jobs = new List<Job>();
        // helps with prevninting multiple threads from being modified at the same time
        private readonly object jobsLock = new object();
        // copy of the list of the jobs
        public List<Job> GetAllJobs()
        {
            lock (jobsLock)
            { return new List<Job>(jobs);}  }
        // a new job is added to the list of jobs
        // marks whether it is available or not
        public void AddJob(Job job)
          {
              lock (jobsLock)
               {
               
                job.Status = "Available";
                jobs.Add(job);

            } }
          
         // find teh job with the help of the unique id
         
        public Job GetJobById(string jobId)
          {  lock (jobsLock) {  return jobs.Find(job => job.JobId == jobId);
            }
          }





          // check whether a job is existing or not with the use of the id
        
        public bool JobExists(string jobId)
        {
            lock (jobsLock)
              {
                return jobs.Exists(job => job.JobId == jobId);
               }
        }


        // Assigns a job to a peer if it is available and not
        public bool ClaimJob(string jobId, string peerId)
        {  lock (jobsLock)
            {    // find the job 
                Job job = jobs.Find(item => item.JobId == jobId);

                if (job == null || job.Status != "Available") {
                    return false; }

                job.Status = "Claimed";

                job.ClaimedBy = peerId;



                return true;
            }
        }



           // the result that is submitted is saveed
        public bool SubmitResult(string jobId, string peerId, string result)
        {
            lock (jobsLock)
            {
                Job job = jobs.Find(item => item.JobId == jobId);

                if (job == null || job.ClaimedBy != peerId ||  job.Status != "Claimed")
                  {
                      return false;
                 }

                
                 // store the result
                 // marks whether the job is completed
                job.Result = result;
                job.Status = "Completed";

                return true;
            }
        }
    }
}

