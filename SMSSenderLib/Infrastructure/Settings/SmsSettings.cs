using System;
using System.Collections.Generic;
using System.Text;

namespace SMSSenderLib.Infrastructure.Settings
{
    public class SmsSettings
    {
        public string Provider { get; set; } = string.Empty;
        
        // Global/Enterprise providers
        public Twilio? Twilio { get; set; }
        public Vonage? Vonage { get; set; }
        public MessageBird? MessageBird { get; set; }
        public Sinch? Sinch { get; set; }
        public Gatewayapi? Gatewayapi { get; set; }
        
        // Africa/Kenya providers
        public AfricasTalking? AfricasTalking { get; set; }
        public IntaSend? IntaSend { get; set; }
        public Safaricom? Safaricom { get; set; }
        
        // Local modem
        public LocalModem? LocalModem { get; set; }
    }
}
