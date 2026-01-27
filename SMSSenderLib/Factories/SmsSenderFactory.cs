using SMSSenderLib.Implementations;
using SMSSenderLib.Infrastructure.Settings;
using SMSSenderLib.Interfaces;
using System;

namespace SMSSenderLib.Factories
{
    public class SmsSenderFactory
    {
        private readonly SmsSettings _smsSettings;

        public SmsSenderFactory(SmsSettings smsSettings)
        {
            _smsSettings = smsSettings ?? throw new ArgumentNullException(nameof(smsSettings));
        }

        public ISmsSender CreateSmsSender()
        {
            return _smsSettings.Provider.ToLower() switch
            {
                "twilio" => _smsSettings.Twilio is not null
                    ? new TwilioSmsSender(
                        _smsSettings.Twilio.AccountSid,
                        _smsSettings.Twilio.AuthToken,
                        _smsSettings.Twilio.FromNumber,
                        _smsSettings.Twilio.MessagingServiceSid
                    )
                    : throw new InvalidOperationException("Twilio settings are not configured"),

                "vonage" => _smsSettings.Vonage is not null
                    ? new VonageSmsSender(
                        _smsSettings.Vonage.ApiKey,
                        _smsSettings.Vonage.ApiSecret,
                        _smsSettings.Vonage.FromNumber,
                        _smsSettings.Vonage.Brand
                    )
                    : throw new InvalidOperationException("Vonage settings are not configured"),

                "messagebird" => _smsSettings.MessageBird is not null
                    ? new MessageBirdSmsSender(
                        _smsSettings.MessageBird.ApiKey,
                        _smsSettings.MessageBird.Originator,
                        _smsSettings.MessageBird.Reference
                    )
                    : throw new InvalidOperationException("MessageBird settings are not configured"),

                "sinch" => _smsSettings.Sinch is not null
                    ? new SinchSmsSender(
                        _smsSettings.Sinch.ServicePlanId,
                        _smsSettings.Sinch.ApiToken,
                        _smsSettings.Sinch.FromNumber,
                        _smsSettings.Sinch.Region
                    )
                    : throw new InvalidOperationException("Sinch settings are not configured"),

                "gatewayapi" => _smsSettings.Gatewayapi is not null
                    ? new GatewayapiSmsSender(
                        _smsSettings.Gatewayapi.ApiToken,
                        _smsSettings.Gatewayapi.Sender,
                        _smsSettings.Gatewayapi.DestinationAddress
                    )
                    : throw new InvalidOperationException("Gatewayapi settings are not configured"),

                "africastalking" => _smsSettings.AfricasTalking is not null
                    ? new AfricasTalkingSmsSender(
                        _smsSettings.AfricasTalking.Username,
                        _smsSettings.AfricasTalking.ApiKey,
                        _smsSettings.AfricasTalking.From,
                        _smsSettings.AfricasTalking.Environment
                    )
                    : throw new InvalidOperationException("Africa's Talking settings are not configured"),

                "intasend" => _smsSettings.IntaSend is not null
                    ? new IntaSendSmsSender(
                        _smsSettings.IntaSend.ApiKey,
                        _smsSettings.IntaSend.PublicKey,
                        _smsSettings.IntaSend.SenderId,
                        _smsSettings.IntaSend.TestMode
                    )
                    : throw new InvalidOperationException("IntaSend settings are not configured"),

                "safaricom" => _smsSettings.Safaricom is not null
                    ? new SafaricomSmsSender(
                        _smsSettings.Safaricom.ConsumerKey,
                        _smsSettings.Safaricom.ConsumerSecret,
                        _smsSettings.Safaricom.ShortCode,
                        _smsSettings.Safaricom.InitiatorName,
                        _smsSettings.Safaricom.SecurityCredential,
                        _smsSettings.Safaricom.Environment,
                        _smsSettings.Safaricom.CallbackUrl
                    )
                    : throw new InvalidOperationException("Safaricom settings are not configured"),

                "localmodem" => _smsSettings.LocalModem is not null
                    ? new LocalModemSmsSender(
                        _smsSettings.LocalModem.ComPort,
                        _smsSettings.LocalModem.BaudRate,
                        _smsSettings.LocalModem.DataBits,
                        _smsSettings.LocalModem.Parity,
                        _smsSettings.LocalModem.StopBits,
                        _smsSettings.LocalModem.ReadTimeout,
                        _smsSettings.LocalModem.WriteTimeout,
                        _smsSettings.LocalModem.Pin
                    )
                    : throw new InvalidOperationException("LocalModem settings are not configured"),

                _ => throw new InvalidOperationException($"Unknown SMS provider: {_smsSettings.Provider}")
            };
        }
    }
}
