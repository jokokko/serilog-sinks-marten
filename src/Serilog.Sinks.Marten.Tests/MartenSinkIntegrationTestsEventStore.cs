using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using Marten.Events;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public sealed class MartenSinkIntegrationTestsEventStoreGuidIdentity : MartenSinkIntegrationTestsEventStore<Guid>
	{
		protected override Guid StreamIdentity => Guid.Parse("9D1F0F42-F217-441D-BB6C-381171E22C6A");
		protected override Func<IEventStore, IReadOnlyList<IEvent>> GetEvents => store => store.FetchStream(StreamIdentity);
		protected override Action<IMartenLogWriterBuilder> ConfigureBuilder => c => c.UseEventStore(StreamIdentity);
	}

	public abstract class MartenSinkIntegrationTestsEventStore<T> : IntegrationTest
	{
		protected abstract T StreamIdentity { get; }
		protected abstract Func<IEventStore, IReadOnlyList<IEvent>> GetEvents { get; }
		protected abstract Action<IMartenLogWriterBuilder> ConfigureBuilder { get; }

		[Fact]
		public void RenderedTemplateIsPersisted()
		{
			var (_, template) = ErrorTick();

			using (var s = Store.LightweightSession())
			{
				var itemFromDb = GetEvents(s.Events).Last().Data.As<LogEntry>();
				Assert.Equal((string) template, itemFromDb.MessageTemplate);
			}
		}

		[Fact]
		public void ScalarPropertyIsPersisted()
		{
			var (msg, _) = ErrorTick();

			using (var s = Store.LightweightSession())
			{
				var itemFromDb = GetEvents(s.Events).Last().Data.As<LogEntry>();
				Assert.Equal(msg, itemFromDb.Properties["ticks"]);
			}
		}

		[Fact]
		public void InnerExceptionsGetPersisted()
		{
			ErrorTick(() => new AggregateException(new InvalidOperationException("First"), new ArgumentException("Second"),
				new ArgumentOutOfRangeException("Third")));

			using (var s = Store.LightweightSession())
			{
				var itemFromDb = GetEvents(s.Events).Last().Data.As<LogEntry>();
				// https://github.com/dotnet/corefx/issues/20776
#if NET46
				Assert.Equal(3, ((AggregateException)itemFromDb.Exception).InnerExceptions.Count);
#else
				Assert.IsType(typeof(AggregateException), itemFromDb.Exception);
#endif
			}
		}

		[Fact]
		public void AllPropertiesGetPersisted()
		{
			var template = $"Dictionary {{dict}}, Nested dictionary {{nesteddict}}, List {{list}}, Array {{array}}";

			using (var sut = new LoggerConfiguration().WriteTo.Marten(Store, ConfigureBuilder).CreateLogger())
			{
				sut.Error(new Exception(nameof(AllPropertiesGetPersisted)), template,
					new Dictionary<string, decimal>
					{
						{"first", 1m},
						{"second", 2m},
						{"third", 3m},
					},
					new Dictionary<int, Dictionary<string, string>>
					{
						{
							0, new Dictionary<string, string>
							{
								{"first", "entry"}
							}
						},
						{
							1, new Dictionary<string, string>
							{
								{"second", "entry"}
							}
						}
					},
					new List<object> {"first", "second"},
					new[] {1.1, 1.2, 1.3}
				);
			}

			using (var s = Store.LightweightSession())
			{
				var itemFromDb = GetEvents(s.Events).Last().Data.As<LogEntry>();

				Assert.NotNull(itemFromDb.Properties["dict"]);
				Assert.NotNull(itemFromDb.Properties["nesteddict"]);
				Assert.NotNull(itemFromDb.Properties["list"]);
				Assert.NotNull(itemFromDb.Properties["array"]);
			}
		}

		private (string message, string template) ErrorTick(Func<Exception> exFactory = null)
		{
			var msg = $"ticks {DateTime.UtcNow.Ticks}";
			var template = $"From {nameof(ErrorTick)} at {{ticks}}";

			using (var sut = new LoggerConfiguration().WriteTo.Marten(Store, ConfigureBuilder).CreateLogger())
			{
				sut.Error(exFactory?.Invoke() ?? new Exception(msg), template, msg);
			}

			return (msg, template);
		}
	}
}