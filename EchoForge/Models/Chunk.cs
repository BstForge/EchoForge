namespace EchoForge.Models
{
    public class Chunk
    {
        public string Voice { get; set; } = string.Empty;
        public int Speed { get; set; }
        public string Tone { get; set; } = string.Empty;
        public int Pacing { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}