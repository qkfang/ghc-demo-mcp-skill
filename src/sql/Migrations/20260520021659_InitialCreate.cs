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
                name: "movie_table",
                columns: table => new
                {
                    m_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    m_available = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_table", x => x.m_id);
                    table.CheckConstraint("CK_movie_table_m_available", "m_available >= 0");
                });

            migrationBuilder.CreateTable(
                name: "order_table",
                columns: table => new
                {
                    o_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    m_id = table.Column<int>(type: "INTEGER", nullable: false),
                    no_tickets = table.Column<int>(type: "INTEGER", nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_table", x => x.o_id);
                    table.CheckConstraint("CK_order_table_no_tickets", "no_tickets > 0");
                    table.CheckConstraint("CK_order_table_price", "price >= 0");
                    table.ForeignKey(
                        name: "FK_order_table_movie_table_m_id",
                        column: x => x.m_id,
                        principalTable: "movie_table",
                        principalColumn: "m_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "movie_table",
                columns: new[] { "m_id", "m_available" },
                values: new object[,]
                {
                    { 1, 150 },
                    { 2, 4 },
                    { 3, 0 }
                });

            migrationBuilder.InsertData(
                table: "order_table",
                columns: new[] { "o_id", "m_id", "price", "no_tickets" },
                values: new object[,]
                {
                    { 1, 1, 200m, 2 },
                    { 2, 2, 540m, 6 }
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
