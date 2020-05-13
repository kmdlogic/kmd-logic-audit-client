using System;
using System.Net.Http;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogSeq
{
    public static class SerilogSeqAuditClientConfigurationExtensions
    {
        public static LoggerConfiguration CreateDefaultConfiguration(this SerilogSeqAuditClientConfiguration config, HttpMessageHandler messageHandler = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                .AuditTo.Seq(config.ServerUrl.ToString(), apiKey: config.ApiKey, messageHandler: messageHandler, compact: true);

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
