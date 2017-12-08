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
using System.Configuration;
using Microsoft.WindowsAzure.Diagnostics.Management;

namespace ServerRole {

    public class WorkerRole : RoleEntryPoint {

        public enum CmdType { ERROR, REQUEST, SIGNAL, CLEAR, FILL, UNDO, ERASE, COMPLIMENT };


        class Client {
            public readonly Guid id = Guid.NewGuid();
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
                SampleEventSourceWriter.Log.MessageMethod("Starting new Client: " + id);
                stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
            }

            public void ReadAsync(IAsyncResult ar) {
                try {
                    NetworkStream stream = (NetworkStream)ar.AsyncState;
                    int numberOfBytesRead = stream.EndRead(ar);

                    cmd += Encoding.ASCII.GetString(readBytes, 0, numberOfBytesRead);

                    var cmdSplit = cmd.Split('\0');
                    int count = cmdSplit.Count();
                    if (count > 1) {
                        for(int i = 0; i < count-1; i++) {
                            server.Command(cmdSplit[i], this);
                        }
                        cmd = cmdSplit[count - 1];
                    }
                    if (client.Connected) {
                        stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
                    } else {
                        SampleEventSourceWriter.Log.MessageMethod("Client has disconnected");
                        server.Remove(this);
                    }
                } catch (SocketException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    server.Remove(this);
                } catch (IOException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    server.Remove(this);
                } catch (ObjectDisposedException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    server.Remove(this);
                }
            }

            public string getCommand() {
                return cmd;
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
            public Client drawingClient = null;
            private TcpListener listener;

            public void Run() {
                SampleEventSourceWriter.Log.MessageMethod("Starting Server...");

                listener = new TcpListener(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["DefaultEndpoint"].IPEndpoint);
                listener.ExclusiveAddressUse = false;
                listener.Start();
                ServicePointManager.SetTcpKeepAlive(true, 30000, 30000);

                try {
                    while (true) {
                        TcpClient client = listener.AcceptTcpClient();
                        Client c = new Client(client, this);
                        clients.Add(c);
                        if (drawingClient == null) {
                            SampleEventSourceWriter.Log.MessageMethod("Adding Drawing Client");
                            drawingClient = c;
                            WriteToClient("HELP -signal:\0", drawingClient);
                        }
                    }
                } catch (IOException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Stop();
                } catch (SocketException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Stop();
                } catch (ObjectDisposedException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Stop();
                }
            }

            public void Stop() {
                SampleEventSourceWriter.Log.MessageMethod("Stopping Server.");
                for (int i = clients.Count(); i > 0; i--) {
                    clients.ElementAt(i).getClient().Dispose();
                }
                clients.Clear();
                drawingClient = null;
                listener.Stop();
                ServicePointManager.SetTcpKeepAlive(false, 30000, 30000);
            }

            public void Command(string cmd, Client client) {
                string currCmd = cmd;

                int spaceIndex = currCmd.IndexOf(' ');
                spaceIndex = spaceIndex == -1 ? currCmd.Count() : spaceIndex;
                switch (currCmd.Substring(0, spaceIndex)) {
                    case "CHAT":
                        ParseChatCmd(currCmd.Substring(spaceIndex));
                        break;
                    case "DRAW":
                        if (client == drawingClient) {
                            ParseDrawCmd(currCmd.Substring(spaceIndex), client);
                        }
                        break;
                    case "MESS":
                        WriteToClient("HELP -error:Message options have not been included for the server yet.\0", client);
                        break;
                    case "HELP":
                        ParseHelpCmd(currCmd.Substring(spaceIndex), client);    
                        break;
                    default:
                        if (currCmd != String.Empty) {
                            SampleEventSourceWriter.Log.MessageMethod("Recieved invalid command: " + currCmd);
                        } else {
                            SampleEventSourceWriter.Log.MessageMethod(
                                "A client sent an Empty String. It's command was: " + client.getCommand() +
                                "It is " + (client.getClient().Connected ? "connected" : "not connected") +
                                "Guid is " + client.id);
                        }
                        break;
                }

            }

            public void ParseChatCmd(string cmd) {
                string tempCmd = cmd;

                // Theres no need to actually parse the chat command, just relay to other clients
                WriteToAllClients("CHAT " + cmd + '\0');
            }

            public void ParseDrawCmd(string cmd, Client c) {
                cmd = "DRAW " + cmd + '\0';

                // Ne need to actually parse, just send data.
                int length = clients.Count;
                for (int i = 0; i < length; i++) {
                    var client = clients.ElementAt(i);
                    if (client == c) { continue; }
                    WriteToClient(cmd, client);

                }
            }

            public void ParseHelpCmd(string cmd, Client c) {
                CmdType cmdType = CmdType.ERROR;
                string message = "";

                //int beginInfoInd = cmd.IndexOf('-');
                //while (beginInfoInd != -1) {
                //    tempCmd = tempCmd.Substring(beginInfoInd + 1);

                //    // We find the colon and grab the type
                //    // UserName
                //    int typeInd = tempCmd.IndexOf(':');
                //    string type = tempCmd.Substring(0, typeInd);

                //    // We find the next dash (-) to get data
                //    int dataInd = tempCmd.IndexOf('-');
                //    if (dataInd == -1) {
                //        dataInd = tempCmd.Count();
                //        beginInfoInd = -1;
                //    } else {
                //        beginInfoInd = dataInd;
                //    }
                //    dataInd = dataInd == -1 ? tempCmd.Count() : dataInd;
                //    string data = tempCmd.Substring(typeInd + 1, dataInd - typeInd - 1);
                int beginInfoInd = cmd.IndexOf('-');
                int typeInd = cmd.IndexOf(':');
                string type = cmd.Substring(beginInfoInd + 1, typeInd - beginInfoInd - 1);
                //if (type.Equals("data")) {
                //    string allLines = cmd.Substring(typeInd + 1);

                //    lines = allLines.Split('|');
                //    drawDelegate?.Invoke(lines);
                //}

                // Get data related to specific values
                message = cmd.Substring(typeInd + 1);
                switch (type) {
                    case "error":
                        SampleEventSourceWriter.Log.MessageMethod("ERROR: " + message);
                        break;
                    case "erase":
                        cmdType = CmdType.ERASE;
                        goto case "undo";
                    case "clear":
                        cmdType = CmdType.CLEAR;
                        goto case "undo";
                    case "fill":
                        cmdType = CmdType.FILL;
                        goto case "undo";
                    case "undo":
                        cmdType = CmdType.UNDO;
                        // ERROR CMD will Change to UNOD
                        if (c == drawingClient) {
                            cmd = "HELP " + cmd + '\0';
                            int length = clients.Count;
                            for (int i = 0; i < length; i++) {
                                var client = clients.ElementAt(i);
                                if (client == c) { continue; }
                                WriteToClient(cmd, client);

                            }
                        }
                        break;
                    case "compliment":
                        cmdType = CmdType.COMPLIMENT;
                        cmd = "HELP " + cmd + '\0';
                        int length2 = clients.Count;
                        for (int i = 0; i < length2; i++) {
                            var client = clients.ElementAt(i);
                            if (client == c) { continue; }
                            WriteToClient(cmd, client);
                        }
                        break;
                    default:
                        break;
                }
            }

            private void WriteToAllClients(string message) {
                SampleEventSourceWriter.Log.MessageMethod("Wrtiting message to Clients: " + message.Substring(0, 5));

                int length = clients.Count;
                for (int i = 0; i < length; i++) {
                    var client = clients.ElementAt(i);
                    WriteToClient(message, client);

                }
            }

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
                } catch (SocketException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Remove(c);
                } catch (IOException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Remove(c);
                } catch (ObjectDisposedException e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                    Remove(c);
                }
            }

            public void Remove(Client c) {
                SampleEventSourceWriter.Log.MessageMethod("Removing Client");
                clients.Remove(c);
                if (c == drawingClient) {
                    SampleEventSourceWriter.Log.MessageMethod("Removed Current Drawer");
                    drawingClient = null;
                    if (clients.Count > 0) {

                        drawingClient = clients.ElementAt(0);
                        WriteToClient("HELP -signal:\0", drawingClient);
                        SampleEventSourceWriter.Log.MessageMethod("Added Drawing Client. " + clients.Count + " more connected Clients");
                    } else {
                        SampleEventSourceWriter.Log.MessageMethod("No more connected Clients");
                    }
                }
            }
        }

        private Server server = new Server();

        public override void Run() {
            SampleEventSourceWriter.Log.MessageMethod("ServerRole is running");

            while (true) {
                Thread.Sleep(10000);
                try {
                    server.Run();
                } catch (Exception e) {
                    SampleEventSourceWriter.Log.MessageMethod(e.ToString());
                }
            }
        }

        public override bool OnStart() {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            SampleEventSourceWriter.Log.MessageMethod("Worker Role has Started");

            bool result = base.OnStart();
            return result;
        }

        public override void OnStop() {

            base.OnStop();
            SampleEventSourceWriter.Log.MessageMethod("ServerRole has stopped");
        }

    }

    sealed class SampleEventSourceWriter : EventSource {
        public static SampleEventSourceWriter Log = new SampleEventSourceWriter();
        public void MessageMethod(string Message) { if (IsEnabled()) WriteEvent(1, Message); }

    }
}
/*
 * PS C:\Users\Flameo326> Set-AzureServiceDiagnosticsExtension -ServiceName $service_name -StorageAccountName $storage_name
 -StorageAccountKey $key -DiagnosticsConfigurationPath $config_path -Slot Production -Role ServerRole
 * 
 * 
 * 
 *System.IO.IOException: Unable to read data from the transport connection: A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond. ---> System.Net.Sockets.SocketException: A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond
   at System.Net.Sockets.Socket.EndReceive(IAsyncResult asyncResult)
   at System.Net.Sockets.NetworkStream.EndRead(IAsyncResult asyncResult)
   --- End of inner exception stack trace ---
   at System.Net.Sockets.NetworkStream.EndRead(IAsyncResult asyncResult)
   at ServerRole.WorkerRole.Client.ReadAsync(IAsyncResult ar) in C:\Users\Flameo326\Documents\IDEs\Group1Projects\WIPProject\ServerRole\WorkerRole.cs:line 52
 */
