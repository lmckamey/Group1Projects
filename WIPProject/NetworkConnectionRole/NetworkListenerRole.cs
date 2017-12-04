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
using System.Text;

namespace NetworkConnectionRole {
    public class NetworkListenerRole : ThreadedRoleEntryPoint {
        TcpListener listener = null;
        List<TcpClient> clients = null;
        private readonly AutoResetEvent connectionWaitHandle = new AutoResetEvent(false);

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e) {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange)) {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }

        public override void Run() {
            Trace.TraceInformation("NetworkConnectionRole is running");

            while (true) {

            }

        }      

        public override bool OnStart() {
            try {
                listener = new TcpListener(RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();

                clients = new List<TcpClient>();

                Trace.WriteLine("Started TCP Listener", "Information");
            } catch (SocketException se) {
                Trace.Write("Echo server could not start: " + se.ToString(), "Error");
            }


            List<WorkerEntryPoint> workers = new List<WorkerEntryPoint>();

            workers.Add(new Listener(this));
            workers.Add(new Connector(this));




            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            //DiagnosticMonitor.Start("DiagnosticsConnectionString");
            RoleEnvironment.Changing += RoleEnvironmentChanging;

            bool result = base.OnStart(workers.ToArray());

            Trace.TraceInformation("NetworkConnectionRole has been started");

            return result;
        }

        public void Add(TcpClient client) {
            clients.Add(client);
        }

        internal class Listener : WorkerEntryPoint {
            private NetworkListenerRole role;

            static int LENGTH = 512;

            static string cmdCHAT = "CHAT";
            static string cmdDRAW = "DRAW";

            public Listener(NetworkListenerRole role) {
                this.role = role;
            }

            public override void Run() {
                while (true) {
                    // Do Some Work
                    foreach(var c in role.clients) {
                        byte[] bytes = new byte[LENGTH];
                        var stream = c.GetStream();
                        int amo = stream.Read(bytes, 0, LENGTH);
                        if (amo > 0) {
                            string cmd = GetMessage(bytes, amo);
                            Parse(bytes, amo);
                        }
                    }
                   
                }
            }

            public string GetMessage(byte[] bytes, int amo) {

            }

            public void Parse(byte[] bytes, int amo) {
                string message = ASCIIEncoding.UTF8.GetString(bytes, 0, amo);
                if (cmd.Equals(cmdCHAT)) {

                }
                if()
            }

            public void Write() {
                foreach (var c in role.clients) {
                    var stream = c.GetStream();
                }
            }
        }

        internal class Connector : WorkerEntryPoint {
            private NetworkListenerRole role;

            public Connector(NetworkListenerRole role) {
                this.role = role;
            }

            public override void Run() {
                while (true) {
                    //IAsyncResult asyncResult = role.listener.BeginAcceptTcpClient(HandleAsyncConnection, role.listener);
                    //role.connectionWaitHandle.WaitOne();

                    if (role.listener.Pending()) {
                        var client = role.listener.AcceptTcpClient();
                        role.Add(client);
                    }
                }
            }

            private void HandleAsyncConnection(IAsyncResult result) {
                // Accept connection
               // TcpListener listener = (TcpListener)result.AsyncState;
                //TcpClient client = listener.EndAcceptTcpClient(result);
                //role.connectionWaitHandle.Set();

                // Accepted connection
                //Guid clientId = Guid.NewGuid();
                //Trace.WriteLine("Accepted connection with ID " + clientId.ToString(), "Information");

                // Setup reader/writer
                //NetworkStream netStream = client.GetStream();
                //StreamReader reader = new StreamReader(netStream);
                //StreamWriter writer = new StreamWriter(netStream);
                //writer.AutoFlush = true;

                //writer.WriteLine("Hello my good friend");

                //role.Add(client);

                // Done!
                //client.Close();
            }
        }
    }
}

/*
 * Start up the TCP Listener
 * Add a thread to check for incoming connections
 * When a client connects 
 * 
 * 
 * 
 * 
 * 
 */