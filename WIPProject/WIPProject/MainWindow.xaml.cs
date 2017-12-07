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

using WIPProject.Database;
using WIPProject.Models;

using System.Net.Sockets;
using System.IO;

namespace WIPProject {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {

            InitializeComponent();

        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e) {

            if (DatabaseConnection.CheckUserLogin(tbxUserName.Text, pbxPassword.Password)){ 
                this.Hide();

                //DrawingPage dp = new DrawingPage();
                //dp.userName = tbxUserName.Text;
                //dp.ShowDialog();
                RoomManager.mainWindow = this;
                RoomManager.username = tbxUserName.Text;
                RoomManager.Initialize();

                this.Close();
            } 
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseConnection.AddUserLogin(tbxCreateUsername.Text, pbxCreatePassword.Password)) {
                this.Hide();

                RoomManager.mainWindow = this;
                RoomManager.username = tbxUserName.Text;
                RoomManager.Initialize();

                this.Close();
            }
        }

        private void pbxPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Return))
            {
                btnSignIn_Click(sender, null);
            }
        }

        private void tbxUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            if((sender as TextBox).Text == "Username")
            {
                (sender as TextBox).Text = "";
                (sender as TextBox).Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void tbxUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                (sender as TextBox).Text = "Username";
                (sender as TextBox).Foreground = new SolidColorBrush(Colors.Silver);
            }
        }

        private void pbxCreatePassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if((string)lblPasstemp2.Content == "Password")
            {
                lblPasstemp2.Content = "";
            }
        }

        private void pbxCreatePassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pbxCreatePassword.Password == "")
            {
                lblPasstemp2.Content = "Password";
            }
        }

        private void pbxPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((string)lblPasstemp1.Content == "Password")
            {
                lblPasstemp1.Content = "";
            }
        }

        private void pbxPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            if (pbxPassword.Password == "")
            {
                lblPasstemp1.Content = "Password";
            }
        }
    }
}