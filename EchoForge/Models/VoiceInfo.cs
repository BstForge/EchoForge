namespace EchoForge.Models
{
    public class VoiceSettings
    {
        public float stability { get; set; }
        public float similarity_boost { get; set; }
    }

    public class VoiceFineTuning
    {
        public bool is_available { get; set; }
        public string? model_id { get; set; }
    }

    public class VoiceInfo
    {
        public string name { get; set; } = string.Empty;
        public string voice_id { get; set; } = string.Empty;
        public Dictionary<string, string>? labels { get; set; }
        public string? preview_url { get; set; }
        public VoiceSettings? settings { get; set; }
        public VoiceFineTuning? fine_tuning { get; set; }

        // selections
        public bool IsNarrator { get; set; }
        public bool IsDialog { get; set; }
        public bool DoNotUse { get; set; }
    }

    public class VoiceResponse
    {
        public List<VoiceInfo> voices { get; set; } = new();
    }
}