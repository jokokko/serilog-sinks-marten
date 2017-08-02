using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using Marten.Storage;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Services;

namespace Serilog.Sinks.Marten
{
    internal sealed class MartenLogWriterBuilder : IMartenLogWriterBuilder
    {
	    private readonly IDocumentStore store;
	    private IFormatProvider formatProviderToUse;
		private Func<IMartenLogWriter> reporterFactory;
	    private string tenancyToUse = Tenancy.DefaultTenantId;

		public MartenLogWriterBuilder(IDocumentStore store)
		{
			this.store = store ?? throw new ArgumentNullException(nameof(store));
		}

	    public IMartenLogWriterBuilder UseFormatProvider(IFormatProvider formatProvider)
        {
	        formatProviderToUse = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));

            return this;
        }

	    public IMartenLogWriterBuilder UseEventStore(Func<LogEvent, Guid> indexBy)
	    {
		    if (indexBy == null)
		    {
			    throw new ArgumentNullException(nameof(indexBy));
		    }

		    reporterFactory = () => new MartenEventStoreReporterGuidIdentity(store, tenancyToUse,
			    events => events.ToLookup(indexBy, x => x).ToDictionary(x => x.Key, x => MartenEventTransform.From(x, formatProviderToUse)));

			return this;
	    }

		public IMartenLogWriterBuilder UseEventStore(Func<LogEvent, string> indexBy)
		{
		    if (indexBy == null)
		    {
			    throw new ArgumentNullException(nameof(indexBy));
		    }

			reporterFactory = () => new MartenEventStoreReporterStringIdentity(store, tenancyToUse,
				events => events.ToLookup(indexBy, x => x).ToDictionary(x => x.Key, x => MartenEventTransform.From(x, formatProviderToUse)));

			return this;
	    }

		public IMartenLogWriterBuilder UseEventStore(Guid streamIdentity)
	    {	
		    reporterFactory = () => new MartenEventStoreReporterGuidIdentity(store, tenancyToUse, events => new Dictionary<Guid, IEnumerable<LogEntry>> { { streamIdentity, MartenEventTransform.From(events, formatProviderToUse) } });

		    return this;
	    }

	    public IMartenLogWriterBuilder UseTenancy(string tenancy)
	    {
		    tenancyToUse = tenancy ?? throw new ArgumentNullException(nameof(tenancy));

		    return this;
	    }

		public IMartenLogWriterBuilder UseEventStore(string streamIdentity)
	    {
		    if (string.IsNullOrEmpty(streamIdentity))
		    {
			    throw new ArgumentException(nameof(streamIdentity));
		    }

		    reporterFactory = () => new MartenEventStoreReporterStringIdentity(store, tenancyToUse, events => new Dictionary<string, IEnumerable<LogEntry>> { { streamIdentity, MartenEventTransform.From(events, formatProviderToUse) } });

			return this;
	    }

	    public IMartenLogWriterBuilder UseLogWriter(IMartenLogWriter writer)
	    {
		    if (writer == null)
		    {
			    throw new ArgumentNullException(nameof(writer));
		    }

		    reporterFactory = () => writer;

		    return this;
	    }

		public IMartenLogWriter Build()
        {			
	        if (reporterFactory == null)
	        {
		        reporterFactory = () => new MartenDocumentReporter(store, tenancyToUse, events => MartenDocumentTransform.From(events, formatProviderToUse));
	        }

	        return reporterFactory();
        }
    }
}