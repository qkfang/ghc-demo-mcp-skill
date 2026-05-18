using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppMod.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movie_table",
                columns: table => new
                {
                    m_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    m_title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    m_genre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ticket_price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    m_available = table.Column<int>(type: "int", nullable: false),
                    show_time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_table", x => x.m_id);
                });

            migrationBuilder.CreateTable(
                name: "order_table",
                columns: table => new
                {
                    o_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    m_id = table.Column<int>(type: "int", nullable: false),
                    no_tickets = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ordered_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_table", x => x.o_id);
                    table.ForeignKey(
                        name: "FK_order_table_movie_table_m_id",
                        column: x => x.m_id,
                        principalTable: "movie_table",
                        principalColumn: "m_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "movie_table",
                columns: new[] { "m_id", "m_available", "m_genre", "show_time", "ticket_price", "m_title" },
                values: new object[,]
                {
                    { 1, 50, "Sci-Fi", new DateTime(2025, 1, 15, 19, 30, 0, 0, DateTimeKind.Utc), 100.00m, "Interstellar" },
                    { 2, 38, "Thriller", new DateTime(2025, 1, 15, 21, 0, 0, 0, DateTimeKind.Utc), 100.00m, "Inception" },
                    { 3, 25, "Action", new DateTime(2025, 1, 16, 20, 0, 0, 0, DateTimeKind.Utc), 120.00m, "The Dark Knight" }
                });

            migrationBuilder.InsertData(
                table: "order_table",
                columns: new[] { "o_id", "m_id", "ordered_at", "no_tickets", "price" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 10, 10, 15, 0, 0, DateTimeKind.Utc), 2, 200.00m },
                    { 2, 2, new DateTime(2025, 1, 10, 11, 0, 0, 0, DateTimeKind.Utc), 1, 100.00m },
                    { 3, 3, new DateTime(2025, 1, 10, 12, 30, 0, 0, DateTimeKind.Utc), 4, 480.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_table_m_id",
                table: "order_table",
                column: "m_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_table");

            migrationBuilder.DropTable(
                name: "movie_table");
        }
    }
}
