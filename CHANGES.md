1.4.0
* Added custom sink to handle large messages (more than 256KB) which will push logs with size exceeding the threshold to a blob storage and send the respective blob url in eventhub sink. The threshold size (in bytes) can be configured. For messages size less than threshold it will behave like an eventhub sink.

1.3.0
* Added CreateCustomized static method to the SerilogSeqAuditClient and SerilogAzureEventHubsAuditClient to allow full control of the logger behaviour

1.2.2
* Ensure events sent via Azure EventHubs client have the message template (#11)

1.1.0
* Added capability to send events via Azure EventHubs

1.0.0
* Major release

0.1.0
* Initial version
