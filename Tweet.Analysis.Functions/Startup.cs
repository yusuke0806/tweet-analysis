using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Tweet.Analysis.Services;

namespace Tweet.Analysis;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient<IFilteredStreamService, FilteredStreamService>(
            client =>
            {
                client.BaseAddress = new Uri("https://api.twitter.com/2/tweets/search/stream/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("BearerToken"));
            });
    }
}