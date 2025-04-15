using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gymvenience.Migrations
{
    /// <inheritdoc />
    public partial class fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Gyms_GymId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_GymId",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "GymId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GymId",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_GymId",
                table: "Reservations",
                column: "GymId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Gyms_GymId",
                table: "Reservations",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
