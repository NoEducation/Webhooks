using AirlineSendAgent.Dtos;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirlineSendAgent.Client
{
    public class WebhookClient : IWebhookClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public WebhookClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendWebhookNotification(FlightDetailChangePayloadDto flightDetailChangePayloadDto)
        {
            var serializedPayload = JsonSerializer.Serialize(flightDetailChangePayloadDto);

            var httpClient = this._httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, flightDetailChangePayloadDto.WebhookUri);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedPayload);

            request.Content.Headers.ContentType  = new MediaTypeWithQualityHeaderValue("application/json");

            try
            {
                using var response = await httpClient.SendAsync(request);
                Console.WriteLine($"Successfull");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unsuccessful {ex.Message}");
            }
        }
    }
}
