using System;
using System.Linq;
using Marten;
using Marten.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public sealed class MartenSinkIntegrationTests : IntegrationTest
	{
		[Fact]
		public void CanInitializeFromConnectionString()
		{
			var msg = DateTime.UtcNow.Ticks.ToString();
			using (var sut = new LoggerConfiguration().WriteTo.Marten(Env).CreateLogger())
			{								
				sut.Information(msg);
			}

			using (var s = DocumentStore.For(Env).QuerySession())
			{
				var itemFromDb = s.Query<LogEntryDocument>().OrderByDescending(x => x.Id).Take(1).First();

				Assert.Contains(msg, itemFromDb.RenderedMessage);
			}
		}

		[Fact]
		public void CanUseMultiTenancy()
		{
			var msg = $"multitenant {DateTime.UtcNow.Ticks}";
			var tenancy = "logs";
			var store = DocumentStore.For(c =>
			{
				c.Connection(Env);
				c.Policies.AllDocumentsAreMultiTenanted();
			});
			using (var sut = new LoggerConfiguration().WriteTo.Marten(store, c =>
			{
				c.UseTenancy(tenancy);
			}).CreateLogger())
			{
				sut.Information(msg);
			}

			using (var s = store.QuerySession())
			{
				var itemFromDb = s.Query<LogEntryDocument>().FirstOrDefault(x => x.RenderedMessage == msg);

				Assert.Null(itemFromDb);
			}

			using (var s = store.QuerySession(tenancy))
			{
				var itemFromDb = s.Query<LogEntryDocument>().FirstOrDefault(x => x.RenderedMessage == msg);

				Assert.NotNull(itemFromDb);
			}
		}		
	}
}