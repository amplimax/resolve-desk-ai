using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ResolveDeskAI.Api.Contracts;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class HealthEndpointTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HealthEndpointTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost")
                });
        }

        [Fact]
        public async Task GetHealth_ReturnsHealthyResponse()
        {
            var response = await _client.GetAsync("/health");

            response.EnsureSuccessStatusCode();

            var payload =
                await response.Content.ReadFromJsonAsync<HealthResponse>();

            Assert.NotNull(payload);
            Assert.Equal("Healthy", payload.Status);
            Assert.Equal("ResolveDeskAI.Api", payload.Service);
        }
    }
}