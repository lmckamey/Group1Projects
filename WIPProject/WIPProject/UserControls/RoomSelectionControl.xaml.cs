using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WIPProject.Models;

namespace WIPProject.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class RoomSelectionControl : UserControl
    {
        public DrawingPage page;
        public RoomSelectionControl()
        {
            InitializeComponent();
        }

        private void brdRoom1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border b = ((Border)sender);
            string s = (string)((Label)b.Child).Content;
            SelectRoom(s);

            this.Visibility = Visibility.Hidden;
        }

        private void SelectRoom(string room)
        {
            int r;
            int.TryParse(room, out r);

            DrawingPage p = RoomManager.JoinRoom(page, r - 1);

            p.uscBasicDrawing.ignoreNextLines = true;
        }
    }
}
