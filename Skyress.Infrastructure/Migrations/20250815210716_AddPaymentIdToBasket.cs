using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skyress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIdToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentId",
                table: "Baskets",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Baskets");
        }
    }
}
