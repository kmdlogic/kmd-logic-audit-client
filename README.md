# KMD Logic Audit Client

A dotnet client library for the [KMD Logic](https://console.kmdlogic.io/) Audit Service, which allows applications to write audit events reliably and securely.

The KMD Logic Audit service utilises many modern concepts from [Serilog](https://serilog.net/) and [Seq](https://datalust.co/seq), such as [Message Templates](https://messagetemplates.org/) and ingestion endpoints capable of understanding [CLEF](https://docs.datalust.co/docs/posting-raw-events). The KMD Logic Audit product also supports using an [Azure EventHubs](https://azure.microsoft.com/en-us/services/event-hubs/) backend.

## How to use this client library

### Reference `Kmd.Logic.Audit.Client`

In projects or components where you have the need to *generate* audit events, add a NuGet package reference to [Kmd.Logic.Audit.Client](https://www.nuget.org/packages/Kmd.Logic.Audit.Client), and use the `IAudit` interface like this:

```csharp
audit
    .ForContext("CorrelationId", correlationId)
    .ForContext("RequestId", httpRequestId)
    .Write("Entity {EntityId} is now {Status} because {UserId} requested it", domainEntity.Id, domainEntity.Status, currentUserId);
```

### Choose your implementation (client backend)

If you are developing locally, we recommend using `Kmd.Logic.Audit.Client.SerilogSeq.SerilogSeqAuditClient` and install [Seq](https://datalust.co/seq) as the backend. To use Logic as the backend, you must choose `Kmd.Logic.Audit.Client.SerilogAzureEventHubs.SerilogAzureEventHubsAuditClient` as the implementation of `IAudit`. In case you want to push large audit events (size typically starts from 256 KB) you can use our large audit event client implementation which is `Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.SerilogLargeAuditEventClient`.

In your application, typically in `Main()` or `Startup.ConfigureServices()` or another [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/), create an instance of your chosen audit client. Use it as the `IAudit` implementation by injecting it into your container with a singleton lifetime or expose it as a `readonly static` property or method.

Since the client implementations are thread-safe and require disposal, it would be appropriate to use a singleton lifetime in a DI container and allow the DI container to dispose of it upon application shut down.

To demonstrate this *without* the use of a DI container:

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

If required you can customise the behaviour of the audit logger, but it is at your own risk:

```csharp
var config = new SerilogSeqAuditClientConfiguration
{
    ServerUrl = new Uri("http://localhost:5341/"),
    ApiKey = null,
    EnrichFromLogContext = true,
};

var logger = config.CreateDefaultConfiguration()
                   .Enrich.WithProperty("MachineName", Environment.MachineName)
                   .CreateLogger();

using (var audit = SerilogSeqAuditClient.CreateCustomized(logger))
{
    // write your audit events here
}
```

> NOTE: We have implemented this functionality initially by  reusing [Serilog](https://github.com/serilog/serilog) and the [Azure EventHub Sink](https://github.com/serilog/serilog-sinks-azureeventhub) or the [Seq sink](https://github.com/serilog/serilog-sinks-seq). We intend to publish a version of this client library in the future that has fewer (or no) external dependencies. If this issue impacts you negatively, please let us know. To help with future migrations away from [Kmd.Logic.Audit.Client.SerilogAzureEventHubs](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogAzureEventHubs) or [Kmd.Logic.Audit.Client.SerilogSeq](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogSeq), try to depend only on the [Kmd.Logic.Audit.Client](https://www.nuget.org/packages/Kmd.Logic.Audit.Client) package in components that write events, and depend on the [Kmd.Logic.Audit.Client.SerilogAzureEventHubs](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogAzureEventHubs) or [Kmd.Logic.Audit.Client.SerilogSeq](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogSeq) package in your application composition root only.

## SerilogLargeAuditEvents implementation for large audit events

The audit client implementation `Kmd.Logic.Audit.Client.SerilogAzureEventHubs.SerilogAzureEventHubsAuditClient` has limitation in terms of size of audit events. To overcome this we have come up with a custom sink which based on the size of audit event will push the event to blob for large messages and send the blob url to event hub. The default size(in bytes) limit is 256 KB which is customizable.

To use the custom sink you need to use the `Kmd.Logic.Audit.Client.SerilogLargeAuditEvents` implementation.

You need to create the client configuration of `Kmd.Logic.Audit.Client.SerilogLargeAuditEvents` where the connection properties of event hub and blob storage can be provided. The blob will be owned by the user who is going to use the audit client. The event size limit can also be customized here.

```csharp
var clientConfig = new Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.SerilogLargeAuditEventClientConfiguration
    {
        EventSource = {EventSource},
        AuditEventTopic = {EventTopic},
        EventHubConnectionString = {EventHubConnectionString},
        EnrichFromLogContext = true,
        StorageAccountName = {StorageAccountName},
        StorageConnectionString = {StorageConnectionString},
        StorageContainerName = {ContainerName},
        EventSizeLimitinBytes = {EventSizeLimitinBytes}
    };
```

After creating the client configuration create the `Kmd.Logic.Audit.Client.SerilogLargeAuditEvents` client.

```csharp
using (var audit = new Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.SerilogLargeAuditEventClient(clientConfig))
{
    // write your audit events here
}
```

You can also use your own Azure storage blob service client while creating the `Kmd.Logic.Audit.Client.SerilogLargeAuditEvents` client.

```csharp
var blobServiceClient = new BlobServiceClient({StorageConnectionString});
using (var client = new SerilogLargeAuditEvents.SerilogLargeAuditEventClient(clientConfig, blobServiceClient))
{
    // write your audit events here
}
```


## Using Seq as the destination of audit events

The KMD Logic Audit service back-end accepts log events in the [Serilog CLEF format](https://github.com/serilog/serilog-formatting-compact#format-details), and is [compatible with the Seq ingestion endpoint](https://docs.getseq.net/docs/posting-raw-events). That means developers writing audit events can [install the free version of Seq](https://getseq.net/Download) and configure the `SerilogSeqAuditClient` to point to their local Seq instance (usually http://localhost:5341/ by default).

We recommend developers use Seq locally to help ensure the audit event development experience is first class, and use the KMD Logic Audit Service back-end in shared deployed application environments.

![Sample Seq Output](./assets/seq-events-view.png)

## Using KMD Logic Audit as the destination of audit events

Contact us at discover@kmdlogic.io for more information.
