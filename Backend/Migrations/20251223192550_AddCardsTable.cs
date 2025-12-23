using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCardsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                schema: "public",
                table: "Invoices",
                newName: "IssueDate");

            migrationBuilder.AddColumn<string>(
                name: "Period",
                schema: "public",
                table: "Invoices",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cards",
                schema: "public",
                columns: table => new
                {
                    CardID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    CardNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ExpiryDate = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    CVV = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CardHolderName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.CardID);
                    table.ForeignKey(
                        name: "FK_Cards_Users_UserID",
                        column: x => x.UserID,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UserID",
                schema: "public",
                table: "Cards",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cards",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Period",
                schema: "public",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "IssueDate",
                schema: "public",
                table: "Invoices",
                newName: "InvoiceDate");
        }
    }
}
