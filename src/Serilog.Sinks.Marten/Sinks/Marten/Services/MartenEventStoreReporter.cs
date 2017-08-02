using System;
using System.Collections.Generic;
using Marten;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;

namespace Serilog.Sinks.Marten.Services
{
	internal abstract class MartenEventStoreReporter<T> : IMartenLogWriter, IDisposable
	{
		protected readonly string Tenancy;
		protected readonly Func<IEnumerable<LogEvent>, IDictionary<T, IEnumerable<LogEntry>>> ToEvents;
		protected readonly IDocumentStore Store;

		protected MartenEventStoreReporter(IDocumentStore store, string tenancy, Func<IEnumerable<LogEvent>, IDictionary<T, IEnumerable<LogEntry>>> toEvents)
		{			
			Store = store ?? throw new ArgumentNullException(nameof(store));
			Tenancy = tenancy ?? throw new ArgumentNullException(nameof(tenancy));
			ToEvents = toEvents ?? throw new ArgumentNullException(nameof(toEvents));
		}

		public abstract void Log(IEnumerable<LogEvent> events);

		public void Dispose()
		{
			Store?.Dispose();
		}
	}
}