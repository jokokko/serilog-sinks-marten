using System;
using Marten;
using Xunit;

namespace Serilog.Sinks.Marten.Tests
{
	public class MartenSinkTests
	{
		[Fact]
		public void CannotInitializeWithEmptyConnectionString()
		{
			Assert.Throws<ArgumentException>(() => new LoggerConfiguration().WriteTo.Marten(String.Empty).CreateLogger());
		}

		[Fact]
		public void CannotInitializeWithNullStore()
		{
			Assert.Throws<ArgumentNullException>(() => new LoggerConfiguration().WriteTo.Marten((IDocumentStore)null).CreateLogger());
		}
	}
}