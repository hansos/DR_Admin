using SMSSenderLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMSSenderLib.Implementations
{
    public class GatewayapiSmsSender : ISmsSender
    {
        private readonly string _apiToken;
        private readonly string _sender;
        private readonly string? _destinationAddress;

        public GatewayapiSmsSender(string apiToken, string sender, string? destinationAddress = null)
        {
            _apiToken = apiToken ?? throw new ArgumentNullException(nameof(apiToken));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _destinationAddress = destinationAddress;
        }

        public async Task SendSmsAsync(string to, string message)
        {
            await SendSmsAsync(new List<string> { to }, message);
        }

        public async Task SendSmsAsync(List<string> recipients, string message)
        {
            // TODO: Implement Gatewayapi SMS sending
            // Requires HTTP client implementation (no official SDK)
            // Example implementation using HttpClient:
            // using var httpClient = new HttpClient();
            // httpClient.DefaultRequestHeaders.Authorization = 
            //     new AuthenticationHeaderValue("Bearer", _apiToken);
            // 
            // var payload = new
            // {
            //     sender = _sender,
            //     message = message,
            //     recipients = recipients.Select(r => new { msisdn = r }).ToList()
            // };
            // 
            // var response = await httpClient.PostAsJsonAsync(
            //     "https://gatewayapi.com/rest/mtsms", payload);
            await Task.CompletedTask;
            throw new NotImplementedException("Gatewayapi SMS sending requires HTTP client implementation");
        }
    }
}
