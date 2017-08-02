using System;
using System.Linq;
using Baseline;
using Marten;
using Marten.Storage;
using Serilog.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public class MartenSinkIntegrationTestsEventStoreIndexing : IntegrationTest
	{
		[Fact]
		public void CanIndexToMultipleStreamsWithStringIdentity()
		{
			var msg = $"multitenant {DateTime.UtcNow.Ticks}";
			var tenancy = "logs";

			var store = DocumentStore.For(c =>
			{
				c.Events.StreamIdentity = global::Marten.Events.StreamIdentity.AsString;
				c.Policies.AllDocumentsAreMultiTenanted();
				c.Connection(Env);
			});

			store.Advanced.Clean.CompletelyRemoveAll();

			using (var sut = new LoggerConfiguration().WriteTo.Marten(store, c =>
			{
				c.UseTenancy(tenancy);
				c.UseEventStore(e => e.Level == LogEventLevel.Error ? "error" : "other");
			}).CreateLogger())
			{
				sut.Information(msg);
				sut.Error(msg);
			}			

			using (var s = store.OpenSession(tenancy))
			{
				var itemFromDb = s.Events.FetchStream("error").Last().Data.As<LogEntry>();

				Assert.Contains(msg, itemFromDb.RenderedMessage);
				Assert.Equal(LogEventLevel.Error, itemFromDb.Level);

				itemFromDb = s.Events.FetchStream("other").Last().Data.As<LogEntry>();

				Assert.Contains(msg, itemFromDb.RenderedMessage);
				Assert.Equal(LogEventLevel.Information, itemFromDb.Level);
			}
		}

		[Fact]
		public void CanIndexToMultipleStreamsWithGuidIdentity()
		{
			var msg = $"multitenant {DateTime.UtcNow.Ticks}";
			var tenancy = "logs";

			var store = DocumentStore.For(c =>
			{
				c.Events.StreamIdentity = global::Marten.Events.StreamIdentity.AsGuid;
				c.Policies.AllDocumentsAreMultiTenanted();
				c.Connection(Env);
			});

			store.Advanced.Clean.CompletelyRemoveAll();

			var error = Guid.NewGuid();
			var other = Guid.NewGuid();

			using (var sut = new LoggerConfiguration().WriteTo.Marten(store, c =>
			{
				c.UseTenancy(tenancy);
				c.UseEventStore(e => e.Level == LogEventLevel.Error ? error : other);
			}).CreateLogger())
			{
				sut.Information(msg);
				sut.Error(msg);
			}

			using (var s = store.OpenSession(tenancy))
			{
				var itemFromDb = s.Events.FetchStream(error).Last().Data.As<LogEntry>();

				Assert.Contains(msg, itemFromDb.RenderedMessage);
				Assert.Equal(LogEventLevel.Error, itemFromDb.Level);

				itemFromDb = s.Events.FetchStream(other).Last().Data.As<LogEntry>();

				Assert.Contains(msg, itemFromDb.RenderedMessage);
				Assert.Equal(LogEventLevel.Information, itemFromDb.Level);
			}
		}
	}
}