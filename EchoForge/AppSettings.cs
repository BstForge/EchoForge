using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoForge.Config
{
    public class AppSettings
    {
        public string DefaultVoice { get; set; } = "Rachel";
        public string DefaultTone { get; set; } = "calm";
        public bool UseLocalStorage { get; set; } = true;
    }
}