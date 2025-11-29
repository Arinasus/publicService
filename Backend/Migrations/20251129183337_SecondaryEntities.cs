using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class SecondaryEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "public",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractID = table.Column<int>(nullable: false),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceID);
                    table.ForeignKey("FK_Invoices_Contracts_ContractID", x => x.ContractID, "Contracts", "ContractID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "public",
                columns: table => new
                {
                    PaymentID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceID = table.Column<int>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    PaymentAmount = table.Column<decimal>(nullable: false),
                    PaymentMethod = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey("FK_Payments_Invoices_InvoiceID", x => x.InvoiceID, "Invoices", "InvoiceID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "public",
                columns: table => new
                {
                    NotificationID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(nullable: false),
                    NotificationDate = table.Column<DateTime>(nullable: false),
                    NotificationType = table.Column<string>(maxLength: 20, nullable: false),
                    NotificationText = table.Column<string>(maxLength: 200, nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey("FK_Notifications_Users_UserID", x => x.UserID, "Users", "UserID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "public",
                columns: table => new
                {
                    UserAddressID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(nullable: false),
                    AddressID = table.Column<int>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.UserAddressID);
                    table.ForeignKey("FK_UserAddresses_Users_UserID", x => x.UserID, "Users", "UserID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_UserAddresses_Addresses_AddressID", x => x.AddressID, "Addresses", "AddressID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeterReadings",
                schema: "public",
                columns: table => new
                {
                    ReadingID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContractID = table.Column<int>(nullable: false),
                    ReadingDate = table.Column<DateTime>(nullable: false),
                    ReadingValue = table.Column<decimal>(nullable: false),
                    SubmittedByUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.ReadingID);
                    table.ForeignKey("FK_MeterReadings_Contracts_ContractID", x => x.ContractID, "Contracts", "ContractID", principalSchema: "public", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_MeterReadings_Users_SubmittedByUserID", x => x.SubmittedByUserID, "Users", "UserID", principalSchema: "public");
                });

            migrationBuilder.CreateIndex(name: "IX_Invoices_ContractID", schema: "public", table: "Invoices", column: "ContractID");
            migrationBuilder.CreateIndex(name: "IX_Payments_InvoiceID", schema: "public", table: "Payments", column: "InvoiceID");
            migrationBuilder.CreateIndex(name: "IX_Notifications_UserID", schema: "public", table: "Notifications", column: "UserID");
            migrationBuilder.CreateIndex(name: "IX_UserAddresses_UserID", schema: "public", table: "UserAddresses", column: "UserID");
            migrationBuilder.CreateIndex(name: "IX_UserAddresses_AddressID", schema: "public", table: "UserAddresses", column: "AddressID");
            migrationBuilder.CreateIndex(name: "IX_MeterReadings_ContractID", schema: "public", table: "MeterReadings", column: "ContractID");
            migrationBuilder.CreateIndex(name: "IX_MeterReadings_SubmittedByUserID", schema: "public", table: "MeterReadings", column: "SubmittedByUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MeterReadings", schema: "public");
            migrationBuilder.DropTable(name: "Notifications", schema: "public");
            migrationBuilder.DropTable(name: "Payments", schema: "public");
            migrationBuilder.DropTable(name: "UserAddresses", schema: "public");
            migrationBuilder.DropTable(name: "Invoices", schema: "public");
        }
    }
}
