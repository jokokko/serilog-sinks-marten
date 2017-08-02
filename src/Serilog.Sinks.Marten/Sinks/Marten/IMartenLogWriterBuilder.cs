using System;
using Marten.Storage;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Services;

namespace Serilog.Sinks.Marten
{
	/// <summary>
	/// Configure Serilog log storage in Marten.
	/// </summary>
	public interface IMartenLogWriterBuilder
    {
		/// <summary>
		/// Configure the format provider to be used when rendering log messages.
		/// </summary>
		/// <param name="formatProvider">Format provider used in rendering log messages.</param>
	    IMartenLogWriterBuilder UseFormatProvider(IFormatProvider formatProvider);

		/// <summary>
		/// Store logged events in Marten Event Store with stream identity configured to strings.
		/// The logged entries are stored in event streams as <see cref="LogEntry"/>.
		/// </summary>
		/// <param name="indexBy">Specify stream identity per logged event.</param>
		IMartenLogWriterBuilder UseEventStore(Func<LogEvent, string> indexBy);

		/// <summary>
		/// Store logged events in Marten Event Store with stream identity configured to Guids.
		/// The logged entries are stored in event streams as <see cref="LogEntry"/>.
		/// </summary>
		/// <param name="indexBy">Specify stream identity per logged event.</param>
		IMartenLogWriterBuilder UseEventStore(Func<LogEvent, Guid> indexBy);

	    /// <summary>
	    /// Store logged events in Marten Event Store with stream identity configured to Guids.
	    /// The logged entries are stored in event streams as <see cref="LogEntry"/>.
	    /// </summary>
		/// <param name="streamIdentity">The stream in which logged entries are stored.</param>
		IMartenLogWriterBuilder UseEventStore(Guid streamIdentity);

		/// <summary>
		/// Store logged events in Marten Event Store with stream identity configured to strings.
		/// The logged entries are stored in event streams as <see cref="LogEntry"/>.
		/// </summary>
		/// <param name="streamIdentity">The stream in which logged entries are stored.</param>
		IMartenLogWriterBuilder UseEventStore(string streamIdentity);

		/// <summary>
		/// Document tenancy to be used. Unless specified, <see cref="Tenancy.DefaultTenantId"/> is used.
		/// </summary>
		/// <param name="tenancy">Tenant id to be used.</param>
		IMartenLogWriterBuilder UseTenancy(string tenancy);

		/// <summary>
		/// Specify a custom <see cref="IMartenLogWriter"/> to log events in Marten.
		/// </summary>
		/// <param name="writer">Specify custom writer to persist events.</param>
		IMartenLogWriterBuilder UseLogWriter(IMartenLogWriter writer);
    }
}