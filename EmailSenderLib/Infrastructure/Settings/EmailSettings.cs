using System;
using System.Collections.Generic;
using System.Text;

namespace EmailSenderLib.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string Provider { get; set; } = string.Empty;
        public Smtp? Smtp { get; set; }
        public MailKit? MailKit { get; set; }
        public GraphApi? GraphApi { get; set; }
        public Exchange? Exchange { get; set; }
        public SendGrid? SendGrid { get; set; }
        public Mailgun? Mailgun { get; set; }
        public AmazonSes? AmazonSes { get; set; }
        public Postmark? Postmark { get; set; }
    }
}
