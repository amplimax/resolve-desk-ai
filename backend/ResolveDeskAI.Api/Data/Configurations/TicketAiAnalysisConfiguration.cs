using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResolveDeskAI.Api.Domain.Entities;

namespace ResolveDeskAI.Api.Data.Configurations
{
    public sealed class TicketAiAnalysisConfiguration :
        IEntityTypeConfiguration<TicketAiAnalysis>
    {
        public void Configure(
            EntityTypeBuilder<TicketAiAnalysis> builder)
        {
            builder.ToTable("TicketAiAnalyses");

            builder.HasKey(analysis => analysis.Id);

            builder.Property(analysis => analysis.Summary)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(analysis => analysis.SuggestedCategory)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(analysis => analysis.SuggestedPriority)
                .HasConversion<string>()
                .HasMaxLength(16)
                .IsRequired();

            builder.Property(analysis => analysis.SuggestedResponse)
                .HasMaxLength(4000)
                .IsRequired();

            builder.Property(analysis => analysis.MissingInformationJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(analysis => analysis.Provider)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(analysis => analysis.Model)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(analysis => analysis.AnalyzedAtUtc)
                .IsRequired();

            builder.HasIndex(analysis => analysis.SupportTicketId)
                .IsUnique();

            builder.HasOne(analysis => analysis.SupportTicket)
                .WithOne(ticket => ticket.AiAnalysis)
                .HasForeignKey<TicketAiAnalysis>(
                    analysis => analysis.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}