using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;

namespace DatabaseConnectionService {
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class DatabaseConnService : IDatabaseConn {

        public string AddNewUser(string userName, string password) {
            string errorString = "";
            try {
                using (SqlConnection conn = new SqlConnection("Server=tcp:wipserver.database.windows.net,1433;" +
                       "Initial Catalog=WIPUsers;" +
                       "Persist Security Info=False;" +
                       "User ID=WIPStudios;" +
                       "Password=741stQRHMsNn14D5;" +
                       "MultipleActiveResultSets=False;" +
                       "Encrypt=True;" +
                       "TrustServerCertificate=False;" +
                       "Connection Timeout=30;")) {

                    conn.Open();

                    SqlCommand findUser = new SqlCommand("SELECT username from Users" +
                        "WHERE username = @name " +
                        "AND password = @password COLLATE SQL_Latin1_General_CP1_CS_AS;", conn);
                    findUser.Parameters.AddWithValue("@name", userName);
                    findUser.Parameters.AddWithValue("@password", password);

                    bool isNotFound = false;
                    using (SqlDataReader reader = findUser.ExecuteReader()) {
                        isNotFound = reader.HasRows;

                    }
                    if (isNotFound) { 
                        SqlCommand addUser = new SqlCommand("INSERT into Users (username, password) " +
                            "Values (@name, @password);");
                        addUser.Parameters.AddWithValue("@name", userName);
                        addUser.Parameters.AddWithValue("@password", password);
                        using (SqlDataReader reader = addUser.ExecuteReader()) {
                            if (!reader.HasRows) {
                                errorString = "Could not add User!";
                            }
                        }
                    } else {
                        errorString = "User already exists";
                    }

                    conn.Close();
                }
            } catch (SqlException e) {
                errorString = e.ToString();
            }
            return errorString;
        }

        public string CheckLogin(string userName, string password) {
            string errorString = "";
            try {
                using (SqlConnection conn = new SqlConnection("Server=tcp:wipserver.database.windows.net,1433;" +
                       "Initial Catalog=WIPUsers;" +
                       "Persist Security Info=False;" +
                       "User ID=WIPStudios;" +
                       "Password=741stQRHMsNn14D5;" +
                       "MultipleActiveResultSets=False;" +
                       "Encrypt=True;" +
                       "TrustServerCertificate=False;" +
                       "Connection Timeout=30;")) {

                    conn.Open();

                    SqlCommand findUser = new SqlCommand("SELECT username from Users " +
                        "WHERE username = @name " +
                        "AND password = @password COLLATE SQL_Latin1_General_CP1_CS_AS;", conn);
                    findUser.Parameters.AddWithValue("@name", userName);
                    findUser.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = findUser.ExecuteReader()) {
                        if (!reader.HasRows) {
                            errorString = "No valid user Found";
                        }
                    }

                    conn.Close();
                }
            } catch(SqlException e) {
                errorString = e.ToString();
            }
            return errorString;
        }

        public string GetConnenctionToDB() {
            SqlConnection conn;
            try {
                conn = new SqlConnection();
                conn.ConnectionString = "Server=tcp:wipserver.database.windows.net,1433;" +
                       "Initial Catalog=WIPUsers;" +
                       "Persist Security Info=False;" +
                       "User ID=WIPStudios;" +
                       "Password=741stQRHMsNn14D5;" +
                       "MultipleActiveResultSets=False;" +
                       "Encrypt=True;" +
                       "TrustServerCertificate=False;" +
                       "Connection Timeout=30;";

                conn.Open();

                conn.Close();

                return "";
            } catch (SqlException e) {

                return e.ToString();
            }  
        }
        
    }
}
