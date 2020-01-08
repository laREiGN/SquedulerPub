using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace sqeudulerApp.Scripts
{
    public class Extra
    {
        public static string Generate_Random_String(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //new empty temp password
            string New_pass = "";
            Random rng = new Random();

            for (int i = 0; i < length; i++)
            {
                //Add a new random character to newpassword
                New_pass += chars[rng.Next(0, chars.Length)];
            }
            return New_pass;
        }

        public static int GetCurrentUserID(string uid)
        {
            string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            // takes current session user id (email in this case)
            string currentUser = uid;

            //connection opened to database
            using SqlConnection conn = new SqlConnection(strCon);

            // sql query, that reads userid's associated to the current users email
            string query = "SELECT [UserId] FROM [dbo].[User] WHERE [Email]= '" + currentUser + "';";

            // block of code, that ready the results of the above query
            using SqlCommand comm = new SqlCommand(query, conn);
            conn.Open();
            SqlDataReader sqlResultReader = comm.ExecuteReader();

            //checks if the query actually returned any data
            if (sqlResultReader.Read())
            {
                // the top result of the above query is saved as team owner in the teams table, and the reader is closed
                int currentUserID = Convert.ToInt32(sqlResultReader[0].ToString());
                conn.Close();
                return currentUserID;
            }
            return 0;
        }


        /// <summary>
        ///  /// Sents a email to target mail
        /// </summary>
        /// smtp host, example: smtp.gmail.com
        /// <param name="Stmp_host"></param>
        /// Port number of the stmp host
        /// <param name="Port"></param>
        /// <param name="Reciever_Email"></param>
        /// <param name="Sender_Email"></param>
        /// <param name="Subject"></param>
        /// <param name="Message"></param>
        public static void Sent_Email(string Stmp_host, int Port, string Reciever_Email, string Sender_Email, string Subject, string Message)
        {
            //                                    
            SmtpClient smtpClient = new SmtpClient(Stmp_host, Port);

            MailMessage msg = new MailMessage();
            SmtpClient client = new SmtpClient();
            try
            {
                //adds subject to email
                msg.Subject = Subject;
                //adds subject to email
                msg.Body = Message;
                //adds subject to email
                msg.From = new MailAddress(Sender_Email);
                //adds subject to email
                msg.To.Add(Reciever_Email);
                //sets email message/body to html
                msg.IsBodyHtml = true;
                //sets host
                client.Host = Stmp_host;
                //Sign in info for email network
                System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential(Sender_Email, "@SquedulerEmailServerPassword9710");
                //sets port
                client.Port = Port;
                //enables ssl
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                //sets credentials
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //sends email
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
