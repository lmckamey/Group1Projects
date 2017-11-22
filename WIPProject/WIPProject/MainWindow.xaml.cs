﻿using System;
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
                RoomManager.Initialize();

                this.Close();
            } 
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (DatabaseConnection.AddUserLogin(tbxCreateUsername.Text, pbxCreatePassword.Password)) {
                this.Hide();

                DrawingPage dp = new DrawingPage();
                dp.userName = tbxCreateUsername.Text;
                dp.ShowDialog();

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
    }
}
