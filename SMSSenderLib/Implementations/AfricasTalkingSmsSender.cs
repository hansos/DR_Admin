using SMSSenderLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMSSenderLib.Implementations
{
    public class AfricasTalkingSmsSender : ISmsSender
    {
        private readonly string _username;
        private readonly string _apiKey;
        private readonly string _from;
        private readonly string? _environment;

        public AfricasTalkingSmsSender(string username, string apiKey, string from, string? environment = "production")
        {
            _username = username ?? throw new ArgumentNullException(nameof(username));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _from = from ?? throw new ArgumentNullException(nameof(from));
            _environment = environment ?? "production";
        }

        public async Task SendSmsAsync(string to, string message)
        {
            await SendSmsAsync(new List<string> { to }, message);
        }

        public async Task SendSmsAsync(List<string> recipients, string message)
        {
            // TODO: Implement Africa's Talking SMS sending
            // Requires AfricasTalkingCS NuGet package
            // Example implementation:
            // var gateway = new AfricasTalkingGateway(_username, _apiKey, _environment);
            // foreach (var recipient in recipients)
            // {
            //     var response = await gateway.SendMessage(
            //         to: recipient,
            //         message: message,
            //         from: _from
            //     );
            // }
            // Or use the newer AfricasTalking SDK:
            // var sms = new SmsService(_username, _apiKey);
            // var response = await sms.SendAsync(
            //     message: message,
            //     from: _from,
            //     to: recipients
            // );
            await Task.CompletedTask;
            throw new NotImplementedException("Africa's Talking SMS sending requires AfricasTalking package implementation");
        }
    }
}
