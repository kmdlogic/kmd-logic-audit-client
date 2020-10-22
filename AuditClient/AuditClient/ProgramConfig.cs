using System;
using System.Collections.Generic;
using System.Text;

namespace AuditClient
{
    class ProgramConfig
    {
        public ProgramConfigIngestion Ingestion { get; set; } = new ProgramConfigIngestion();

        public ProgramConfigClient Client { get; set; } = new ProgramConfigClient();
    }
}
