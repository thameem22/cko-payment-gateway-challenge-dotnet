using Microsoft.AspNetCore.Mvc.Testing;

namespace PaymentGateway.Api.Tests.IntegrationTests;

public class IntegrationTestFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; }

    public IntegrationTestFixture()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }

    public void Dispose()
    {
        Client?.Dispose();
        Factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}



