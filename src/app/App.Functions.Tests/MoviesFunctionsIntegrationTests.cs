using App.Functions.Functions;
using App.Functions.Models;
using App.Functions.Options;
using App.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text.Json;

namespace App.Functions.Tests;

public class MoviesFunctionsIntegrationTests
{
    [Fact]
    public async Task GetMovies_ReturnsOnlyAvailableMovies()
    {
        var sut = CreateSubject();
        var request = TestHttpRequestData.Create("http://localhost/api/movies");

        var response = await sut.GetMovies(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.ReadFromJsonAsync<List<Movie>>();
        Assert.NotNull(payload);
        Assert.All(payload!, movie => Assert.True(movie.AvailableTickets > 0));
    }

    [Fact]
    public async Task BookTickets_ReturnsBookedOrder()
    {
        var sut = CreateSubject();
        var request = TestHttpRequestData.Create("http://localhost/api/movies/2?no_tickets=6");

        var response = await sut.BookTickets(request, "2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.ReadFromJsonAsync<List<OrderDetail>>();
        Assert.NotNull(payload);
        Assert.Single(payload!);
        Assert.Equal(540, payload![0].Price);
    }

    [Fact]
    public async Task BookTickets_InvalidQuery_ReturnsBadRequest()
    {
        var sut = CreateSubject();
        var request = TestHttpRequestData.Create("http://localhost/api/movies/1?no_tickets=0");

        var response = await sut.BookTickets(request, "1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.ReadFromJsonAsync<ApiError>();
        Assert.NotNull(payload);
        Assert.Contains("no_tickets", payload!.Error);
    }

    private static MoviesFunctions CreateSubject()
    {
        var options = Microsoft.Extensions.Options.Options.Create(new MovieApiOptions
        {
            MoviesJson =
                "[{\"m_id\":1,\"m_name\":\"Interstellar\",\"m_available\":20},{\"m_id\":2,\"m_name\":\"Inception\",\"m_available\":10}]"
        });

        IMovieBookingService service = new InMemoryMovieBookingService(options);
        return new MoviesFunctions(service, NullLogger<MoviesFunctions>.Instance);
    }
}

file static class HttpResponseDataExtensions
{
    public static async Task<T?> ReadFromJsonAsync<T>(this HttpResponseData response)
    {
        response.Body.Position = 0;
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        return await JsonSerializer.DeserializeAsync<T>(response.Body, jsonOptions);
    }
}

file sealed class TestFunctionContext : FunctionContext
{
    private readonly IServiceProvider _serviceProvider = new ServiceCollection().BuildServiceProvider();

    public override string InvocationId { get; } = Guid.NewGuid().ToString();
    public override string FunctionId { get; } = Guid.NewGuid().ToString();
    public override TraceContext TraceContext { get; } = null!;
    public override BindingContext BindingContext { get; } = null!;
    public override RetryContext RetryContext { get; } = null!;
    public override IServiceProvider InstanceServices { get => _serviceProvider; set { } }
    public override FunctionDefinition FunctionDefinition { get; } = null!;
    public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
    public override IInvocationFeatures Features { get; } = null!;
    public override CancellationToken CancellationToken { get; } = CancellationToken.None;
}

file sealed class TestHttpRequestData : HttpRequestData
{
    private TestHttpRequestData(FunctionContext functionContext, Uri url)
        : base(functionContext)
    {
        Url = url;
        Body = new MemoryStream();
        Headers = new HttpHeadersCollection();
        Cookies = [];
        Identities = [];
        Method = "GET";
    }

    public override Stream Body { get; }
    public override HttpHeadersCollection Headers { get; }
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; }
    public override Uri Url { get; }
    public override IEnumerable<System.Security.Claims.ClaimsIdentity> Identities { get; }
    public override string Method { get; }

    public override HttpResponseData CreateResponse()
    {
        return new TestHttpResponseData(FunctionContext);
    }

    public static TestHttpRequestData Create(string absoluteUrl)
    {
        return new TestHttpRequestData(new TestFunctionContext(), new Uri(absoluteUrl));
    }
}

file sealed class TestHttpResponseData : HttpResponseData
{
    public TestHttpResponseData(FunctionContext functionContext)
        : base(functionContext)
    {
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
        Cookies = new TestHttpCookies();
        StatusCode = HttpStatusCode.OK;
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }
    public override HttpCookies Cookies { get; }
}

file sealed class TestHttpCookies : HttpCookies
{
    private readonly List<IHttpCookie> _cookies = [];

    public override void Append(string name, string value)
    {
        _cookies.Add(new TestHttpCookie(name, value));
    }

    public override void Append(IHttpCookie cookie)
    {
        _cookies.Add(cookie);
    }

    public override IHttpCookie CreateNew()
    {
        return new TestHttpCookie(string.Empty, string.Empty);
    }
}

file sealed class TestHttpCookie(string name, string value) : IHttpCookie
{
    public string Name => name;
    public string Value => value;
    public string? Domain { get; } = null;
    public string? Path { get; } = null;
    public DateTimeOffset? Expires { get; } = null;
    public bool? HttpOnly { get; } = true;
    public bool? Secure { get; } = true;
    public SameSite SameSite { get; } = SameSite.None;
    public double? MaxAge { get; } = null;
}
