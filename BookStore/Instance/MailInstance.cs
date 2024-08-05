using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Extensions;
using BookStore.Utils;
using DAO.DAOImp;
using LoggerService;
using MailKit.Net.Smtp;
using MimeKit;

namespace BookStore.Instance
{
    public class MailInstance
    {
        private static object _syncObject = new object();
        private static MailInstance _inst { get; set; }
        public static MailInstance Inst
        {
            get
            {
                if (_inst == null)
                    lock (_syncObject)
                    {
                        if (_inst == null) {
                            _inst = new MailInstance();
                            _logger = new LoggerManager();
                        }
                    }
                return _inst;
            }
        }
        private static ILoggerManager _logger;

        public void SendMailRegis(long accountId)
        {
            string nickNameSend = "admin@gmail.com";
            string mailHeader = "Chào mừng bạn đến với Gamma!";
            string mailContent = "Chúc bạn có các trải nghiệm tốt đẹp với chúng tôi.";
            int responseStatus = -99;
            string _Description = "";//json khuyen mai
            StoreMailSqlInstance.Inst.SendMail(accountId, nickNameSend, mailHeader, mailContent, out responseStatus, 0, _Description);
            if (responseStatus < 0){
                Task.Run(async () => await _logger.LogError("SQL-SendMailRegis()", "SendMailRegis "+accountId).ConfigureAwait(false));
            }
        }

        public void SendGmailMail(string address, string subject, string mailbody)
        {
            //string to = address; //To address    
            //string from = "hungthan9x9@gmail.com"; //From address    
            //MailMessage message = new MailMessage(from, to);

            ////string mailbody = "In this article you will learn how to send a email using Asp.Net & C#";
            //message.Subject = subject;// "Sending Email Using Asp.Net & C#";
            //message.Body = mailbody;
            //message.BodyEncoding = Encoding.UTF8;
            //message.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            //System.Net.NetworkCredential basicCredential1 = new
            //System.Net.NetworkCredential("yourmail id", "Password");
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = basicCredential1;
            //try
            //{
            //    client.Send(message);
            //}

            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x, x)));
            Subject = subject;
            Content = content;
        }
    }
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }

    public class EmailSender : IEmailSender
    {
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }
        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);
            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", CONFIG.EmailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.Connect(CONFIG.EmailConfig.SmtpServer, CONFIG.EmailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(CONFIG.EmailConfig.UserName, CONFIG.EmailConfig.Password);
                    client.Send(mailMessage);
                    client.Disconnect(true);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    string username = "";
                    string Password = "";
                    var responseCode = StoreMailSqlInstance.Inst.GetEmailSaveDatabase(out username, out Password);
                    if (responseCode < 0)
                        return;
                    await client.ConnectAsync(CONFIG.EmailConfig.SmtpServer, CONFIG.EmailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(username, Password);
                    //await client.AuthenticateAsync(CONFIG.EmailConfig.UserName, CONFIG.EmailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception, or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
