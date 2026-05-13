using Business.Abstract;
using Microsoft.Extensions.Configuration; 
using System.Net;
using System.Net.Mail;

namespace Business.Concrete
{
    public class EmailManager : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            string fromMail = _configuration["EmailSettings:Email"];
            string fromPassword = _configuration["EmailSettings:Password"];

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail, "BibliosHub Kütüphane");
            message.Subject = subject;
            message.To.Add(new MailAddress(toEmail));
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
        }
    }
}