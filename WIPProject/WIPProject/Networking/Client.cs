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

namespace WIPProject.Networking {
    public class Client { 
        static private TcpClient client;

        static private readonly int LENGTH = 1024;
        static private byte[] readBytes = new byte[LENGTH];
        static private byte[] writeBytes;
        static private string cmd = String.Empty;

        public delegate void ChatCommand(string username, string message, int color);
        static private ChatCommand chatDelegate;

        public delegate void MessageCommand();
        static private MessageCommand messageDelegate;

        public delegate void DrawCommand();
        static private DrawCommand drawDelegate;

        public delegate void HelpCommand(string error);
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
            client = new TcpClient();
            client.Connect("40.69.169.63", 10100);
            Console.WriteLine("Connected!\n");

            NetworkStream stream = client.GetStream();

            stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);

            //writeBytes = ASCIIEncoding.UTF8.GetBytes(Console.ReadLine());
            //stream.BeginWrite(writeBytes, 0, writeBytes.Length, new AsyncCallback(WriteAsync), stream);
        }

        static public void Shutdown() {
            if(client != null) {
                client.Close();
            }

            readBytes = new byte[LENGTH];
            cmd = String.Empty;
            // Remove delegate connections???
        }

        static private void ReadAsync(IAsyncResult ar) {
            try {


                NetworkStream stream = (NetworkStream)ar.AsyncState;

                int numberOfBytesRead = stream.EndRead(ar);

                cmd += Encoding.ASCII.GetString(readBytes, 0, numberOfBytesRead);

                if (!stream.DataAvailable) {
                    Parse(cmd);
                    cmd = String.Empty; // Reset command
                    stream.Flush();
                }
                if (client.Connected) {
                    stream.BeginRead(readBytes, 0, readBytes.Length, new AsyncCallback(ReadAsync), stream);
                }
            } catch (SocketException e) {
                client.Close();
            }
            
        }

        static public void WriteChatMessage(string user, string message, string color = "#FFFFFFFF") {
            string cmd = "CHAT -username:" + user + "-message:" + message + "-color:" + color;
            writeBytes = ASCIIEncoding.UTF8.GetBytes(cmd);

            NetworkStream stream = client.GetStream();
            stream.BeginWrite(writeBytes, 0, writeBytes.Length, new AsyncCallback(WriteAsync), stream);
        }

        static private void WriteAsync(IAsyncResult ar) {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            stream.EndWrite(ar);

            //Console.Write("What would you like to send? ");
            //string mess = Console.ReadLine();
            //writeBytes = ASCIIEncoding.UTF8.GetBytes(mess + '\0');
            //stream.BeginWrite(writeBytes, 0, writeBytes.Length, new AsyncCallback(WriteAsync), stream);
            // Change to occur at specific intervals...
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
            int color = 16777215; // WHITE ? OR BLACK?
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
                        int.TryParse(data, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color);
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

        static private void ParseHelpCmd(string cmd) {
            string error = "";

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
                        break;
                    default:
                        break;
                }
            }

            // Chat Command Error Delegate
            helpDelegate?.Invoke(error);
        }
    }
}
