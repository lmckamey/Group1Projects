using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            Client.Add(AddMessage);
            Client.Initialize();

            CreateChatRooms(numberOfRooms);

            ChatRooms[currRoom].Active = true; 
            ChatRooms[currRoom].ShowDialog();
        }

        private static void AddMessage(string username, string message, int color) {
            ChatRooms[currRoom].AddMessage($"{username}: {message}");
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
