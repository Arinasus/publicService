using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    public partial class BaseEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                schema: "public",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.UserID); });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "public",
                columns: table => new
                {
                    AddressID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Street = table.Column<string>(maxLength: 100, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Addresses", x => x.AddressID); });

            migrationBuilder.CreateTable(
                name: "Providers",
                schema: "public",
                columns: table => new
                {
                    ProviderID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderName = table.Column<string>(maxLength: 100, nullable: false),
                    ContactPerson = table.Column<string>(maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Providers", x => x.ProviderID); });

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "public",
                columns: table => new
                {
                    ServiceID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceName = table.Column<string>(maxLength: 100, nullable: false),
                    UnitOfMeasure = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Services", x => x.ServiceID); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Services", schema: "public");
            migrationBuilder.DropTable(name: "Providers", schema: "public");
            migrationBuilder.DropTable(name: "Addresses", schema: "public");
            migrationBuilder.DropTable(name: "Users", schema: "public");
        }
    }
}
