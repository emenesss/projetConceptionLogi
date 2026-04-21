using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjetConception.Models
{
    public class DecoderApiService
    {
        private readonly HttpClient _httpClient;

        public DecoderApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> SendActionAsync(string address, string action)
        {
            var postDataValues = new
            {
                id = "ESSI86300500",
                address = address,
                action = action
            };

            var json = JsonSerializer.Serialize(postDataValues);
            var postData = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://wflageol-uqtr.net/decoder", postData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return $"Erreur : {(int)response.StatusCode} {response.ReasonPhrase}";
        }
    }
}