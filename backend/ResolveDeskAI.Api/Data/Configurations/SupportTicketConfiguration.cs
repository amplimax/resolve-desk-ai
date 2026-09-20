using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResolveDeskAI.Api.Domain.Entities;

namespace ResolveDeskAI.Api.Data.Configurations
{
    public sealed class SupportTicketConfiguration :
        IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(
            EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("SupportTickets");

            builder.HasKey(ticket => ticket.Id);

            builder.Property(ticket => ticket.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(ticket => ticket.Description)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(ticket => ticket.RequesterName)
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(ticket => ticket.RequesterEmail)
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(ticket => ticket.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(ticket => ticket.Priority)
                .HasConversion<string>()
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(ticket => ticket.Category)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(ticket => ticket.CreatedAtUtc)
                .IsRequired();

            builder.Property(ticket => ticket.UpdatedAtUtc)
                .IsRequired();

            builder.HasIndex(ticket => ticket.Status);

            builder.HasIndex(ticket => ticket.Category);

            builder.HasIndex(ticket => ticket.CreatedAtUtc);
        }
    }
}