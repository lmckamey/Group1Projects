using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using System.Net.Sockets;
using System.IO;

namespace NetworkConnectionRole {
    public class WorkerRole : RoleEntryPoint {
        private readonly AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);

        public override void Run() {
            Trace.TraceInformation("NetworkConnectionRole is running");

            TcpListener listener = null;
            try {
                listener = new TcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();

                Trace.WriteLine("Started TCP Listener", "Information");
            } catch (SocketException se) {
                Trace.Write("Echo server could not start: " + se.ToString(), "Error");
                return;
            }

            while (true) {
                IAsyncResult asyncResult = listener.BeginAcceptTcpClient(HandleAsyncConnection, listener);
                connectionWaitHandle.WaitOne();
            }

            //try {
            //    this.RunAsync(this.cancellationTokenSource.Token).Wait();
            //} finally {
            //    this.runCompleteEvent.Set();
            //}
        }

        private void HandleAsyncConnection(IAsyncResult result) {
            // Accept connection
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);
            connectionWaitHandle.Set();

            // Accepted connection
            Guid clientId = Guid.NewGuid();
            Trace.WriteLine("Accepted connection with ID " + clientId.ToString(), "Information");

            // Setup reader/writer
            NetworkStream netStream = client.GetStream();
            StreamReader reader = new StreamReader(netStream);
            StreamWriter writer = new StreamWriter(netStream);
            writer.AutoFlush = true;

            writer.WriteLine("Hello my good friend");

            // Show application
            /*
            string input = string.Empty;
            while (input != "9") {
                // Show menu
                writer.WriteLine("Menu:");
                writer.WriteLine("-----");
                writer.WriteLine("  1) Display date");
                writer.WriteLine("  2) Display time");
                writer.WriteLine("  3) Role ID");
                writer.WriteLine("  4) Connection ID");
                writer.WriteLine("  8) Recycle");
                writer.WriteLine("  9) Quit");
                writer.WriteLine();
                writer.Write("Your choice: ");

                input = reader.ReadLine();
                writer.WriteLine();

                // Do something
                if (input == "1") {
                    writer.WriteLine("Current date: " + DateTime.Now.ToShortDateString());
                } else if (input == "2") {
                    writer.WriteLine("Current time: " + DateTime.Now.ToShortTimeString());
                } else if (input == "3") {
                    writer.WriteLine("Role ID: " + RoleEnvironment.CurrentRoleInstance.Id);
                } else if (input == "4") {
                    writer.WriteLine("Connection ID: " + clientId.ToString());
                } else if (input == "8") {
                    RoleEnvironment.RequestRecycle();
                }

                writer.WriteLine();
            }
            */

            // Done!
            client.Close();
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            //DiagnosticMonitor.Start("DiagnosticsConnectionString");
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            bool result = base.OnStart();

            Trace.TraceInformation("NetworkConnectionRole has been started");

            return result;
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange)) {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        //public override void OnStop() {
        //    Trace.TraceInformation("NetworkConnectionRole is stopping");

        //    this.cancellationTokenSource.Cancel();
        //    this.runCompleteEvent.WaitOne();

        //    base.OnStop();

        //    Trace.TraceInformation("NetworkConnectionRole has stopped");
        //}

        //private async Task RunAsync(CancellationToken cancellationToken) {
        //    // TODO: Replace the following with your own logic.
        //    while (!cancellationToken.IsCancellationRequested) {
        //        Trace.TraceInformation("Working");
        //        await Task.Delay(1000);
        //    }
        //}
    }
}
