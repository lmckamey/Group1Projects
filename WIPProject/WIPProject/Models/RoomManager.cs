using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIPProject.Models
{
    public class RoomManager
    {
        private static DrawingPage[] ChatRooms;
        public static int NumberOfChatRooms;

        public static void Initialize()
        {
            CreateChatRooms(10);

            ChatRooms[0].ShowDialog();
        }

        private static void CreateChatRooms(int roomCount = 10)
        {
            NumberOfChatRooms = roomCount;

            ChatRooms = new DrawingPage[roomCount];

            for (int i = 0; i < roomCount; ++i)
            {
                ChatRooms[i] = new DrawingPage();
            }
        }

        public static void JoinRoom(DrawingPage sender, int roomNumber)
        {
            sender.Close();

            ChatRooms[roomNumber].Show();
        }
    }
}
