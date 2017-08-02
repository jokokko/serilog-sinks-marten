using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Sinks.Marten.Model
{
	/// <summary>
	/// Type used in persisting logged events via Marten.
	/// </summary>
	public class LogEntry
	{   
	    /// <summary>
	    /// The time at which the event occurred.
	    /// </summary>
        public DateTimeOffset Timestamp { get; set; }

	    /// <summary>
	    /// The template that was used for the log message.
	    /// </summary>
	    public string MessageTemplate { get; set; }

	    /// <summary>
	    /// The level of the log.
	    /// </summary>
	    public LogEventLevel Level { get; set; }

	    /// <summary>
	    /// A string representation of the exception that was attached to the log (if any).
	    /// </summary>
	    public Exception Exception { get; set; }

	    /// <summary>
	    /// The rendered log message.
	    /// </summary>
	    public string RenderedMessage { get; set; }

	    /// <summary>
	    /// Properties associated with the event, including those presented in <see cref="LogEvent.MessageTemplate"/>.
	    /// </summary>
	    public IDictionary<string, object> Properties { get; set; }
    }
}