using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WIPProject.Database {
    public static class DatabaseConnection {

        public static bool CheckUserLogin(string username, string password) {
            DatabaseConnClient client = new DatabaseConnClient();

            bool success = false;

            var error = client.CheckLogin(username, password);
            if(error == null) {
                MessageBox.Show("Invalid Error Code", "Invalid Error");
            } else if(error.Length > 0) {
                MessageBox.Show(error, "Error has been found");
            } else {
                success = true;
            }

            client.Close();

            return success;
        }

        public static bool AddUserLogin(string username, string password) {
            DatabaseConnClient client = new DatabaseConnClient();

            bool success = false;

            var error = client.AddNewUser(username, password);
            if (error == null) {
                MessageBox.Show("Invalid Error Code", "Invalid Error");
            } else if (error.Length > 0) {
                MessageBox.Show(error, "Error has been found");
            } else {
                success = true;
            }

            client.Close();

            return success;
        }

    }
}
