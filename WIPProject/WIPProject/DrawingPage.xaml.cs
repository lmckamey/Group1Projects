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
        public DrawingPage()
        {
            InitializeComponent();
        }

        private void txbChatBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                lblTextWatermark.Visibility = Visibility.Visible;
            }
            else if (txbChatBox.GetLineText(0).Length >= 0 && !e.Key.Equals(Key.Back))
            {
                lblTextWatermark.Visibility = Visibility.Hidden;
            }
            else if (e.Key.Equals(Key.Back) && txbChatBox.GetLineText(0).Length <= 1)
            {
                lblTextWatermark.Visibility = Visibility.Visible;
            }
        }
    }
}
