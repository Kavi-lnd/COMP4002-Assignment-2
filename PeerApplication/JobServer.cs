using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using PeerApplication.Services;

namespace PeerApplication
{
    internal class JobServer
    {
        private ServiceHost host;

        public void Start(int port)
        {
            Uri address = new Uri(
                "net.tcp://localhost:" + port + "/JobService");

            host = new ServiceHost(typeof(JobService), address);

            host.AddServiceEndpoint(
                typeof(IJobService),
                new NetTcpBinding(),
                "");

            host.Open();
            System.Diagnostics.Debug.WriteLine(
    "WCF Job Server started successfully on port " + port);
        }

        public void Stop()
        {
            if (host != null)
            {
                host.Close();
            }
        }
    }
}
