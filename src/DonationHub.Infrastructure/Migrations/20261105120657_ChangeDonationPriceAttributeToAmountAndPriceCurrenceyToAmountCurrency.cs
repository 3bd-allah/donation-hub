using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonationHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDonationPriceAttributeToAmountAndPriceCurrenceyToAmountCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriceCurrency",
                table: "Donations",
                newName: "AmountCurrency");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Donations",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmountCurrency",
                table: "Donations",
                newName: "PriceCurrency");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Donations",
                newName: "Price");
        }
    }
}
