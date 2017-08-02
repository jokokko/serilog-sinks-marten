using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Sinks.Marten.Model;
using Serilog.Sinks.Marten.Tests.Harness;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public sealed class MartenSinkIntegrationTestsDocumentStore : IntegrationTest
	{
		[Fact]
		public void RenderedTemplateIsPersisted()
		{
			var (_, template) = ErrorTick();

			using (var s = Store.QuerySession())
			{
				var itemFromDb = Queryable.OrderByDescending<LogEntryDocument, long>(s.Query<LogEntryDocument>(), x => x.Id).Take(1)
					.Single();
				Assert.Equal((string) template, itemFromDb.MessageTemplate);
			}
		}

		[Fact]
		public void ScalarPropertyIsPersisted()
		{
			var (msg, _) = ErrorTick();

			using (var s = Store.QuerySession())
			{
				var itemFromDb = Queryable.OrderByDescending<LogEntryDocument, long>(s.Query<LogEntryDocument>(), x => x.Id).Take(1)
					.Single();
				Assert.Equal(msg, itemFromDb.Properties["ticks"]);
			}
		}

		[Fact]
		public void InnerExceptionsGetPersisted()
		{
			ErrorTick(() => new AggregateException(new InvalidOperationException("First"), new ArgumentException("Second"),
				new ArgumentOutOfRangeException("Third")));

			using (var s = Store.QuerySession())
			{
				var itemFromDb = Queryable.OrderByDescending<LogEntryDocument, long>(s.Query<LogEntryDocument>(), x => x.Id).Take(1)
					.Single();
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

			using (var sut = new LoggerConfiguration().WriteTo.Marten(Store).CreateLogger())
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

			using (var s = Store.QuerySession())
			{
				var itemFromDb = Queryable.OrderByDescending<LogEntryDocument, long>(s.Query<LogEntryDocument>(), x => x.Id).Take(1)
					.Single();

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

			using (var sut = new LoggerConfiguration().WriteTo.Marten(Store).CreateLogger())
			{
				sut.Error(exFactory?.Invoke() ?? new Exception(msg), template, msg);
			}

			return (msg, template);
		}
	}
}