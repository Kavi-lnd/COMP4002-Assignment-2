using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using PeerApplication.Models;
using PeerApplication.Services;

namespace PeerApplication
{
    internal class JobClient
      {

         // retrieves list of the avialble jobs from the server
        public List<JobSummary> GetAvailableJobs(   string address)
         {    // channel factory (timeout settings)
            ChannelFactory<IJobService> factory = new ChannelFactory<IJobService>(  new NetTcpBinding
                    {
                          OpenTimeout = TimeSpan.FromSeconds(5),
                         SendTimeout = TimeSpan.FromSeconds(5),
                        ReceiveTimeout = TimeSpan.FromSeconds(5)

                    },  new EndpointAddress(address));

            IJobService client = factory.CreateChannel();

            try { return client.GetAvailableJobs();  }
            finally {
                ((IClientChannel)client).Close();
                  
                  factory.Close();
            }  }

        // claims a job from the server for a specific peer
        public bool ClaimJob(string address, string jobId, string peerId) {
            ChannelFactory<IJobService> factory = new ChannelFactory<IJobService>(
                    new NetTcpBinding
                       {
                        OpenTimeout = TimeSpan.FromSeconds(5),
                        SendTimeout = TimeSpan.FromSeconds(5),
                        ReceiveTimeout = TimeSpan.FromSeconds(5)
                     },
                    new EndpointAddress(address));

            IJobService client = factory.CreateChannel();


            try{
                return client.ClaimJob(jobId, peerId);
            } finally {
                 ((IClientChannel)client).Close();
                
                
                factory.Close();
            }  }

        // Downloads the claimed job for a peer from the server
        public Job DownloadJob(string address, string jobId, string peerId)
         {
            ChannelFactory<IJobService> factory =
                new ChannelFactory<IJobService>(
                    new NetTcpBinding
                    {
                        OpenTimeout = TimeSpan.FromSeconds(5),
                        SendTimeout = TimeSpan.FromSeconds(5),
                        ReceiveTimeout = TimeSpan.FromSeconds(5)
                    },
                    new EndpointAddress(address));

            IJobService client = factory.CreateChannel();
            try {
                return client.DownloadJob(jobId, peerId);
            } finally
            { ((IClientChannel)client).Close();
                 factory.Close();
            }
        }



        // submit the execution result of a job(server)

        public bool SubmitResult(string address, string jobId, string peerId, string result)
        {
            ChannelFactory<IJobService> factory = new ChannelFactory<IJobService>(
                    new NetTcpBinding
                    {  OpenTimeout = TimeSpan.FromSeconds(5),
                       
                        SendTimeout = TimeSpan.FromSeconds(5),
                        ReceiveTimeout = TimeSpan.FromSeconds(5)
                    },
                   
                    new EndpointAddress(address));

            IJobService client = factory.CreateChannel();
            try
            { return client.SubmitResult(jobId, peerId, result);
            }
            finally {  ((IClientChannel)client).Close();
                factory.Close();  }}

    }


}
