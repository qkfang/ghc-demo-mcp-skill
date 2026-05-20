using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using MovieApi.Repositories;

namespace MovieApi.Functions;

public sealed class GetMoviesFunction(IMovieRepository movieRepository)
{
    [Function("GetMovies")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "movies")]
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        var movies = await movieRepository.GetAvailableMoviesAsync(cancellationToken);
        return new OkObjectResult(movies);
    }
}
