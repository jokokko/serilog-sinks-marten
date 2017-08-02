using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;

namespace Serilog.Sinks.Marten.Services
{
	internal static class MartenDocumentTransform
	{
		public static LogEntryDocument[] From(IEnumerable<LogEvent> events, IFormatProvider provider)
		{
			var eventsToLog = events.Select(x => new LogEntryDocument
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