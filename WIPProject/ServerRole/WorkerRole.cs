using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;


namespace ServerRole {
    public class WorkerRole : RoleEntryPoint {

        class Client {
            Server server;
            TcpClient client;
            NetworkStream stream;

            static readonly int LENGTH = 1024;
            byte[] readBytes = new byte[LENGTH];
            string cmd;

            public Client(TcpClient client, Server server) {
                this.server = server;
                this.client = client;
                this.stream = client.GetStream();

                cmd = String.Empty;

                stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
            }

            public void ReadAsync(IAsyncResult ar) {
                NetworkStream stream = (NetworkStream)ar.AsyncState;
                int numberOfBytesRead = stream.EndRead(ar);

                cmd += Encoding.ASCII.GetString(readBytes, 0, numberOfBytesRead);

                if (!stream.DataAvailable) {
                    server.Command(cmd, stream);
                    cmd = String.Empty; // Reset command
                                        //Console.WriteLine("You received the following message : " + cmd);
                }
                stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
            }

            public NetworkStream getStream() {
                return stream;
            }

            public TcpClient getClient() {
                return client;
            }
        }

        class Server {
            public List<Client> clients = new List<Client>();

            public void Run() {
                Console.WriteLine("Starting Server...\n");
                
                TcpListener listener = new TcpListener(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();

                listener.BeginAcceptTcpClient(new AsyncCallback(ConnectAsync), listener);

                while (true) ;

                int length = clients.Count;
                for (int i = 0; i < length; i++) {
                    clients.ElementAt(i).getClient().Close();
                }
            }

            public void ConnectAsync(IAsyncResult ar) {
                TcpListener listener = (TcpListener)ar.AsyncState;

                TcpClient client = listener.EndAcceptTcpClient(ar);
                clients.Add(new Client(client, this));

                

                listener.BeginAcceptTcpClient(new AsyncCallback(ConnectAsync), listener);
            }

            /// <summary>
            /// Parses a text command and performs logic according to it
            /// </summary>
            /// <param name="cmd"></param>
            public void Command(string cmd, NetworkStream stream) {
                // Parse the message...
                var commands = cmd.Split('\0');
                int length = commands.Count();
                for (int i = 0; i < length; i++) {
                    string currCmd = commands[i];

                    Console.WriteLine("You received the following message : " + currCmd);

                    int spaceIndex = currCmd.IndexOf(' ');
                    spaceIndex = spaceIndex == -1 ? currCmd.Count() : spaceIndex;
                    switch (currCmd.Substring(0, spaceIndex)) {
                        case "CHAT":
                            ParseChatCmd(currCmd.Substring(spaceIndex));
                            break;
                        case "DRAW":
                            WriteToClient("HELP -error:Draw options have not been included for the server yet.", stream);
                            break;
                        case "MESS":
                            WriteToClient("HELP -error:Message options have not been included for the server yet.", stream);
                            break;
                        case "HELP":
                            WriteToClient("HELP -error:Help options have not been included for the server yet.", stream);
                            break;
                        default:
                            //Invalid command, do nothing
                            //WriteToClient("HELP -Error")
                            break;
                    }
                }

            }

            public void ParseChatCmd(string cmd) {
                string tempCmd = cmd;
                // Start with:
                // CHAT -UserName:admin-Color:#FFFFFF-Message:Hi guys you all are my best friends
                // We parse the type
                //  -UserName:admin-Color:#FFFFFF-Message:Hi guys you all are my best friends

                // We find the second dash and make a substring with that

                // We find the first dash (-) and create a substring of the entire rest of message
                // UserName:admin-Color:#FFFFFF-Message:Hi guys you all are my best friends
                int beginInfoInd = cmd.IndexOf('-');
                while (beginInfoInd != -1) {
                    tempCmd = tempCmd.Substring(beginInfoInd + 1);

                    // We find the colon and grab the type
                    // UserName
                    int typeInd = tempCmd.IndexOf(':');
                    string type = tempCmd.Substring(0, typeInd);

                    // We find the next dash (-) to get data
                    int dataInd = tempCmd.IndexOf('-');
                    if (dataInd == -1) {
                        dataInd = tempCmd.Count();
                        beginInfoInd = -1;
                    } else {
                        beginInfoInd = dataInd;
                    }
                    dataInd = dataInd == -1 ? tempCmd.Count() : dataInd;
                    string data = tempCmd.Substring(typeInd + 1, dataInd - typeInd - 1);

                }

                // Theres no need to actually parse the chat command, just relay to other clients
                Write("CHAT " + cmd);
            }

            /// <summary>
            /// Writes a message to all clients
            /// </summary>
            /// <param name="message"></param>
            private void Write(string message) {
                int length = clients.Count;
                for (int i = 0; i < length; i++) {
                    var client = clients.ElementAt(i);
                    if (client.getClient().Connected) {
                        WriteToClient(message, client.getStream());
                    } else {
                        clients.Remove(client);
                        i--;
                    }
                    
                }
            }

            /// <summary>
            /// Writes a message to a specific client
            /// </summary>
            /// <param name="message"></param>
            /// <param name="stream"></param>
            private void WriteToClient(string message, NetworkStream stream) {
                byte[] bytes = ASCIIEncoding.UTF8.GetBytes(message);
                stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(WriteAsync), stream);
            }

            private void WriteAsync(IAsyncResult ar) {
                NetworkStream stream = (NetworkStream)ar.AsyncState;
                stream.EndWrite(ar);
            }
        }

        private Server server = new Server();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run() {
            Trace.TraceInformation("ServerRole is running");

            Server server = new Server();
            server.Run();
            while (true) ;


        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            bool result = base.OnStart();

            Trace.TraceInformation("ServerRole has been started");

            return result;
        }

        public override void OnStop() {
            Trace.TraceInformation("ServerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ServerRole has stopped");
        }

        //try {
        //    this.RunAsync(this.cancellationTokenSource.Token).Wait();
        //} finally {
        //    this.runCompleteEvent.Set();
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
