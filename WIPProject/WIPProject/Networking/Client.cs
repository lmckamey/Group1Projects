using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using System.Net.Sockets;
using System.IO;

namespace WIPProject.Networking {

    class Reader {
        StreamReader writer;
        public Reader(NetworkStream nStream) {
            writer = new StreamReader(nStream);
        }

        public void Run() {
            while (true) {

            }
        }
    }

    class Writer {
        StreamWriter writer;
        public Writer(NetworkStream nStream) {
            writer = new StreamWriter(nStream);
        }

        public void Run() {
            while (true) {
                writer.WriteLine()
            }
        }
    }

    public static class Client {

        private static TcpClient client;
        private static Reader clientReader;
        private static Writer clientWriter;

        public static async Task<bool> Join() {
            await Leave();

            try {
                client = new TcpClient();
                await client.ConnectAsync("40.69.169.63", 10100);

                // Create Writer Thread
                clientWriter = new Writer(client.GetStream());
                clientReader = new Reader(client.GetStream());
                // Create Reader Thread

                //Guid id = new Guid()

                return true;
            } catch (SocketException e) {
                MessageBox.Show(e.Message, "Networking Error");
            }
            return false;
        }

        public static async Task<bool> Leave() {
            bool error = false;
            try {
                if (client != null) {
                    // Tell Server we are disconnecting, Use Await
                    client.Close();

                    error = true;
                }
            } catch (SocketException e) {
                MessageBox.Show(e.Message, "Networking Error");
            }

            

            client = null;
            clientWriter = null;
            clientWriter = null;
            return error;
        }

    }
}
