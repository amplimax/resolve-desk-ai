using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Services.Tickets;
using ResolveDeskAI.Api.Services.AI;
using Microsoft.Extensions.Options;
using ResolveDeskAI.Api.Configuration;
using ResolveDeskAI.Api.Hosting;

namespace ResolveDeskAI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString =
                builder.Configuration.GetConnectionString(
                    "DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");

            builder.Services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter());
                });

            builder.Services.AddDbContext<ResolveDeskDbContext>(
                options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<
                ISupportTicketService,
                SupportTicketService>();

            builder.Services.Configure<AiOptions>(
                builder.Configuration.GetSection(
                    AiOptions.SectionName));

            builder.Services.AddSingleton<MockTicketAnalyzer>();
            builder.Services.AddSingleton<OpenAiTicketAnalyzer>();

            builder.Services.AddSingleton<ITicketAnalyzer>(
                serviceProvider =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<AiOptions>>()
                        .Value;

                    if (options.Provider.Equals(
                        "OpenAI",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return serviceProvider
                            .GetRequiredService<OpenAiTicketAnalyzer>();
                    }

                    if (options.Provider.Equals(
                        "MockAI",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return serviceProvider
                            .GetRequiredService<MockTicketAnalyzer>();
                    }

                    throw new InvalidOperationException(
                        $"Unsupported AI provider '{options.Provider}'.");
                });

            builder.Services.AddScoped<
                ITicketAnalysisService,
                TicketAnalysisService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseWebsiteIndexing();
            app.UseStaticFiles();
            // Serve real assets before selecting the catch-all 404 endpoint.
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllers();

            app.MapWebsite();

            app.Run();
        }
    }
}
