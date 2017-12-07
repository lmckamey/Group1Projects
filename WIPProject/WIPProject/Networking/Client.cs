using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace WIPProject.Networking {
    public class Client {
        static private readonly string CONN_STRING = "40.69.169.63";
        static private TcpClient client;
        static private bool isConnected = false;

        static private readonly int LENGTH = 1024;
        static private byte[] readBytes = new byte[LENGTH];
        static private byte[] writeBytes;
        static private string cmd = String.Empty;

        public delegate void ChatCommand(string username, string message, string color);
        static private ChatCommand chatDelegate;

        public delegate void MessageCommand();
        static private MessageCommand messageDelegate;

        public delegate void DrawCommand(string[] lines);
        static private DrawCommand drawDelegate;
        
        public enum CmdType { ERROR, REQUEST, SIGNAL};
        public delegate void HelpCommand(CmdType type, string error);
        static private HelpCommand helpDelegate;

        static public void Add(ChatCommand chatFunc) { chatDelegate += chatFunc; }
        static public void Add(MessageCommand messageFunc) { messageDelegate += messageFunc; }
        static public void Add(DrawCommand drawFunc) { drawDelegate += drawFunc; }
        static public void Add(HelpCommand helpFunc) { helpDelegate += helpFunc; }

        static public void Remove(ChatCommand chatFunc) { chatDelegate -= chatFunc; }
        static public void Remove(MessageCommand messageFunc) { messageDelegate -= messageFunc; }
        static public void Remove(DrawCommand drawFunc) { drawDelegate -= drawFunc; }
        static public void Remove(HelpCommand helpFunc) { helpDelegate -= helpFunc; }

        static public void Initialize() {
            ServicePointManager.SetTcpKeepAlive(true, 30000, 30000);
            client = new TcpClient();
            client.Connect(CONN_STRING, 10100);
            isConnected = true;

            NetworkStream stream = client.GetStream();

            stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
        }

        static public void Shutdown(bool withRestart = false) {
            if(client != null) {
                client.Close();
            }

            ServicePointManager.SetTcpKeepAlive(false, 0, 0);
            isConnected = false;
            readBytes = new byte[LENGTH];
            cmd = String.Empty;
            // Remove delegate connections???

            if (withRestart) {
                do {
                    try {
                        client = new TcpClient();
                        client.Connect(CONN_STRING, 10100);
                        ServicePointManager.SetTcpKeepAlive(true, 30000, 30000);
                        isConnected = true;

                        NetworkStream stream = client.GetStream();

                        stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
                    } catch (IOException e) {
                        MessageBox.Show(e.ToString());
                    }catch (SocketException e) {
                        MessageBox.Show(e.ToString());
                    }
                } while (!isConnected);
                
            }
        }

        static private void ReadAsync(IAsyncResult ar) {
            try {
                NetworkStream stream = (NetworkStream)ar.AsyncState;
                int numberOfBytesRead = stream.EndRead(ar);

                cmd += Encoding.ASCII.GetString(readBytes, 0, numberOfBytesRead);

                //if (!stream.DataAvailable) {
                //    //cmd.Last() == '\0'
                //    // !stream.DataAvailable
                //    Parse(cmd);
                //    cmd = String.Empty; // Reset command
                //    stream.Flush();
                //}
                var cmdSplit = cmd.Split('\0');
                int count = cmdSplit.Count();
                if (count > 1) {
                    for (int i = 0; i < count - 1; i++) {
                        Parse(cmdSplit[i]);
                    }
                    cmd = cmdSplit[count - 1];
                }
                if (client.Connected) {
                    stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
                } else {
                    Shutdown();
                }
            } catch (SocketException e) {
                Shutdown(true); // Error inside Socket class 
            } catch(IOException e) {
                Shutdown(true); // Error connecting to Server, restart
            } catch(ObjectDisposedException e) {
                Shutdown(); // Object has been disclosed and closed. This most likely happened client side, it is fine.
            }
        }

        static public void WriteChatMessage(string user, string message, string color = "#FFFFFF") {
            if (isConnected) {
                string cmd = "CHAT -username:" + user + "-message:" + message + "-color:" + color + '\0';
                writeBytes = ASCIIEncoding.UTF8.GetBytes(cmd);

                NetworkStream stream = client.GetStream();
                stream.BeginWrite(writeBytes, 0, writeBytes.Length, new AsyncCallback(WriteAsync), stream);
            }
        }

        static public void WriteDrawMessage(Line[] lines) {
            if (isConnected) {
                StringBuilder sb = new StringBuilder();

                foreach (Line l in lines) {
                    if (l != null) {
                        sb.Append(XamlWriter.Save(l));
                        sb.Append("|");
                    }
                }

                if (sb.Length > 0) {
                    sb.Remove(sb.Length - 1, 1);


                    string cmd = "DRAW -data:" + sb.ToString() + '\0';
                    writeBytes = ASCIIEncoding.UTF8.GetBytes(cmd);

                    NetworkStream stream = client.GetStream();
                    stream.BeginWrite(writeBytes, 0, writeBytes.Length, new AsyncCallback(WriteAsync), stream);
                }
            }
        }

        static private void WriteAsync(IAsyncResult ar) {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            stream.EndWrite(ar);
        }

        static private void Parse(string cmd) {
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
                        ParseDrawCmd(currCmd.Substring(spaceIndex));
                        break;
                    case "MESS":
                        break;
                    case "HELP":
                        ParseHelpCmd(currCmd.Substring(spaceIndex));
                        break;
                    default:
                        Console.WriteLine("Invalid message recieved: " + currCmd);
                        break;
                }
            }
        }

        static private void ParseChatCmd(string cmd) {
            string userName = "";
            string color = "#FFFFFF"; // WHITE ? OR BLACK?
            string message = "";

            string tempCmd = cmd;

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

                // Get data related to specific values
                switch (type) {
                    case "username":
                        userName = data;
                        break;
                    case "color":
                        color = data;
                        break;
                    case "message":
                        message = data;
                        break;
                    default:
                        break;
                }
            }

            // ChatCommand Deleggate function
            chatDelegate?.Invoke(userName, message, color);
        }

        static private void ParseDrawCmd(string cmd) {
            //List<Line> lines = new List<Line>();
            string[] lines;

            int beginInfoInd = cmd.IndexOf('-');
            int typeInd = cmd.IndexOf(':');
            string type = cmd.Substring(beginInfoInd+1, typeInd - beginInfoInd - 1);
            if (type.Equals("data")) {
                string allLines = cmd.Substring(typeInd+1);

                lines = allLines.Split('|');
                drawDelegate?.Invoke(lines);
            }
        }

        static private void ParseHelpCmd(string cmd) {
            string error = "";
            CmdType cmdType = CmdType.ERROR;

            string tempCmd = cmd;

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

                // Get data related to specific values
                switch (type) {
                    case "error":
                        error = data;
                        cmdType = CmdType.ERROR;
                        break;
                    case "signal":
                        cmdType = CmdType.SIGNAL;
                        break;
                    default:
                        break;
                }
            }

            // Chat Command Error Delegate
            helpDelegate?.Invoke(cmdType, error);
        }
    }
}
// An empty string is being sent to the server very frequently
// The server will ocassionaly get a IOexception other client did not respond quick enough... and remove the client.
// Something with Drawing client not correct