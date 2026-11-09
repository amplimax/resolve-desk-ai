using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Data;

namespace ResolveDeskAI.Api.Hosting
{
    public static class WebsiteEndpoints
    {
        private static readonly string[] PageMethods = [HttpMethods.Get, HttpMethods.Head];

        public static void UseWebsiteIndexing(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                // Capture the original URL before any static-file handling.
                var path = context.Request.Path;
                var noIndex = path.StartsWithSegments("/dashboard")
                    || path.StartsWithSegments("/tickets")
                    || path.StartsWithSegments("/api")
                    || path.StartsWithSegments("/health")
                    || path.StartsWithSegments("/not-found")
                    || path == "/index.csr.html";

                context.Response.OnStarting(() =>
                {
                    if (noIndex || context.Response.StatusCode >= 400)
                    {
                        context.Response.Headers["X-Robots-Tag"] = "noindex, follow";
                    }
                    return Task.CompletedTask;
                });

                if (path == "/index.html")
                {
                    context.Response.Redirect("/" + context.Request.QueryString, permanent: true);
                    return;
                }

                // Internal HTML documents must not bypass the page endpoints/status codes.
                if (path == "/index.csr.html" || path == "/not-found/index.html")
                {
                    await SendNotFoundAsync(context, app.Environment);
                    return;
                }

                await next(context);
            });
        }

        public static void MapWebsite(this WebApplication app)
        {
            app.MapMethods("/", PageMethods,
                context => SendPageAsync(context, app.Environment, "index.html"));

            foreach (var route in new[] { "/dashboard", "/tickets", "/tickets/new" })
            {
                app.MapMethods(route, PageMethods,
                    context => SendPageAsync(context, app.Environment, "index.csr.html"));
            }

            foreach (var route in new[] { "/tickets/{id:guid}", "/tickets/{id:guid}/edit" })
            {
                app.MapMethods(route, PageMethods, async (
                    Guid id, HttpContext context, ResolveDeskDbContext database) =>
                {
                    var exists = await database.SupportTickets
                        .AnyAsync(ticket => ticket.Id == id, context.RequestAborted);

                    if (exists)
                    {
                        await SendPageAsync(context, app.Environment, "index.csr.html");
                    }
                    else
                    {
                        await SendNotFoundAsync(context, app.Environment);
                    }
                });
            }

            app.MapMethods("/not-found", PageMethods,
                context => SendNotFoundAsync(context, app.Environment));

            // Reserve the API namespace; an unknown API must never return the SPA.
            app.Map("/api/{**path}", () => Results.NotFound());

            // Match dotted paths and missing assets too, without a successful SPA fallback.
            app.MapFallback("/{**path}", context => SendNotFoundAsync(context, app.Environment));
        }

        private static Task SendNotFoundAsync(HttpContext context, IWebHostEnvironment environment) =>
            SendPageAsync(context, environment, "not-found/index.html", StatusCodes.Status404NotFound);

        private static async Task SendPageAsync(
            HttpContext context, IWebHostEnvironment environment, string fileName,
            int statusCode = StatusCodes.Status200OK)
        {
            var file = environment.WebRootFileProvider.GetFileInfo(fileName);
            if (!file.Exists)
            {
                // A missing build is an unavailable site, never an indexable empty page.
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return;
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength = file.Length;
            context.Response.Headers.CacheControl = "no-cache";

            if (!HttpMethods.IsHead(context.Request.Method))
            {
                await context.Response.SendFileAsync(file, context.RequestAborted);
            }
        }
    }
}
