using Serilog.Events;
using System.Collections.Generic;

namespace Serilog.Sinks.Marten.Services
{
	/// <summary>
	/// Write logged events to PostgreSQL via Marten.
	/// </summary>
	public interface IMartenLogWriter
	{
		/// <summary>
		/// Write logged events to Marten Document Store or Marten Event Store.
		/// </summary>
		/// <param name="events">Events to write to database.</param>
		void Log(IEnumerable<LogEvent> events);
	}
}