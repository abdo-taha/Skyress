using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skyress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDddPatternHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_SoldItems_InvoiceId",
                table: "SoldItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "refresh_tokens",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "refresh_tokens",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Items",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS pgcrypto;

                UPDATE refresh_tokens
                SET "TokenHash" = UPPER(ENCODE(DIGEST("Token", 'sha256'), 'hex'))
                WHERE "TokenHash" = '';

                UPDATE refresh_tokens
                SET "Token" = '';
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM "Invoices"
                        GROUP BY "BasketId"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Cannot add IX_Invoice_BasketId because duplicate invoices exist for one or more baskets.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM "Payments"
                        GROUP BY "InvoiceId"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Cannot add IX_Payment_InvoiceId because duplicate payments exist for one or more invoices.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM "SoldItems"
                        WHERE "ItemId" IS NOT NULL
                        GROUP BY "InvoiceId", "ItemId"
                        HAVING COUNT(*) > 1
                    ) THEN
                        RAISE EXCEPTION 'Cannot add IX_SoldItem_InvoiceId_ItemId because duplicate sold items exist for one or more invoice/item pairs.';
                    END IF;
                END $$;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_SoldItem_InvoiceId_ItemId",
                table: "SoldItems",
                columns: new[] { "InvoiceId", "ItemId" },
                unique: true,
                filter: "\"ItemId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_InvoiceId",
                table: "Payments",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BasketId",
                table: "Invoices",
                column: "BasketId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SoldItem_InvoiceId_ItemId",
                table: "SoldItems");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_Payment_InvoiceId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_BasketId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_SoldItems_InvoiceId",
                table: "SoldItems",
                column: "InvoiceId");
        }
    }
}
