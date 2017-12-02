using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using WIPProject.Networking;

namespace WIPProject.Models
{
    public class RoomManager
    {
        private static DrawingPage[] ChatRooms;
        public static MainWindow mainWindow;
        public static string username;
        public static int NumberOfChatRooms;
        private static int currRoom = 0;

        public static void Initialize(int numberOfRooms = 10)
        {
            CreateChatRooms(numberOfRooms);

            ChatRooms[currRoom].Active = false;

            Client.Add(ChatMessage);
            Client.Add(HelpMessage);
            Client.Add(DrawMessage);
            Client.Initialize();

            ChatRooms[currRoom].ShowDialog();
        }

        private static void ChatMessage(string username, string message, int color) {
            ChatRooms[currRoom].AddMessage($"{username}: {message}");
        }

        private static void HelpMessage(Client.CmdType type, string msg) {
            if(type == Client.CmdType.ERROR) {
                MessageBox.Show(msg, "Server sent an Error");
            }
            if(type == Client.CmdType.SIGNAL) {
                ChatRooms[currRoom].ToggleDrawing();
            }
        }

        private static void DrawMessage(List<Line> lines) {
            ChatRooms[currRoom].DrawMessage(lines);
        }

        private static void CreateChatRooms(int roomCount)
        {
            NumberOfChatRooms = roomCount;

            ChatRooms = new DrawingPage[roomCount];

            for (int i = 0; i < roomCount; ++i)
            {
                ChatRooms[i] = new DrawingPage(false, mainWindow, username);
            }
        }

        public static DrawingPage JoinRoom(DrawingPage sender, int roomNumber)
        {
            sender.Hide();

            DrawingPage room = ChatRooms[roomNumber];
            currRoom = roomNumber;
            
            room.Left = sender.Left;
            room.Top = sender.Top;
            room.Width = sender.ActualWidth;
            room.Height = sender.ActualHeight;
            room.Show();

            return ChatRooms[roomNumber];
        }
    }
}
