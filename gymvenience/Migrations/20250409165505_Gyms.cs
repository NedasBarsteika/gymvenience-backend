using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gymvenience_backend.Migrations
{
    /// <inheritdoc />
    public partial class Gyms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GymId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Gyms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gyms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GymId",
                table: "Users",
                column: "GymId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Gyms_GymId",
                table: "Users",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Gyms_GymId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Gyms");

            migrationBuilder.DropIndex(
                name: "IX_Users_GymId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GymId",
                table: "Users");
        }
    }
}
