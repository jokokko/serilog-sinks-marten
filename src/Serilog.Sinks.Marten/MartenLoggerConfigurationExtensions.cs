using System;
using Marten;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Marten;
using Serilog.Sinks.Marten.Model;

namespace Serilog
{
    /// <summary>
    /// Plug Marten sink to Serilog.
    /// </summary>
    public static class MartenLoggerConfigurationExtensions
    {
        /// <summary>
        /// The default batch size for sending off events to Marten.
        /// </summary>
        public const int DefaultBatchPostingLimit = 50;
        /// <summary>
        /// The default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);

		/// <summary>
		/// Adds a sink that sends log events to PostgreSQL via Marten.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="connectionString">The connection string used to initialize <see cref="IDocumentStore"/>.</param>
		/// <param name="configure">Configure logging parameters, such as document tenancy or using Marten Event Store as log storage. By default events are logged to Marten Document Store as <see cref="LogEntryDocument"/>.</param>
		/// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration Marten(this LoggerSinkConfiguration sinkConfiguration, string connectionString, Action<IMartenLogWriterBuilder> configure = null, int batchSizeLimit = DefaultBatchPostingLimit, TimeSpan? period = null, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
	    {
		    if (sinkConfiguration == null)
		    {
			    throw new ArgumentNullException(nameof(sinkConfiguration));
		    }
		    if (string.IsNullOrEmpty(connectionString))
		    {
			    throw new ArgumentException(nameof(connectionString));
		    }

		    var store = DocumentStore.For(connectionString);

		    period = period ?? DefaultPeriod;

		    var sink = new MartenSink(store, configure, batchSizeLimit, period.Value);

		    return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
	    }

		/// <summary>
		/// Adds a sink that sends log events to PostgreSQL via Marten.
		/// </summary>
		/// <param name="sinkConfiguration">The logger configuration.</param>
		/// <param name="store">Document store to be used in log persistence.</param>
		/// <param name="configure">Configure logging parameters, such as document tenancy or using Marten Event Store as log storage. By default events are logged to Marten Document Store as <see cref="LogEntryDocument"/>.</param>
		/// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
		/// <param name="period">The time to wait between checking for event batches.</param>
		/// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
		/// <returns>Logger configuration, allowing configuration to continue.</returns>
		public static LoggerConfiguration Marten(this LoggerSinkConfiguration sinkConfiguration, IDocumentStore store, Action<IMartenLogWriterBuilder> configure = null, int batchSizeLimit = DefaultBatchPostingLimit, TimeSpan? period = null, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

	        if (store == null)
	        {
		        throw new ArgumentNullException(nameof(store));
	        }

	        period = period ?? DefaultPeriod;

            var sink = new MartenSink(store, configure, batchSizeLimit, period.Value);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}