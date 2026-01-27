using SMSSenderLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMSSenderLib.Implementations
{
    public class MessageBirdSmsSender : ISmsSender
    {
        private readonly string _apiKey;
        private readonly string _originator;
        private readonly string? _reference;

        public MessageBirdSmsSender(string apiKey, string originator, string? reference = null)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _originator = originator ?? throw new ArgumentNullException(nameof(originator));
            _reference = reference;
        }

        public async Task SendSmsAsync(string to, string message)
        {
            await SendSmsAsync(new List<string> { to }, message);
        }

        public async Task SendSmsAsync(List<string> recipients, string message)
        {
            // TODO: Implement MessageBird SMS sending
            // Requires MessageBird NuGet package
            // Example implementation:
            // var client = Client.CreateDefault(_apiKey);
            // var messageRequest = new MessageBird.Objects.Message
            // {
            //     Originator = _originator,
            //     Recipients = recipients,
            //     Body = message,
            //     Reference = _reference
            // };
            // var result = await client.SendMessageAsync(messageRequest);
            await Task.CompletedTask;
            throw new NotImplementedException("MessageBird SMS sending requires MessageBird package implementation");
        }
    }
}
