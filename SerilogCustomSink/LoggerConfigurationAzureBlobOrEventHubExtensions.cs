using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Configuration;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public static class LoggerConfigurationAzureBlobOrEventHubExtensions
    {
        public static LoggerConfiguration AzureBlobOrEventHub(
              this LoggerSinkConfiguration loggerConfiguration,
              IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new AzureBlobOrEventHubSink(formatProvider));
        }
    }
}
