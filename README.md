# Serilog.Sinks.Marten [![Build Status](https://ci.appveyor.com/api/projects/status/5kp843wlg6r12187?svg=true)](https://ci.appveyor.com/project/jokokko/serilog-sinks-marten) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Marten.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Marten/)
Send log events to [PostgreSQL](https://www.postgresql.org/) via [Marten](http://jasperfx.github.io/marten/).

**Package** [Serilog.Sinks.Marten](https://www.nuget.org/packages/Serilog.Sinks.Marten) | **Platforms** .NET 4.6, .NET Standard 1.3

### Getting started

Install the [Serilog.Sinks.Marten](https://www.nuget.org/packages/Serilog.Sinks.Marten) package from NuGet:

```powershell
Install-Package Serilog.Sinks.Marten -Pre
```

Enable & configure the sink through one of the LoggerSinkConfiguration.Marten extension methods:

```csharp
// Persist logs through the provided IDocumentStore.
var log = new LoggerConfiguration().WriteTo.Marten(store).CreateLogger();
// Or provide a connection string in place of store.
```

Without further customizations, logs are persited in the Marten Document Store as `LogEntryDocument` documents, indexed by primary key of type `long`. Any customizations to the `IDocumentStore` are respected.

More advanced scenarios are attainable through the `IMartenLogWriterBuilder` configuration callback. The following sample demonstrates persisting events in the Marten Event Store, whereby the document tenancy of "logs" is used for logged events with error level logs being indexed by the stream identify of "error", whereas other levels are indexed by the identify of "other".

```csharp
var log = new LoggerConfiguration()
        .WriteTo.Marten(store, c =>
        {
            c.UseTenancy("logs");
            c.UseEventStore(e => e.Level == LogEventLevel.Error ? "error" : "other");
        })
        .CreateLogger();
```

Note: By default, documents in the Marten Document Store are created through the bulk insertion API. The default Marten Event Store loggers enlist in a Marten `LightweightSession` (i.e. no document tracking) to persist any logged event batches within a single transaction.

Integration tests (against PostgreSQL) only run if `marten_testing_database` environment variable is set (e.g. `host=localhost;database=marten_test;password=marten;username=marten`).