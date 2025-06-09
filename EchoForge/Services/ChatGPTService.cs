using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
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
            sb.AppendLine("You are an expert literary parsing assistant helping to prepare audiobook scenes for high-quality AI voice narration using ElevenLabs.");
            sb.AppendLine();
            sb.AppendLine("You will receive a full scene of text containing both narration and dialogue. Your job is to break the scene into logical \"chunks\" that are each under 4800 characters, while maintaining the integrity of natural paragraph breaks and flow.");
            sb.AppendLine();
            sb.AppendLine("Depending on the selected narration style, follow one of these two formatting options:");
            sb.AppendLine();
            sb.AppendLine("Narration Style A: Single Narration");
            sb.AppendLine();
            sb.AppendLine("Output the entire scene in continuous, naturally flowing chunks.");
            sb.AppendLine();
            sb.AppendLine("Use the label format for each chunk:");
            sb.AppendLine("Chunk X:\nVoice: Narrator\nSpeed: [Your suggestion: 1–10]\nTone: [Your suggestion: 1–10]\nPacing: [Your suggestion: 1–10]\nText: [Parsed text with appropriate pauses]");
            sb.AppendLine("Within each chunk, insert three periods (...) at the end of each paragraph to signal a 0.25-second pause.");
            sb.AppendLine();
            sb.AppendLine("Narration Style B: Narrator & Dialog Separation");
            sb.AppendLine();
            sb.AppendLine("Split narration and dialogue into separate chunks. Each chunk should contain only narration or only spoken dialogue.");
            sb.AppendLine();
            sb.AppendLine("Dialogue chunks must be written exactly as spoken, without any narration attached.");
            sb.AppendLine();
            sb.AppendLine("Use these labels:\nChunk X:\nVoice: Narrator or Dialog\nSpeed: [Your suggestion: 1–10]\nTone: [Your suggestion: 1–10]\nPacing: [Your suggestion: 1–10]\nText: [Parsed text with appropriate pauses]");
            sb.AppendLine("Identify and extract the spoken dialogue by characters and present it in its own chunk.");
            sb.AppendLine("Keep narration in separate chunks that describe events, settings, or actions.");
            sb.AppendLine("As above, insert ... at paragraph ends to create pause cues.");
            sb.AppendLine();
            sb.AppendLine("Additional Instructions:");
            sb.AppendLine();
            sb.AppendLine("Maintain the chronological integrity of the original scene.");
            sb.AppendLine("Each chunk must be cohesive, well-formed, and ready to be matched with voice tone sliders later.");
            sb.AppendLine("Match suggested Speed, Tone, and Pacing values based on emotional and narrative context. Use values from 1 (low/slow/mild) to 10 (intense/fast/urgent).");
            sb.AppendLine("Do not include any summaries or explanations in the output.");
            sb.AppendLine("Return only the structured list of chunks, in numbered order, with no extra commentary.");

            if (narrationAndDialog)
            {
                sb.AppendLine();
                sb.AppendLine("Use Narration Style B.");
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine("Use Narration Style A.");
            }

            return sb.ToString();
        }
    }
}
