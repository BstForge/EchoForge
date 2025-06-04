using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoForge.Services
{
    public interface IApiKeyService
    {
        string GetOpenAIApiKey();
        string GetElevenLabsApiKey();
    }
}
