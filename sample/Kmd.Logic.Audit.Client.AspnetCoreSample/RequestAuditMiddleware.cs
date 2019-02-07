// based on https://github.com/datalust/serilog-middleware-example/blob/bb385a01169ce73c7adc220ca323178e6e35730d/src/Datalust.SerilogMiddlewareExample/Diagnostics/SerilogMiddleware.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Kmd.Logic.Audit.Client;
using Microsoft.AspNetCore.Http.Features;

namespace Kmd.Logic.Audit.Client.AspnetCoreSample
{
    internal class RequestAuditMiddleware
    {
        private const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        private const string ErrorMessageTemplate = "Error in HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private static readonly HashSet<string> HeaderWhitelist = new HashSet<string> { "Content-Type", "Content-Length", "User-Agent" };

        private readonly IAudit audit;

        private readonly RequestDelegate next;

        public RequestAuditMiddleware(IAudit audit, RequestDelegate next)
        {
            this.audit = audit ?? throw new ArgumentNullException(nameof(audit));
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var start = Stopwatch.GetTimestamp();
            try
            {
                await this.next(httpContext).ConfigureAwait(false);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                var statusCode = httpContext.Response?.StatusCode;
                var isError = statusCode > 499;
                var template = isError ? ErrorMessageTemplate : MessageTemplate;
                this.audit.Write(template, httpContext.Request.Method, GetPath(httpContext), statusCode, elapsedMs);
            }
            catch (Exception ex) when (AuditException(this.audit, httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex)) { }
        }

        private static bool AuditException(IAudit audit, HttpContext httpContext, double elapsedMs, Exception ex)
        {
            AuditForErrorContext(audit, httpContext)
                .ForContext("Exception", ex.ToString())
                .Write(ErrorMessageTemplate, httpContext.Request.Method, GetPath(httpContext), 500, elapsedMs);

            return false;
        }

        private static IAudit AuditForErrorContext(IAudit audit, HttpContext httpContext)
        {
            var request = httpContext.Request;

            var loggedHeaders = request.Headers
                .Where(h => HeaderWhitelist.Contains(h.Key))
                .ToDictionary(h => h.Key, h => h.Value.ToString());

            var result = audit
                .ForContext("RequestHeaders", loggedHeaders, captureObjectStructure: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            return result;
        }

        private static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }

        private static string GetPath(HttpContext httpContext)
        {
            return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget ?? httpContext.Request.Path.ToString();
        }
    }
}
