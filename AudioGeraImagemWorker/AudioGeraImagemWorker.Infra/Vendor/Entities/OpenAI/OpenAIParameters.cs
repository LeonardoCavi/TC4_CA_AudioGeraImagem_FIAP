using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Infra.Vendor.Entities.OpenAI
{
    [ExcludeFromCodeCoverage]
    public class OpenAIParameters
    {
        public string SecretKey { get; set; }
        public OpenAIEndPointParameters TranscriptionParameters { get; set; }
        public OpenAIEndPointParameters ImageGeneratorParameters { get; set; }
    }

    public class OpenAIEndPointParameters
    {
        public string Url { get; set; }
        public string Model { get; set; }
    }
}
