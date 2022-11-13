using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;

namespace Tweet.Analysis.Http;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigurationService(IServiceCollection services)
    {
        var httpClientName = Configuration["FilteredStreamClientName"];
        services.AddHttpClient(
            httpClientName,
            client =>
            {
                // Set the base address of the named client.
                client.BaseAddress = new Uri("https://api.twitter.com/2/tweets/search/stream/");
            });
    }
    
}