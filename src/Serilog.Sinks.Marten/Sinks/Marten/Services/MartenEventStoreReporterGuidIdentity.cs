using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;

namespace Serilog.Sinks.Marten.Services
{
	internal sealed class MartenEventStoreReporterGuidIdentity : MartenEventStoreReporter<Guid>
	{
		public MartenEventStoreReporterGuidIdentity(IDocumentStore store, string tenancy, Func<IEnumerable<LogEvent>, IDictionary<Guid, IEnumerable<LogEntry>>> toEvents) : base(store, tenancy, toEvents)
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