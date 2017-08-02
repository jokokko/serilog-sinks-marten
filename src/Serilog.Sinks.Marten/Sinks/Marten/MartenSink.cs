using System;
using System.Collections.Generic;
using Marten;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Services;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Marten
{
	/// <summary>
	/// Send off logged events to PostgreSQL via Marten.
	/// </summary>
	public sealed class MartenSink : PeriodicBatchingSink
    {	    		
	    private readonly IMartenLogWriter writer;

		/// <summary>
		/// Sink that passes messages to PostgreSQL via Marten.
		/// </summary>
		/// <param name="store">Document store to be used in log persistence.</param>
		/// <param name="configure">Configure logging parameters, such as document tenancy or using Marten Event Store as log storage. By default, events are logged to Marten Document Store as <see cref="LogEntryDocument"/>.</param>
		/// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		public MartenSink(IDocumentStore store, Action<IMartenLogWriterBuilder> configure,
            int batchSizeLimit, TimeSpan period) : base(batchSizeLimit, period)
	    {
		    if (store == null)
		    {
			    throw new ArgumentNullException(nameof(store));
		    }
			
            var configuration = new MartenLogWriterBuilder(store);
            configure?.Invoke(configuration);
            writer = configuration.Build();            
	    }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="events">Events to log.</param>        
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            try
            {
                writer.Log(events);
            }
            catch (Exception e)
            {
                SelfLog.WriteLine($"Persisting messages via {nameof(IMartenLogWriter)}", e);
	            throw;
            }
        }

		/// <summary>
		/// Dispose any IDisposable IMartenLogWriter implementations.
		/// </summary>
		protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            var disposable = writer as IDisposable;
            disposable?.Dispose();
        }
    }
}