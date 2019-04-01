namespace Kmd.Logic.Audit.Client.Sample
{
    public class ProgramConfig
    {
        public ProgramConfigIngestion Ingestion { get; set; } = new ProgramConfigIngestion();

        public ProgramConfigClient Client { get; set; } = new ProgramConfigClient();
    }
}
