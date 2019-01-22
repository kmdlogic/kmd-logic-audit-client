
using System;

namespace Kmd.Logic.Audit.Client.Sample
{
    public class ProgramConfig
    {
        public ProgramConfigIngestion Ingestion { get; set; } = new ProgramConfigIngestion();
        public ProgramConfigClient Client { get; set; } = new ProgramConfigClient();
    }
    
    public class ProgramConfigClient
    {
        public bool EnrichFromLogContext { get; set; } = false;
    }

    public class ProgramConfigIngestion
    {
        public Uri Endpoint { get; set; } = new Uri("http://localhost:5341/");
        public string ApiKey { get; set; } = null;
    }
}
