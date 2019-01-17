using System;

namespace Kmd.Logic.Audit.Client.SerilogSeq
{
    public class SerilogSeqAuditClientConfiguration
    {
        public Uri ServerUrl { get; set; }

        public string ApiKey { get; set; }

        public bool? EnrichFromLogContext { get; set; }
    }
}
