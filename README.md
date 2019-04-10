# KMD Logic Audit Client

A dotnet client library for the [KMD Logic](https://console.kmdlogic.io/) Audit Service, which allows applications to write audit events reliably and securely.

The KMD Logic Audit service utilises many modern concepts from [Serilog](https://serilog.net/) and [Seq](https://getseq.net/), such as [Message Templates](https://messagetemplates.org/) and ingestion endpoints capable of understanding [CLEF](https://docs.getseq.net/docs/posting-raw-events).

## How to use this client library

### Reference `Kmd.Logic.Audit.Client`

In projects or components where you have the need to *generate* audit events, add a NuGet package reference to [Kmd.Logic.Audit.Client](https://www.nuget.org/packages/Kmd.Logic.Audit.Client), and use the `IAudit` interface like this:

```csharp
audit
    .ForContext("CorrelationId", correlationId)
    .ForContext("RequestId", httpRequestId)
    .Write("Entity {EntityId} is now {Status} because {UserId} requested it", domainEntity.Id, domainEntity.Status, currentUserId);
```

### Choose your client backend

In your application, typically in `Main()` (or perhaps `Startup.ConfigureServices()` or another [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/)), you create an instance of the audit client (`Kmd.Logic.Audit.Client.SerilogSeq.SerilogSeqAuditClient` for Seq or `Kmd.Logic.Audit.Client.SerilogAzureEventHubs.SerilogAzureEventHubsAuditClient` for Azure EventHubs) and use it as the implementation of `IAudit` by injecting it into your container with a singleton lifetime or expose it as a static property or method. Since the client implementations are thread-safe and require disposal, it would be appropriate to use a singleton lifetime in a DI container and allow the DI container to dispose of it upon application shut down.

To demonstrate this without the use of a DI container:

```csharp
public static class Audit
{
  public static readonly IAudit Instance;
}
using (var audit = new SerilogSeqAuditClient(
    new SerilogSeqAuditClientConfiguration
    {
        ServerUrl = new Uri("http://localhost:5341/"),
        ApiKey = null,
        EnrichFromLogContext = true,
    }))
{
    // write your audit events here
}
```

> NOTE: We have implemented this functionality initially by  reusing [Serilog](https://github.com/serilog/serilog) and the [Seq sink](https://github.com/serilog/serilog-sinks-seq). We intend to publish a version of this client library in the future that has no external dependencies. If this issue impacts you negatively, please let us know. To help with future migrations away from [Kmd.Logic.Audit.Client.SerilogSeq](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogSeq), try to depend only on the [Kmd.Logic.Audit.Client](https://www.nuget.org/packages/Kmd.Logic.Audit.Client) package in components that write events, and depend on the [Kmd.Logic.Audit.Client.SerilogSeq](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogSeq) package in your application composition root only.

## Using Seq as the destination of audit events

The KMD Logic Audit service back-end accepts log events in the [Serilog CLEF format](https://github.com/serilog/serilog-formatting-compact#format-details), and is [compatible with the Seq ingestion endpoint](https://docs.getseq.net/docs/posting-raw-events). That means developers writing audit events can [install the free version of Seq](https://getseq.net/Download) and configure the `SerilogSeqAuditClient` to point to their local Seq instance (usually http://localhost:5341/ by default).

We recommend developers use Seq locally to help ensure the audit event development experience is first class, and use the KMD Logic Audit Service back-end in shared deployed application environments.

![Sample Seq Output](./assets/seq-events-view.png)

## Using KMD Logic Audit as the destination of audit events

Contact us at discover@kmdlogic.io for more information.
