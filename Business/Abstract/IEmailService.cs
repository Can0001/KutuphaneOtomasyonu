using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
