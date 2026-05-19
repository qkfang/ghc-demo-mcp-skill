using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GhcDemo.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LegacyMovieId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    AvailableTickets = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.MovieId);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_orders_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_line_items",
                columns: table => new
                {
                    OrderLineItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_line_items", x => x.OrderLineItemId);
                    table.ForeignKey(
                        name: "FK_order_line_items_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "MovieId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_line_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "CustomerId", "CreatedAtUtc", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 6, 15, 30, 0, 0, DateTimeKind.Utc), "taylor.nguyen@example.com", "Taylor", "Nguyen", "+1-555-0101" },
                    { 2, new DateTime(2025, 1, 8, 9, 0, 0, 0, DateTimeKind.Utc), "jordan.patel@example.com", "Jordan", "Patel", "+1-555-0102" }
                });

            migrationBuilder.InsertData(
                table: "movies",
                columns: new[] { "MovieId", "AvailableTickets", "Language", "LegacyMovieId", "Title", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 120, "English", 1, "Interstellar", 100m },
                    { 2, 75, "Japanese", 2, "Spirited Away", 90m },
                    { 3, 40, "English", 3, "The Dark Knight", 80m }
                });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "OrderId", "CustomerId", "OrderNumber", "OrderedAtUtc", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, "ORD-20250110-0001", new DateTime(2025, 1, 10, 17, 0, 0, 0, DateTimeKind.Utc), "Confirmed", 360m },
                    { 2, 1, "ORD-20250111-0002", new DateTime(2025, 1, 11, 18, 15, 0, 0, DateTimeKind.Utc), "Confirmed", 500m },
                    { 3, 2, "ORD-20250112-0003", new DateTime(2025, 1, 12, 19, 45, 0, 0, DateTimeKind.Utc), "Pending", 160m }
                });

            migrationBuilder.InsertData(
                table: "order_line_items",
                columns: new[] { "OrderLineItemId", "LineTotal", "MovieId", "OrderId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 360m, 2, 1, 4, 90m },
                    { 2, 500m, 1, 2, 5, 100m },
                    { 3, 160m, 3, 3, 2, 80m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_customers_Email",
                table: "customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_LegacyMovieId",
                table: "movies",
                column: "LegacyMovieId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_line_items_MovieId",
                table: "order_line_items",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_order_line_items_OrderId_MovieId",
                table: "order_line_items",
                columns: new[] { "OrderId", "MovieId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderNumber",
                table: "orders",
                column: "OrderNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_line_items");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "customers");
        }
    }
}
