using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Instance
{
    public class EmailInstance
    {
        private static object _syncObject = new object();
        private static EmailInstance _inst { get; set; }
        public static EmailInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) _inst = new EmailInstance();
                    }
                return _inst;
            }
        }

        public void SendGmailMail(string address, string subject, string mailbody) {
            string to = "toaddress@gmail.com"; //To address    
            string from = address; //From address    
            MailMessage message = new MailMessage(from, to);

            //string mailbody = "In this article you will learn how to send a email using Asp.Net & C#";
            message.Subject = subject;// "Sending Email Using Asp.Net & C#";
            message.Body = mailbody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("yourmail id", "Password");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
