using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Data;
using ResolveDeskAI.Api.Services.Tickets;
using ResolveDeskAI.Api.Services.AI;

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

            builder.Services.AddSingleton<
                ITicketAnalyzer,
                MockTicketAnalyzer>();

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
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}