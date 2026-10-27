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
        {   // adrs og job service is created
            Uri address = new Uri(  "net.tcp://localhost:" + port + "/JobService");

              // service host is created
            
              host = new ServiceHost(typeof(JobService), address);
            // adds the endpoint for the job service
            host.AddServiceEndpoint( typeof(IJobService),  new NetTcpBinding(), "");
            // accepts the client req
            host.Open();

            System.Diagnostics.Debug.WriteLine( "WCF Job Server has been started successfully on port " + port);
        }



        // Jobb server is stopped
        public void Stop() {
            if (host != null)
           
                { host.Close(); }
        }
    }
}
