using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;

using Microsoft.WindowsAzure.Storage;
using System.IO;

namespace ServerRole {
    public class WorkerRole : RoleEntryPoint {

        private enum LogType { INFO, WARN, ERROR };
        private sealed class Logger : EventSource {
          
            public void Log(LogType type, string Message) { if (IsEnabled()) WriteEvent(2, type.ToString() + ": " + Message); }
        }
        private static Logger Log = new Logger();

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
                Trace.TraceInformation("Starting new Client");
                stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
            }

            public void ReadAsync(IAsyncResult ar) {
                try {
                    NetworkStream stream = (NetworkStream)ar.AsyncState;
                    int numberOfBytesRead = stream.EndRead(ar);

                    cmd += Encoding.ASCII.GetString(readBytes, 0, numberOfBytesRead);

                    if (!stream.DataAvailable) {
                        server.Command(cmd, this);
                        cmd = String.Empty; // Reset command
                                            //Console.WriteLine("You received the following message : " + cmd);
                    }
                    if (client.Connected) {
                        stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
                    } else {
                        Trace.TraceInformation("Client has disconnected");
                        server.Remove(this);
                    }
                } catch (SocketException e) {
                    Trace.TraceInformation(e.ToString());
                    server.Remove(this);
                } catch(IOException e) {
                    Trace.TraceInformation(e.ToString());
                    server.Remove(this);
                } catch (ObjectDisposedException e) {
                    Trace.TraceInformation(e.ToString());
                    server.Remove(this);
                }
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
            private TcpListener listener;
            private System.Timers.Timer activeConn;

            public void Run() {
                Trace.TraceInformation("Starting Server...");
                
                listener = new TcpListener(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();
                ServicePointManager.SetTcpKeepAlive(true, 30000, 30000);

                activeConn = new System.Timers.Timer(30000);
                activeConn.Elapsed += DefaultConnectionMessage;
                activeConn.AutoReset = true;
                activeConn.Enabled = true;

                try {
                    while (true) {
                        //if (listener.Pending()) {
                            TcpClient client = listener.AcceptTcpClient();
                            clients.Add(new Client(client, this));
                        //}
                    }
                } catch (IOException e) {
                    Trace.TraceInformation(e.ToString());
                    Stop();
                } catch (SocketException e) {
                    Trace.TraceInformation(e.ToString());
                    Stop();
                } catch (ObjectDisposedException e) {
                    Trace.TraceInformation(e.ToString());
                    Stop();
                }
            }

            public void Stop() {
                Trace.TraceInformation("Stopping Server.");
                for(int i = clients.Count(); i > 0 ; i--) {
                    clients.ElementAt(i).getClient().Dispose();
                }
                clients.Clear();
                listener.Stop();
                ServicePointManager.SetTcpKeepAlive(false, 30000, 30000);
                activeConn.Enabled = false;
            }

            private void DefaultConnectionMessage(object obj, ElapsedEventArgs args) {
                Write("a");
            }

            /// <summary>
            /// Parses a text command and performs logic according to it
            /// </summary>
            /// <param name="cmd"></param>
            public void Command(string cmd, Client client) {
                // Parse the message...
                var commands = cmd.Split('\0');
                int length = commands.Count();
                for (int i = 0; i < length; i++) {
                    string currCmd = commands[i];

                    int spaceIndex = currCmd.IndexOf(' ');
                    spaceIndex = spaceIndex == -1 ? currCmd.Count() : spaceIndex;
                    switch (currCmd.Substring(0, spaceIndex)) {
                        case "CHAT":
                            ParseChatCmd(currCmd.Substring(spaceIndex));
                            break;
                        case "DRAW":
                            WriteToClient("HELP -error:Draw options have not been included for the server yet.", client);
                            break;
                        case "MESS":
                            WriteToClient("HELP -error:Message options have not been included for the server yet.", client);
                            break;
                        case "HELP":
                            WriteToClient("HELP -error:Help options have not been included for the server yet.", client);
                            break;
                        default:
                            Trace.TraceWarning("Recieved invalid command: " + currCmd);
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
                Trace.TraceInformation("Wrrtiting message to Clients: " + message);
                int length = clients.Count;
                for (int i = 0; i < length; i++) {
                    var client = clients.ElementAt(i);
                    WriteToClient(message, client);                   
                }
            }

            /// <summary>
            /// Writes a message to a specific client
            /// </summary>
            /// <param name="message"></param>
            /// <param name="stream"></param>
            private void WriteToClient(string message, Client c) {
                byte[] bytes = ASCIIEncoding.UTF8.GetBytes(message);
                var stream = c.getStream();
                stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(WriteAsync), c);
            }

            private void WriteAsync(IAsyncResult ar) {
                Client c = (Client)ar.AsyncState;
                try {
                    
                    var stream = c.getStream();
                    stream.EndWrite(ar);
                } catch(SocketException e) {
                    Trace.TraceError(e.ToString());
                    Remove(c);
                } catch(IOException e) {
                    Trace.TraceError(e.ToString());
                    Remove(c);
                } catch(ObjectDisposedException e) {
                    Trace.TraceError(e.ToString());
                    Remove(c);
                }               
            }

            public void Remove(Client c) {
                clients.Remove(c);
            }
        }

        private Server server = new Server();

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run() {
            Trace.TraceInformation("ServerRole is running");

            while (true) {
                server.Run();
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;
            
            //DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString");

            bool result = base.OnStart();

            Log.Log(LogType.INFO, "Worker Role has Started");
            Trace.TraceInformation("ServerRole has been started");

            return result;
        }

        public override void OnStop() {

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
/*
 * After the client is closed, an exception may throw about reading data where the client stream is not working
 * Can not acces disposed object - System.ObjectDisposedException
 * 
 * 'Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.' - IOException
 * 
 * 
 * try
        {
            // ShutdownEvent is a ManualResetEvent signaled by
            // Client when its time to close the socket.
            while (!ShutdownEvent.WaitOne(0))
            {
                try
                {
                    // We could use the ReadTimeout property and let Read()
                    // block.  However, if no data is received prior to the
                    // timeout period expiring, an IOException occurs.
                    // While this can be handled, it leads to problems when
                    // debugging if we are wanting to break when exceptions
                    // are thrown (unless we explicitly ignore IOException,
                    // which I always forget to do).
                    if (!_stream.DataAvailable)
                    {
                        // Give up the remaining time slice.
                        Thread.Sleep(1);
                    }
                    else if (_stream.Read(_data, 0, _data.Length) > 0)
                    {
                        // Raise the DataReceived event w/ data...
                    }
                    else
                    {
                        // The connection has closed gracefully, so stop the
                        // thread.
                        ShutdownEvent.Set();
                    }
                }
                catch (IOException ex)
                {
                    // Handle the exception...
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception...
        }
        finally
        {
            _stream.Close();
        }
 * 
 */
