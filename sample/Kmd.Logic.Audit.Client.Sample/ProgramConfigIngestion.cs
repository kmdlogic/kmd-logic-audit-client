using System;

namespace Kmd.Logic.Audit.Client.Sample
{
    public class ProgramConfigIngestion
    {
        public Uri Endpoint { get; set; } = new Uri("http://localhost:5341/");

        public string ApiKey { get; set; } = null;
    }
}
