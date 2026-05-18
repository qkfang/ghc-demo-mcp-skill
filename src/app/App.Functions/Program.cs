using App.Functions.Options;
using App.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.Configure<MovieApiOptions>(context.Configuration.GetSection("MovieApi"));
        services.AddSingleton<IMovieBookingService, InMemoryMovieBookingService>();
    })
    .Build();

host.Run();
