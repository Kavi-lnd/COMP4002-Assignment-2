using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PeerApplication.Models;

namespace PeerApplication.Data
{
    public class JobManager
    {
        private readonly List<Job> jobs = new List<Job>();
        private readonly object jobsLock = new object();
        public List<Job> GetAllJobs()
        {
            lock (jobsLock)
            {
                return new List<Job>(jobs);
            }
        }

        public void AddJob(Job job)
        {
            lock (jobsLock)
            {
                job.Status = "Available";
                jobs.Add(job);
            }
        }

        public Job GetJobById(string jobId)
        {
            lock (jobsLock)
            {
                return jobs.Find(job => job.JobId == jobId);
            }
        }

        public bool JobExists(string jobId)
        {
            lock (jobsLock)
            {
                return jobs.Exists(job => job.JobId == jobId);
            }
        }

        public bool ClaimJob(string jobId, string peerId)
        {
            lock (jobsLock)
            {
                Job job = jobs.Find(item => item.JobId == jobId);

                if (job == null || job.Status != "Available")
                {
                    return false;
                }

                job.Status = "Claimed";
                job.ClaimedBy = peerId;

                return true;
            }
        }


        public bool SubmitResult(string jobId, string peerId, string result)
        {
            lock (jobsLock)
            {
                Job job = jobs.Find(item => item.JobId == jobId);

                if (job == null || job.ClaimedBy != peerId ||
                    job.Status != "Claimed")
                {
                    return false;
                }

                job.Result = result;
                job.Status = "Completed";

                return true;
            }
        }
    }
}

