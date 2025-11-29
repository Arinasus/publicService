using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class Contracts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "public",
                columns: table => new
                {
                    ContractID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(nullable: false),
                    AddressID = table.Column<int>(nullable: false),
                    ServiceID = table.Column<int>(nullable: false),
                    ProviderID = table.Column<int>(nullable: false),
                    ContractNumber = table.Column<string>(maxLength: 50, nullable: false),
                    ContractStartDate = table.Column<DateTime>(nullable: false),
                    ContractEndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractID);
                    table.ForeignKey("FK_Contracts_Users_UserID", x => x.UserID, "Users", "UserID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Contracts_Addresses_AddressID", x => x.AddressID, "Addresses", "AddressID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Contracts_Services_ServiceID", x => x.ServiceID, "Services", "ServiceID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_Contracts_Providers_ProviderID", x => x.ProviderID, "Providers", "ProviderID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_Contracts_UserID", schema: "public", table: "Contracts", column: "UserID");
            migrationBuilder.CreateIndex(name: "IX_Contracts_AddressID", schema: "public", table: "Contracts", column: "AddressID");
            migrationBuilder.CreateIndex(name: "IX_Contracts_ServiceID", schema: "public", table: "Contracts", column: "ServiceID");
            migrationBuilder.CreateIndex(name: "IX_Contracts_ProviderID", schema: "public", table: "Contracts", column: "ProviderID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Contracts", schema: "public");
        }
    }
}
