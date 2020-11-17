namespace Kmd.Logic.Audit.Client.SampleLargeAuditEvents
{
    public class ProgramConfig
    {
        public ProgramConfigIngestion Ingestion { get; set; } = new ProgramConfigIngestion();

        public ProgramConfigClient Client { get; set; } = new ProgramConfigClient();
    }
}
