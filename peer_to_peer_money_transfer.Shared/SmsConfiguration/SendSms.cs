using Microsoft.Extensions.Logging;
using peer_to_peer_money_transfer.Shared.Interfaces;
using System.Net.Http.Json;

namespace peer_to_peer_money_transfer.Shared.SmsConfiguration
{
    public class SendSms : ISendSms
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SendSms> _logger;

        string token = "cuX1cPE3aJNeDxhdkqo7qI7zLGWrsXBybNuaNEVy9yksC1XSz9FsUVqcWkJe";
        public SendSms(IHttpClientFactory httpClientFactory, ILogger<SendSms> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<string> SendSmsAsync(SmsModel model)
        {
            var client = _httpClientFactory.CreateClient("SmsClient");

            try
            {
                await client.GetFromJsonAsync<SmsModel>(
                $"?api_token={token} &from=CashMingle&to={model.ReceiversPhoneNumber}&body={model.MessageBody}&dnd=2");

                _logger.LogError("Message sent successfully");
                return "Sms sent successfully";
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Message could not be sent");
                return "Message could not be sent";
            }
        }
    }
}
