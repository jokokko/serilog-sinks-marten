using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;

namespace Serilog.Sinks.Marten.Services
{
	internal sealed class MartenDocumentReporter : IMartenLogWriter, IDisposable
	{
		private readonly Func<IEnumerable<LogEvent>, IEnumerable<LogEntryDocument>> toDocuments;
		private readonly string tenancy;		
		private readonly IDocumentStore store;

		public MartenDocumentReporter(IDocumentStore store, string tenancy, Func<IEnumerable<LogEvent>, IEnumerable<LogEntryDocument>> toDocuments)
		{			
			this.store = store ?? throw new ArgumentNullException(nameof(store));
			this.tenancy = tenancy ?? throw new ArgumentNullException(nameof(tenancy));
			this.toDocuments = toDocuments ?? throw new ArgumentNullException(nameof(toDocuments));
		}

		public void Log(IEnumerable<LogEvent> events)
		{
			if (events == null)
			{
				throw new ArgumentNullException(nameof(events));
			}

			var eventsToLog = toDocuments(events);

			store.BulkInsert(tenancy, eventsToLog.ToArray());
		}

		public void Dispose()
		{
			store?.Dispose();
		}
	}
}