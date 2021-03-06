﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RedditBots.Web.Hubs;
using RedditBots.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditBots.Web.Helpers
{
    public class LogHandler
    {
        private const int _totalLogsHistory = 50;
        private readonly IHubContext<LogHub, ILogClient> _hubContext;

        public DateTime? LastLogDateTime { get; set; }

        public List<LogEntry> LastLogs { get; set; } = new List<LogEntry>(_totalLogsHistory);

        public LogHandler(IHubContext<LogHub, ILogClient> hubContext)
        {
            _hubContext = hubContext;
        }

        internal async Task LogAsync(LogEntry entry)
        {
            LastLogDateTime = DateTime.Now;

            await _hubContext.Clients.All.Log(entry);
            await _hubContext.Clients.All.UpdateLastDateTime(LastLogDateTime.Value.ToShortTimeString());

            if (LastLogs.Count == _totalLogsHistory)
            {
                LastLogs.RemoveAt(0);
            }

            if (Enum.Parse<LogLevel>(entry.LogLevel) > LogLevel.Debug)
            {
                entry.Notify = false;
                LastLogs.Add(entry);
            }
        }
    }
}
