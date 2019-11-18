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
