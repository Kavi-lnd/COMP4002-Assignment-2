using Newtonsoft.Json;
using PeerApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace PeerApplication.Networking
{
    internal class PeerNetworking
    {
        private Thread networkingThread;
        private bool isRunning;

        private string peerId;
        private string trackerUrl;
        private int wcfPort;

        public PeerNetworking(string peerId, string trackerUrl, int wcfPort)
        {
            this.peerId = peerId;
            this.trackerUrl = trackerUrl;
            this.wcfPort = wcfPort;
        }
        public void Start()
        {
            isRunning = true;

            networkingThread = new Thread(NetworkingLoop);
            networkingThread.IsBackground = true;
            networkingThread.Start();
        }


        private void NetworkingLoop()
        {
            while (isRunning)
            {
                try
                {
                    SendHeartbeat().GetAwaiter().GetResult();

                    List<PeerInfo> peers = GetPeerList()
                        .GetAwaiter().GetResult();

                    System.Diagnostics.Debug.WriteLine(
    "Peers found: " + peers.Count);

                    foreach (PeerInfo peer in peers)
                    {
                        if (peer.PeerId == peerId)
                        {
                            continue;
                        }

                        string address = "net.tcp://"
                            + peer.IpAddress + ":"
                            + peer.WcfPort + "/JobService";

                        System.Diagnostics.Debug.WriteLine(
                            "Peer address: " + address);

                        JobClient jobClient = new JobClient();

                        List<JobSummary> jobs =
                            jobClient.GetAvailableJobs(address);

                        System.Diagnostics.Debug.WriteLine(
                            "Available jobs from " + peer.PeerId
                            + ": " + jobs.Count);

                        if (jobs.Count == 0)
                        {
                            continue;
                        }


                        JobSummary job = jobs[0];

                        bool claimed = jobClient.ClaimJob(
                            address, job.JobId, peerId);

                        if (!claimed)
                        {
                            continue;
                        }

                        System.Diagnostics.Debug.WriteLine(
                            "Claimed job: " + job.JobId);



                        Job downloadedJob = jobClient.DownloadJob(
                            address, job.JobId, peerId);

                        if (downloadedJob == null)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Job download failed: " + job.JobId);

                            continue;
                        }

                        System.Diagnostics.Debug.WriteLine(
                            "Job downloaded successfully: " + downloadedJob.JobId);


                        PythonExecutor executor = new PythonExecutor();

                        string result = executor.Execute(
                            downloadedJob.Code,
                            downloadedJob.Hash,
                            downloadedJob.FunctionName,
                            downloadedJob.Arguments);

                        System.Diagnostics.Debug.WriteLine(
                            "Python result: " + result);

                        bool submitted = jobClient.SubmitResult(
                            address, downloadedJob.JobId, peerId, result);

                        System.Diagnostics.Debug.WriteLine(
                            "Result submitted: " + submitted);

                        if (submitted)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Job completed successfully: " + downloadedJob.JobId);

                            ReportCompletedJob(downloadedJob.JobId)
                                .GetAwaiter().GetResult();
                        }

                    }
                }


                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Peer discovery failed: " + ex.Message);
                }



                Thread.Sleep(5000);
            }
        }

        private async Task SendHeartbeat()
        {
            using (HttpClient client = new HttpClient())
            {
                await client.PutAsync(
                    trackerUrl + "/api/Peers/"
                    + Uri.EscapeDataString(peerId)
                    + "/heartbeat",
                    null);
            }
        }

        private async Task<string> GetPeers()
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(
                    trackerUrl + "/api/Peers");
            }
        }


        private async Task<List<PeerInfo>> GetPeerList()
        {
            string json = await GetPeers();

            return JsonConvert.DeserializeObject<List<PeerInfo>>(json);
        }

        private async Task ReportCompletedJob(string jobId)
        {
            using (HttpClient client = new HttpClient())
            {
                var completedJob = new
                {
                    PeerId = peerId,
                    JobId = jobId
                };

                string json = JsonConvert.SerializeObject(completedJob);

                StringContent content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await client.PostAsync(
                    trackerUrl + "/api/CompletedJobs",
                    content);

                System.Diagnostics.Debug.WriteLine(
                    "Tracker recorded completed job: " + response.IsSuccessStatusCode);
            }
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
