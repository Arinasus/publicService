using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SyncUsers : Migration
    {
        /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.CreateTable(
                    name: "AuthTokens",
                    schema: "public",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.SerialColumn),
                        Token = table.Column<string>(type: "text", nullable: false),
                        CreatedAt = table.Column<DateTime>(nullable: false),
                        UserID = table.Column<int>(nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AuthTokens", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AuthTokens_Users_UserID",
                            column: x => x.UserID,
                            principalSchema: "public",
                            principalTable: "Users",
                            principalColumn: "UserID",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_AuthTokens_UserID",
                    schema: "public",
                    table: "AuthTokens",
                    column: "UserID");
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropTable(
                    name: "AuthTokens",
                    schema: "public");
            }
     }
}
