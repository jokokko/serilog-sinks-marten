using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;

namespace Serilog.Sinks.Marten.Services
{
	internal sealed class MartenEventStoreReporterStringIdentity : MartenEventStoreReporter<string>
	{
		public MartenEventStoreReporterStringIdentity(IDocumentStore store, string tenancy, Func<IEnumerable<LogEvent>, IDictionary<string, IEnumerable<LogEntry>>> toEvents) : base(store, tenancy, toEvents)
		{
		}

		public override void Log(IEnumerable<LogEvent> events)
		{
			var eventsToLog = ToEvents(events);
			using (var s = Store.LightweightSession(Tenancy))
			{
				foreach (var byStream in eventsToLog)
				{
					// ReSharper disable once CoVariantArrayConversion
					s.Events.Append(byStream.Key, byStream.Value.ToArray());
				}
				s.SaveChanges();
			}
		}
	}
}