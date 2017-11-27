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
using System.Windows.Shapes;

namespace WIPProject
{
    /// <summary>
    /// Interaction logic for DrawingPage.xaml
    /// </summary>
    public partial class DrawingPage : Window
    {
        public string userName;

        public DrawingPage()
        {
            InitializeComponent();
        }

        private void txbChatBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                lblTextWatermark.Visibility = Visibility.Visible;
                SendMessage();
            }
            else if (tbxChatBox.GetLineText(0).Length >= 0 && !e.Key.Equals(Key.Back) 
                && (e.Key >= Key.A && e.Key <= Key.Z))
            {
                lblTextWatermark.Visibility = Visibility.Hidden;
            }
            else if (e.Key.Equals(Key.Back) && tbxChatBox.GetLineText(0).Length <= 1)
            {
                lblTextWatermark.Visibility = Visibility.Visible;
            }
        }

        private void SendMessage()
        {
            tblChatWindow.Text = $"{tblChatWindow.Text}\n{userName}: {tbxChatBox.Text}";
            //tbxChatWindow.AppendText($"\n{userName}: {tbxChatBox.Text}\n");

            tbxChatBox.Clear();

            scvChatScrollbar.ScrollToBottom();
        }

        private void btnModeChange_Click(object sender, RoutedEventArgs e)
        {
            ReverseVisibility(uscBasicDrawing);
            ReverseVisibility(uscViewer);
        }

        private void ReverseVisibility(UIElement element)
        {
            if (element.Visibility == Visibility.Hidden)
            {
                element.Visibility = Visibility.Visible;
            }
            else
            {
                element.Visibility = Visibility.Hidden;
            }
        }

        private void btnRoomSelect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

/*
 * WPF con
 * TCP
 * UDP 
 * SOCKET
 *  
 *  
 *  
 *  
 *  
 */
