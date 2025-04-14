using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Skyress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSkyressDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Installments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<long>(type: "bigint", nullable: false),
                    DueAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    paymentState = table.Column<int>(type: "integer", nullable: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Installments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TotalAmount = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    CostPrice = table.Column<double>(type: "numeric(18,2)", nullable: true),
                    QuantityLeft = table.Column<int>(type: "integer", nullable: false),
                    QuantitySold = table.Column<int>(type: "integer", nullable: false),
                    QrCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    PaymentState = table.Column<int>(type: "integer", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    TotalDue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pricingHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    OldPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OldCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NewCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    pricingChangeType = table.Column<int>(type: "integer", nullable: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricingHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "soldItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    SellingTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: true),
                    ItemCost = table.Column<double>(type: "numeric(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soldItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagAssignments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAssignments", x => x.Id);
                    table.UniqueConstraint("AK_TagAssignments_TagId_ItemId", x => new { x.TagId, x.ItemId });
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'100000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    State = table.Column<int>(type: "integer", nullable: false),
                    context = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LastEditBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastEditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreaedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Installments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "pricingHistories");

            migrationBuilder.DropTable(
                name: "soldItems");

            migrationBuilder.DropTable(
                name: "TagAssignments");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
