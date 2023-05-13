using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace SafeSender.StorageAPI.Tests.IntegrationTests;

public static class SystemUnderTest
{
    public static FlurlClient GetClient()
    {
        var factory = new WebApplicationFactory<Program>();
        var httpClient = factory.CreateClient();

        return new FlurlClient(httpClient);
    }
}