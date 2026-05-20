using Microsoft.Azure.Functions.Worker.Builder;
using MovieApi.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton<IMovieRepository, InMemoryMovieRepository>();

builder.Build().Run();
