using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
namespace Project_c_framework.Views.Home
{
    
    public class sql
    {
        private string connString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=tangodelta;Database=Scheduler";
        public bool Check_login(string name, string password)
        {
            //SQL Connection
            var connString = "Host=127.0.0.1;Port=5432;Username=postgres;Password=tangodelta;Database=Scheduler";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                // Retrieve all rows
                var cmd = new NpgsqlCommand("SELECT id, name, password FROM public.users;", conn);
                var reader = cmd.ExecuteReader();
                conn.Close();
                if (reader.FieldCount >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //No accounts found
            return false;
        }


        public bool Create_Account(String user, String password)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO public.users(id, name, password)VALUES (" + user + "," + password + ");";
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }

            }
            //Failed to create
            return false;
        }
    }
}
