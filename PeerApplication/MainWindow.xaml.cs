
using Microsoft.Win32;
using Newtonsoft.Json;
using PeerApplication.Models;
using PeerApplication.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PeerApplication
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient httpClient = new HttpClient();

        private JobServer jobServer;
        private PeerApplication.Networking.PeerNetworking peerNetworking;
        private const string TrackerUrl = "https://localhost:7083";

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            LoadFileButton.Click += LoadFileButton_Click;
            SubmitJobButton.Click += SubmitJobButton_Click;
            RegisterPeerButton.Click += RegisterPeerButton_Click;
            GetJobsButton.Click += GetJobsButton_Click;
            jobServer = new JobServer();
            // jobServer.Start(int.Parse(WcfPortTextBox.Text));
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Python files (*.py)|*.py|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                PythonCodeTextBox.Text = File.ReadAllText(dialog.FileName);
                StatusTextBlock.Text = "Status: Python file loaded";
            }
        }

        private void SubmitJobButton_Click(object sender, RoutedEventArgs e)
        {
            string code = PythonCodeTextBox.Text;

            MessageBox.Show("Python code being submitted:\n" + code);

            if (string.IsNullOrWhiteSpace(code))
            {
                MessageBox.Show("Please enter or load Python code first.");
                return;
            }

            byte[] codeBytes = Encoding.UTF8.GetBytes(code);
            string encodedCode = Convert.ToBase64String(codeBytes);

            byte[] hashBytes = SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(encodedCode));

            string hash = BitConverter.ToString(hashBytes).Replace("-", "");


            Job job = new Job
            {
                JobId = Guid.NewGuid().ToString(),
                FunctionName = "calculate",
                Code = encodedCode,
                Hash = hash,
                Arguments = new System.Collections.Generic.List<object> { 5 },
                Status = "Available"
            };

            JobService.jobManager.AddJob(job);

            OutputTextBox.Text =
                "Job created:\n" +
                "Job ID: " + job.JobId + "\n" +
                "Function: " + job.FunctionName + "\n" +
                "Status: " + job.Status;

            StatusTextBlock.Text = "Status: Job created";
        }

        private async void RegisterPeerButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(WcfPortTextBox.Text, out int port)
                || port < 1 || port > 65535)
            {
                MessageBox.Show("Enter a valid WCF port.");
                return;
            }

            if (string.IsNullOrWhiteSpace(PeerIdTextBox.Text)
                || string.IsNullOrWhiteSpace(IpAddressTextBox.Text))
            {
                MessageBox.Show("Enter a peer ID and IP address.");
                return;
            }

            var peer = new
            {
                peerId = PeerIdTextBox.Text.Trim(),
                ipAddress = IpAddressTextBox.Text.Trim(),
                wcfPort = port
            };

            try
            {
                RegisterPeerButton.IsEnabled = false;
                StatusTextBlock.Text = "Status: Registering...";

                string json = JsonConvert.SerializeObject(peer);
                using (var content = new StringContent(
                    json, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response = await httpClient.PostAsync(
                        TrackerUrl + "/api/Peers", content);

                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        jobServer.Start(port);
                        if (peerNetworking != null)
                        {
                            peerNetworking.Stop();
                        }

                        peerNetworking =
                            new PeerApplication.Networking.PeerNetworking(
                                PeerIdTextBox.Text.Trim(),
                                TrackerUrl,
                                port);

                        peerNetworking.Start();

                        StatusTextBlock.Text = "Status: Peer registered";
                        OutputTextBox.Text =
                            "Successfully registered with the tracker.\n\n"
                            + responseBody;
                        MessageBox.Show("Peer registered successfully!");
                    }
                    else
                    {
                        StatusTextBlock.Text = "Status: Registration failed";
                        MessageBox.Show(
                            "Tracker returned: " + (int)response.StatusCode
                            + "\n" + responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Status: Connection error";
                MessageBox.Show(
                    "Could not contact the tracker.\n\n" + ex.Message);
            }
            finally
            {
                RegisterPeerButton.IsEnabled = true;
            }
        }






        private async void GetJobsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JobClient jobClient = new JobClient();

                string address = "net.tcp://localhost:"
                    + WcfPortTextBox.Text + "/JobService";
                string peerId = PeerIdTextBox.Text.Trim();

                var jobs = await Task.Run(() =>
                    jobClient.GetAvailableJobs(address));

                if (jobs.Count == 0)
                {
                    OutputTextBox.Text = "No available jobs found.";
                    StatusTextBlock.Text = "Status: No available jobs";
                    return;
                }

                var job = jobs[0];

                bool claimed = await Task.Run(() =>
                    jobClient.ClaimJob(address, job.JobId, peerId));

                if (!claimed)
                {
                    OutputTextBox.Text = "Could not claim the job.";
                    StatusTextBlock.Text = "Status: Job claim failed";
                    return;
                }

                Job downloadedJob = await Task.Run(() =>
                    jobClient.DownloadJob(address, job.JobId, peerId));

                
                if (downloadedJob == null)
                {
                    OutputTextBox.Text = "Job claimed, but download failed.";
                    StatusTextBlock.Text = "Status: Download failed";
                    return;
                }

                OutputTextBox.Text =
                    "Job ID: " + downloadedJob.JobId + "\n"
                    + "Function: " + downloadedJob.FunctionName + "\n"
                    + "Claim successful: True\n"
                    + "Download successful: True";



                PythonExecutor executor = new PythonExecutor();

                string result = await Task.Run(() =>
                    executor.Execute(
                        downloadedJob.Code,
                        downloadedJob.Hash,
                        downloadedJob.FunctionName,
                        downloadedJob.Arguments));

                bool submitted = await Task.Run(() =>
                    jobClient.SubmitResult(
                        address, downloadedJob.JobId, peerId, result));


                OutputTextBox.Text +=
    "\nPython result: " + result
    + "\nResult submitted: " + submitted;

                StatusTextBlock.Text = submitted
                    ? "Status: Job completed"
                    : "Status: Result submission failed";

            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Status: Job operation failed";
                MessageBox.Show(
                    "Could not retrieve, claim, or download the job:\n"
                    + ex.Message);
            }
        }





        private void MainWindow_Closing(
    object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (peerNetworking != null)
            {
                peerNetworking.Stop();
            }

            if (jobServer != null)
            {
                jobServer.Stop();
            }
        }
    }
}
