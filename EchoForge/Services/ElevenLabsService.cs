using EchoForge.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace EchoForge.Services
{
    public class ElevenLabsService
    {
        public async Task<List<VoiceInfo>> GetVoicesAsync(string apiKey)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("xi-api-key", apiKey);
            var resp = await client.GetAsync("https://api.elevenlabs.io/v1/voices");
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<VoiceResponse>(json);
            return data?.voices ?? new List<VoiceInfo>();
        }
    }
}