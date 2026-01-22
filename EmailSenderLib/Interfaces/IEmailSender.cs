using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSenderLib.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendEmailAsync(string to, string subject, string body, List<string> attachments, bool isHtml = false);

    }
}
