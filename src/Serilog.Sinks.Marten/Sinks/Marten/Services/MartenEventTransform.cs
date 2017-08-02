using System;
using Serilog.Sinks.Marten.Model;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace Serilog.Sinks.Marten.Services
{
	internal static class MartenEventTransform
	{
		public static IEnumerable<LogEntry> From(IEnumerable<LogEvent> events, IFormatProvider provider)
		{
			var eventsToLog = events.Select(x => new LogEntry
			{
				Exception = x.Exception,
				Level = x.Level,
				RenderedMessage = x.RenderMessage(provider),
				Timestamp = x.Timestamp,
				MessageTemplate = x.MessageTemplate.Text,
				Properties = x.Properties?.ToDictionary(k => k.Key, v => MartenPropertyFormatter.Simplify(v.Value))
			}).ToArray();

			return eventsToLog;
		}
	}
}