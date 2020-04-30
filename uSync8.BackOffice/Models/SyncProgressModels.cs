﻿using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using uSync8.BackOffice.SyncHandlers;

namespace uSync8.BackOffice
{
    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class SyncProgressSummary
    {
        public int Count { get; set; }
        public int Total { get; set; }
        public string Message { get; set; }
        public List<SyncHandlerSummary> Handlers { get; set; }

        public int CurrentStep { get; internal set; }

        public SyncProgressSummary(
            IEnumerable<ISyncHandler> handlers,
            string message,
            int totalSteps)
        {
            this.Total = totalSteps;
            this.Message = message;

            if (handlers != null)
            {
                this.Handlers = handlers.Select(x => new SyncHandlerSummary()
                {
                    Icon = x.Icon,
                    Name = x.Name,
                    Status = HandlerStatus.Pending
                }).ToList();
            }
            else
            {
                this.Handlers = new List<SyncHandlerSummary>();
            }
        }

        public SyncProgressSummary(IEnumerable<SyncHandlerSummary> summaries,
            string message, int totalSteps)
        {
            this.Total = totalSteps;
            this.Message = message;
            this.Handlers = summaries.ToList();
        }

        /// <summary>
        ///  Updated the change status of a single handler in the list
        /// </summary>
        /// <param name="name">Name of handler</param>
        /// <param name="status">current handler status</param>
        /// <param name="changeCount">number of changes</param>
        public void UpdateHandler(string name, HandlerStatus status, int changeCount)
        {
            UpdateHandler(name, status, changeCount, false);
        }

        /// <summary>
        ///  Updated the change status of a single handler in the list
        /// </summary>
        /// <param name="name">Name of handler</param>
        /// <param name="status">current handler status</param>
        /// <param name="changeCount">number of changes</param>
        /// <param name="hasErrors">there are actions that have failed in the set</param>
        public void UpdateHandler(string name, HandlerStatus status, int changeCount, bool hasErrors)
        {
            var item = this.Handlers.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                item.Status = status;
                item.Changes = changeCount;
                item.InError = hasErrors;
            }


            // 8.7 + return the current step (helps us with smoother progress bars).
            if (status == HandlerStatus.Processing)
            {
                var pos = this.Handlers.FindIndex(x => x.Name == name);
                this.CurrentStep = pos + 1;
            }

        }

        /// <summary>
        ///  Updated the change status of a single handler in the list
        /// </summary>
        /// <param name="name">Name of handler</param>
        /// <param name="status">current handler status</param>
        /// <param name="message">Update the main progress message for the UI</param>
        /// <param name="changeCount">number of changes</param>
        public void UpdateHandler(string name, HandlerStatus status, string message, int changeCount)
        {
            UpdateHandler(name, status, changeCount);
            this.Message = message;
        }

    }

    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class SyncHandlerSummary
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public HandlerStatus Status { get; set; }

        public int Changes { get; set; }

        /// <summary>
        ///  reports if the handler has errors
        /// </summary>
        public bool InError { get; set; }
    }

    public enum HandlerStatus
    {
        Pending,
        Processing,
        Complete,
        Error
    }
}
