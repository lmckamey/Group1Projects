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

            ChatRooms[currRoom].Active = true;

            Client.Add(ChatMessage);
            Client.Add(HelpMessage);
            Client.Add(DrawMessage);
            Client.Initialize();

            ChatRooms[currRoom].ShowDialog();
        }

        private static void ChatMessage(string username, string message, string color) {
            ChatRooms[currRoom].AddMessage(username, message, color);
        }

        private static void HelpMessage(Client.CmdType type, string msg) {
            switch (type) {
                case Client.CmdType.CLEAR:
                    ChatRooms[currRoom].ClearDrawing();
                    break;
                case Client.CmdType.ERASE:
                    int index;
                    int.TryParse(msg, out index);
                    ChatRooms[currRoom].EraseDrawing(index);
                    break;
                case Client.CmdType.ERROR:
                    MessageBox.Show(msg, "Server sent an Error");
                    break;
                case Client.CmdType.FILL:
                    ChatRooms[currRoom].FillDrawing(msg);
                    break;
                case Client.CmdType.REQUEST:
                    // ?????
                    break;
                case Client.CmdType.SIGNAL:
                    ChatRooms[currRoom].ToggleDrawing();
                    break;
                case Client.CmdType.UNDO:
                    int amo;
                    int.TryParse(msg, out amo);
                    ChatRooms[currRoom].UndoDrawing(amo);
                    break;
                default:
                    break;
            }
        }

        private static void DrawMessage(string[] lines) {
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
