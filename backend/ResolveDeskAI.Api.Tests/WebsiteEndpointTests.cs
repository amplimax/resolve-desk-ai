using System.Net;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Domain.Entities;

namespace ResolveDeskAI.Api.Tests
{
    public sealed class WebsiteFactory : WebApplicationFactory<Program>
    {
        public const string ExistingTicketId = "f982068e-9b87-4afb-8f53-b3330bfb0d75";
        private readonly string _databaseName = $"WebsiteTests-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ResolveDeskDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<ResolveDeskDbContext>>();
                services.AddDbContext<ResolveDeskDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }
    }

    public sealed class WebsiteEndpointTests : IClassFixture<WebsiteFactory>
    {
        private readonly HttpClient _client;

        public WebsiteEndpointTests(WebsiteFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false,
            });

            using var scope = factory.Services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<ResolveDeskDbContext>();
            var id = Guid.Parse(WebsiteFactory.ExistingTicketId);
            if (!database.SupportTickets.Any(ticket => ticket.Id == id))
            {
                database.SupportTickets.Add(new SupportTicket { Id = id, Title = "Example ticket" });
                database.SaveChanges();
            }
        }

        [Fact]
        public async Task Home_IsIndexableAndContainsPresentationWithoutJavaScript()
        {
            var response = await _client.GetAsync("/");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(response.Headers.Contains("X-Robots-Tag"));
            Assert.Contains("AI-assisted support ticket management</title>", html);
            Assert.Contains("A clearer path from support request to resolution.", html);
            Assert.Contains("name=\"description\"", html);
            Assert.Contains("rel=\"canonical\" href=\"https://resolvedeskai.anywaresolutions.eu/\"", html);
            Assert.Contains("href=\"/dashboard\"", html);
            Assert.DoesNotContain("noindex", html);
        }

        [Theory]
        [InlineData("/dashboard")]
        [InlineData("/dashboard/")]
        [InlineData("/tickets")]
        [InlineData("/tickets/new")]
        [InlineData("/tickets/" + WebsiteFactory.ExistingTicketId)]
        [InlineData("/tickets/" + WebsiteFactory.ExistingTicketId + "/edit")]
        public async Task DemoRoutes_ReturnNonIndexableAngularShell(string path)
        {
            var response = await _client.GetAsync(path);
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("noindex", response.Headers.GetValues("X-Robots-Tag").Single());
            Assert.Contains("name=\"robots\" content=\"noindex, follow\"", html);
            Assert.DoesNotContain("rel=\"canonical\"", html);
            Assert.DoesNotContain("A clearer path from support request to resolution.", html);
        }

        [Theory]
        [InlineData("/does-not-exist")]
        [InlineData("/missing.html")]
        [InlineData("/missing.js")]
        [InlineData("/dashboard/unknown")]
        [InlineData("/tickets/invalid")]
        [InlineData("/tickets/new/edit")]
        [InlineData("/tickets/00000000-0000-0000-0000-000000000000")]
        [InlineData("/tickets/00000000-0000-0000-0000-000000000000/edit")]
        [InlineData("/not-found")]
        [InlineData("/not-found/index.html")]
        [InlineData("/index.csr.html")]
        public async Task UnknownPages_ReturnStaticErrorWithReal404(string path)
        {
            var response = await _client.GetAsync(path);
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("Page not found", html);
            Assert.Contains("noindex", response.Headers.GetValues("X-Robots-Tag").Single());
            Assert.DoesNotContain("<script", html);
            Assert.DoesNotContain("rel=\"canonical\"", html);
        }

        [Theory]
        [InlineData("/", HttpStatusCode.OK)]
        [InlineData("/dashboard", HttpStatusCode.OK)]
        [InlineData("/unknown", HttpStatusCode.NotFound)]
        [InlineData("/tickets/00000000-0000-0000-0000-000000000000", HttpStatusCode.NotFound)]
        public async Task Head_HasTheSameStatusWithoutBody(string path, HttpStatusCode expected)
        {
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, path));
            Assert.Equal(expected, response.StatusCode);
            Assert.Empty(await response.Content.ReadAsByteArrayAsync());
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        public async Task UnknownApi_DoesNotReturnWebsiteHtml(string method)
        {
            var response = await _client.SendAsync(
                new HttpRequestMessage(new HttpMethod(method), "/api/unknown"));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.DoesNotContain("<html", await response.Content.ReadAsStringAsync());
            Assert.Contains("noindex", response.Headers.GetValues("X-Robots-Tag").Single());
        }

        [Fact]
        public async Task IndexAlias_RedirectsPermanentlyToHome()
        {
            var response = await _client.GetAsync("/index.html");
            Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.Equal("/", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task ReferencedScriptsAndStyles_AreServedAsAssets()
        {
            var html = await _client.GetStringAsync("/dashboard");
            var assets = Regex.Matches(html, "(?:src|href)=\"([^\"]+\\.(?:js|css))\"")
                .Select(match => match.Groups[1].Value)
                .Where(url => !url.StartsWith("http"))
                .ToArray();
            Assert.NotEmpty(assets);
            foreach (var asset in assets)
            {
                var response = await _client.GetAsync("/" + asset.TrimStart('/'));
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.DoesNotContain("text/html", response.Content.Headers.ContentType?.ToString() ?? "");
            }
        }

        [Fact]
        public async Task RobotsAndSitemap_AllowNoindexToBeReadAndListOnlyHome()
        {
            var robots = await _client.GetStringAsync("/robots.txt");
            Assert.Contains("Sitemap: https://resolvedeskai.anywaresolutions.eu/sitemap.xml", robots);
            Assert.DoesNotContain("Disallow:", robots);

            var sitemap = XDocument.Parse(await _client.GetStringAsync("/sitemap.xml"));
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            Assert.Equal("https://resolvedeskai.anywaresolutions.eu/",
                Assert.Single(sitemap.Descendants(ns + "loc")).Value);
        }
    }
}
