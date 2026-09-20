using Microsoft.EntityFrameworkCore;
using ResolveDeskAI.Api.Domain.Entities;

namespace ResolveDeskAI.Api.Data
{
    public sealed class ResolveDeskDbContext : DbContext
    {
        public ResolveDeskDbContext(
            DbContextOptions<ResolveDeskDbContext> options)
            : base(options)
        {
        }

        public DbSet<SupportTicket> SupportTickets =>
            Set<SupportTicket>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ResolveDeskDbContext).Assembly);
        }
    }
}