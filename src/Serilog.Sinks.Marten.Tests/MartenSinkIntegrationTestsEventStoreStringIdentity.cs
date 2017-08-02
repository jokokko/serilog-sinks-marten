using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using Marten;
using Marten.Events;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public sealed class MartenSinkIntegrationTestsEventStoreStringIdentity : MartenSinkIntegrationTestsEventStore<string>
	{
		protected override Action<StoreOptions> Configure => options => options.Events.StreamIdentity =
			global::Marten.Events.StreamIdentity.AsString;

		protected override string StreamIdentity => "string-identity";
		protected override Func<IEventStore, IReadOnlyList<IEvent>> GetEvents => store => store.FetchStream(StreamIdentity);
		protected override Action<IMartenLogWriterBuilder> ConfigureBuilder => c => c.UseEventStore((string) StreamIdentity);		
	}
}