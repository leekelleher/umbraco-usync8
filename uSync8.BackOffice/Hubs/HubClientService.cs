﻿using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uSync8.BackOffice.Hubs
{
    public class HubClientService
    {
        private readonly IHubContext hubContext;
        private readonly string clientId;

        public HubClientService(string clientId)
        {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<uSyncHub>();
            this.clientId = clientId;
        }

        public void SendMessage<TObject>(TObject item)
        {
            if (hubContext != null && !string.IsNullOrWhiteSpace(clientId))
            {
                var client = hubContext.Clients.Client(clientId);
                if (client != null)
                {
                    client.Add(item);
                    return;
                }

                hubContext.Clients.All.Add(item);
            }
        }

        public void SendUpdate(Object message)
        {
            if (hubContext != null && !string.IsNullOrWhiteSpace(clientId))
            {
                var client = hubContext.Clients.Client(clientId);
                if (client != null)
                {
                    client.Update(message);
                    return;
                }
                hubContext.Clients.All.Update(message);
            }
        }


        public void PostSummary(SyncProgressSummary summary)
        {
            this.SendMessage(summary);
        }

        public void PostUpdate(string message, int count, int total)
        {
            this.SendUpdate(new uSyncUpdateMessage()
            {
                Message = message,
                Count = count,
                Total = total
            });
        }

        public uSyncCallbacks Callbacks() =>
            new uSyncCallbacks(this.PostSummary, this.PostUpdate);
    }

    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class uSyncUpdateMessage
    {
        public string Message { get; set; }
        public int Count { get; set; }
        public int Total { get; set; }
    }
}
