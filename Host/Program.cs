using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder
.Services
.AddReverseProxy()
.LoadFromMemory(new[]
    {
        new RouteConfig
        {
            RouteId = "api",
            ClusterId = "api",
            Match = new() { Path = "/api/{**catch-all}" },
            Transforms = new[]
            {
                new Dictionary<string, string>
                {
                    { "PathRemovePrefix", "/api" }
                }
            }
        },
        new RouteConfig
        {
            RouteId = "app",
            ClusterId = "app",
            Match = new() { Path = "{**catch-all}" }
        }
    },
    new[]
    {
        new ClusterConfig
        {
            ClusterId = "api",
            Destinations = new Destinations
            {
                { "destination_api", new() { Address = "https://localhost:7210" } }
            }
        }
    });

var app = builder.Build();

app.MapReverseProxy();
app.MapGet("/", () => "Hello World!");

app.Run();

// Ain't nobody got time to say new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
// every damn time. Not today, not ever!
internal sealed class Destinations : Dictionary<string, DestinationConfig>
{
    public Destinations() :base(StringComparer.OrdinalIgnoreCase)
    {
    }
}

