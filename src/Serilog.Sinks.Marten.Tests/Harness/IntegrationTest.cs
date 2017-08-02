using System;
using Marten;
using Xunit;

namespace Serilog.Sinks.Marten.Tests.Harness
{
    [Trait("Category", "Integration"), Collection("StartClean")]
    public abstract class IntegrationTest
    {
	    protected virtual Action<StoreOptions> Configure { get; }

        protected IntegrationTest()
        {            
            Env = Environment.GetEnvironmentVariable("marten_testing_database");

            if (string.IsNullOrEmpty(Env))
            {
                throw new InvalidOperationException("marten_testing_database environment variable not set (e.g. marten_testing_database=host=localhost;database=marten_test;password=marten;username=marten).");
            }

            store = new Lazy<IDocumentStore>(() =>
            {
				var testStore = DocumentStore.For(c =>
	            {
		            c.Connection(Env);
		            Configure?.Invoke(c);
	            });

	            testStore.Advanced.Clean.CompletelyRemoveAll();

	            return testStore;
            });
        }

	    private readonly Lazy<IDocumentStore> store;
	    public readonly string Env;
	    public IDocumentStore Store => store.Value;
    }
}