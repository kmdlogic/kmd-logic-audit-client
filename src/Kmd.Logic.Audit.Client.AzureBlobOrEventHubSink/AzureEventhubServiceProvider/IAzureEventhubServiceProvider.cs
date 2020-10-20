using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public interface IAzureEventhubServiceProvider
    {
        void PostMessage(EventHubClient eventHubClient, EventData eventHubData);
    }
}
