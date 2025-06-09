using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EchoForge.Services
{
    public class ChatGPTService
    {
        private readonly string _apiKey;

        public ChatGPTService(string apiKey)
        {
            _apiKey = apiKey;
        }

        private class Message
        {
            [JsonProperty("role")]
            public string Role { get; set; } = "";
            [JsonProperty("content")]
            public string Content { get; set; } = "";
        }

        private class Request
        {
            [JsonProperty("model")]
            public string Model { get; set; } = "gpt-3.5-turbo";
            [JsonProperty("messages")]
            public List<Message> Messages { get; set; } = new();
            [JsonProperty("temperature")]
            public double Temperature { get; set; } = 0.7;
        }

        private class Choice
        {
            [JsonProperty("message")]
            public Message Message { get; set; } = new();
        }

        private class Response
        {
            [JsonProperty("choices")]
            public List<Choice> Choices { get; set; } = new();
        }

        public async Task<string> ParseSceneAsync(string scene, bool narrationAndDialog)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var systemPrompt = GetSystemPrompt(narrationAndDialog);

            var req = new Request
            {
                Messages = new List<Message>
                {
                    new Message{ Role="system", Content=systemPrompt },
                    new Message{ Role="user", Content=scene }
                }
            };

            var json = JsonConvert.SerializeObject(req);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            resp.EnsureSuccessStatusCode();
            var respJson = await resp.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Response>(respJson);
            return data?.Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
        }

        private string GetSystemPrompt(bool narrationAndDialog)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are an expert literary parsing assistant preparing audiobook scenes for high quality narration.");
            sb.AppendLine();
            sb.AppendLine("Break the provided scene into logical chunks under 4800 characters each. Use natural paragraph breaks and maintain chronological order.");
            sb.AppendLine();
            sb.AppendLine("Each chunk must use this label format:");
            sb.AppendLine("Chunk X:\nVoice: Narrator or Dialog\nSpeed: 1-10\nTone: [descriptor]\nPacing: 1-10\nText: [chunk text]");
            sb.AppendLine();
            sb.AppendLine("Insert '...' at the end of a chunk whenever a pause or paragraph break is appropriate.");
            sb.AppendLine("Estimate speed, tone and pacing using punctuation and verbs like 'shouted', 'muttered' or 'asked'.");
            sb.AppendLine();
            sb.AppendLine("Narration Style A: Single Narration");
            sb.AppendLine("Return the entire parsed text as consecutive Narrator chunks.");
            sb.AppendLine();
            sb.AppendLine("Narration Style B: Narrator & Dialog");
            sb.AppendLine("Split narration and dialogue into separate chunks. Only quoted speech is included in dialog chunks. All other text is placed in narrator chunks.");
            sb.AppendLine();
            if (narrationAndDialog)
            {
                sb.AppendLine("Use Narration Style B.");
            }
            else
            {
                sb.AppendLine("Use Narration Style A.");
            }
            sb.AppendLine();
            sb.AppendLine("Return only the numbered list of chunks with no extra commentary.");
            return sb.ToString();
        }

        public static List<Models.Chunk> ParseChunks(string response)
        {
            var chunks = new List<Models.Chunk>();
            if (string.IsNullOrWhiteSpace(response))
                return chunks;

            var pattern = @"Chunk\s*\d+:\s*Voice:\s*(?<voice>.+?)\s*\nSpeed:\s*(?<speed>\d+)\s*\nTone:\s*(?<tone>.+?)\s*\nPacing:\s*(?<pacing>\d+)\s*\nText:\s*(?<text>.*?)(?=\nChunk|$)";
            foreach (Match m in Regex.Matches(response, pattern, RegexOptions.Singleline))
            {
                if (!int.TryParse(m.Groups["speed"].Value.Trim(), out var speed)) speed = 5;
                if (!int.TryParse(m.Groups["pacing"].Value.Trim(), out var pacing)) pacing = 5;

                chunks.Add(new Models.Chunk
                {
                    Voice = m.Groups["voice"].Value.Trim(),
                    Speed = speed,
                    Tone = m.Groups["tone"].Value.Trim(),
                    Pacing = pacing,
                    Text = m.Groups["text"].Value.Trim()
                });
            }

            return chunks;
        }
    }
}