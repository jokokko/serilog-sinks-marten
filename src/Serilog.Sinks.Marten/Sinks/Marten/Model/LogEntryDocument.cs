namespace Serilog.Sinks.Marten.Model
{
	/// <summary>
	/// Augment <see cref="LogEntry"/> with Id field to be used as document primary key.
	/// </summary>
	public sealed class LogEntryDocument : LogEntry
	{
		/// <summary>
		/// Document id (primary key).
		/// </summary>
		public long Id { get; set; }
	}
}