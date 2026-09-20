using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResolveDeskAI.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAiAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketAiAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SuggestedCategory = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SuggestedPriority = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    SuggestedResponse = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    MissingInformationJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnalyzedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketAiAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketAiAnalyses_SupportTickets_SupportTicketId",
                        column: x => x.SupportTicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketAiAnalyses_SupportTicketId",
                table: "TicketAiAnalyses",
                column: "SupportTicketId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketAiAnalyses");
        }
    }
}
