using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureEventhubServiceProvider : IAzureEventhubServiceProvider
    {
        public void PostMessage(EventHubClient eventHubClient, EventData eventHubData)
        {
            eventHubClient.SendAsync(eventHubData, Guid.NewGuid().ToString()).GetAwaiter().GetResult();
        }
    }
}
