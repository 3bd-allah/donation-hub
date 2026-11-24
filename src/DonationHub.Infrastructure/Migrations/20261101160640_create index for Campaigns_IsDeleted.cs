using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonationHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createindexforCampaigns_IsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_IsDeleted",
                table: "Campaigns",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Campaigns_IsDeleted",
                table: "Campaigns");
        }
    }
}
