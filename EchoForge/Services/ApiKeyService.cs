using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EchoForge.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IConfiguration _configuration;

        public ApiKeyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetOpenAIApiKey()
        {
            return _configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key is missing.");
        }

        public string GetElevenLabsApiKey()
        {
            return _configuration["ElevenLabs:ApiKey"] ?? throw new InvalidOperationException("ElevenLabs API Key is missing.");
        }
    }
}
