using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skyress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCoumnNameTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_soldItems",
                table: "soldItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pricingHistories",
                table: "pricingHistories");

            migrationBuilder.RenameTable(
                name: "soldItems",
                newName: "SoldItems");

            migrationBuilder.RenameTable(
                name: "pricingHistories",
                newName: "PricingHistories");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Todos",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "SoldItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "PricingHistories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Payments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Items",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Invoices",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Installments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreaedAt",
                table: "Customers",
                newName: "CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoldItems",
                table: "SoldItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PricingHistories",
                table: "PricingHistories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SoldItems",
                table: "SoldItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PricingHistories",
                table: "PricingHistories");

            migrationBuilder.RenameTable(
                name: "SoldItems",
                newName: "soldItems");

            migrationBuilder.RenameTable(
                name: "PricingHistories",
                newName: "pricingHistories");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Todos",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "soldItems",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "pricingHistories",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Payments",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Items",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Invoices",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Installments",
                newName: "CreaedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Customers",
                newName: "CreaedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_soldItems",
                table: "soldItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pricingHistories",
                table: "pricingHistories",
                column: "Id");
        }
    }
}
