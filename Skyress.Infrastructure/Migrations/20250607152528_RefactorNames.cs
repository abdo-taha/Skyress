using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skyress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pricingChangeType",
                table: "PricingHistories",
                newName: "PricingChangeType");

            migrationBuilder.CreateIndex(
                name: "IX_PricingHistories_ItemId",
                table: "PricingHistories",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PricingHistories_Items_ItemId",
                table: "PricingHistories",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PricingHistories_Items_ItemId",
                table: "PricingHistories");

            migrationBuilder.DropIndex(
                name: "IX_PricingHistories_ItemId",
                table: "PricingHistories");

            migrationBuilder.RenameColumn(
                name: "PricingChangeType",
                table: "PricingHistories",
                newName: "pricingChangeType");
        }
    }
}
