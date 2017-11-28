using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WIPProject.Models
{
    public class RoomManager
    {
        private static DrawingPage[] ChatRooms;
        public static MainWindow mainWindow;
        public static int NumberOfChatRooms;

        public static void Initialize(int numberOfRooms = 10)
        {
            CreateChatRooms(numberOfRooms);

            ChatRooms[0].ShowDialog();
        }

        private static void CreateChatRooms(int roomCount)
        {
            NumberOfChatRooms = roomCount;

            ChatRooms = new DrawingPage[roomCount];

            for (int i = 0; i < roomCount; ++i)
            {
                ChatRooms[i] = new DrawingPage(mainWindow);
            }
        }

        public static DrawingPage JoinRoom(DrawingPage sender, int roomNumber)
        {
            sender.Hide();

            DrawingPage room = ChatRooms[roomNumber];
            
            room.Left = sender.Left;
            room.Top = sender.Top;
            room.Width = sender.ActualWidth;
            room.Height = sender.ActualHeight;
            room.Show();

            return ChatRooms[roomNumber];
        }
    }
}
