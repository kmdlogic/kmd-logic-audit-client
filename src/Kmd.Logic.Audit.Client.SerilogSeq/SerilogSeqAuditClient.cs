﻿using System;
using System.Net.Http;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogSeq
{
    public class SerilogSeqAuditClient : IAudit, IDisposable
    {
        private readonly Logger logger;
        private readonly bool disposeLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogSeqAuditClient(SerilogSeqAuditClientConfiguration config, HttpMessageHandler messageHandler = null)
        {
            this.logger = config.CreateDefaultConfiguration(messageHandler).CreateLogger();
            this.disposeLogger = true;
            this.audit = new SerilogLoggerAudit(this.logger);
        }

        private SerilogSeqAuditClient(Logger logger, bool disposeLogger)
        {
            this.logger = logger;
            this.disposeLogger = disposeLogger;
            this.audit = new SerilogLoggerAudit(this.logger);
        }

        public static SerilogSeqAuditClient CreateCustomized(Logger logger, bool disposeLogger = true)
        {
            return new SerilogSeqAuditClient(logger, disposeLogger);
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.audit.Write(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return this.audit.ForContext(propertyName, value, captureObjectStructure);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.disposeLogger)
            {
                this.logger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
