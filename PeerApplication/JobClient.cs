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
        public List<JobSummary> GetAvailableJobs(
            string address)
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

            try
            {
                return client.GetAvailableJobs();
            }
            finally
            {
                ((IClientChannel)client).Close();
                factory.Close();
            }
        }

        public bool ClaimJob(string address, string jobId, string peerId)
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
            try
            {
                return client.ClaimJob(jobId, peerId);
            }
            finally
            {
                ((IClientChannel)client).Close();
                factory.Close();
            }
        }


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
            try
            {
                return client.DownloadJob(jobId, peerId);
            }
            finally
            {
                ((IClientChannel)client).Close();
                factory.Close();
            }
        }



        public bool SubmitResult(string address, string jobId, string peerId, string result)
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
            try
            {
                return client.SubmitResult(jobId, peerId, result);
            }
            finally
            {
                ((IClientChannel)client).Close();
                factory.Close();
            }
        }

    }
}
