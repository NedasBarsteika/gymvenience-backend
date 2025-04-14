using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gymvenience.Migrations
{
    /// <inheritdoc />
    public partial class GymId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerAvailabilities_Users_TrainerId",
                table: "TrainerAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_TrainerAvailabilities_TrainerId",
                table: "TrainerAvailabilities");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "TrainerAvailabilities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "GymId",
                table: "TrainerAvailabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GymId",
                table: "TrainerAvailabilities");

            migrationBuilder.AlterColumn<string>(
                name: "TrainerId",
                table: "TrainerAvailabilities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAvailabilities_TrainerId",
                table: "TrainerAvailabilities",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerAvailabilities_Users_TrainerId",
                table: "TrainerAvailabilities",
                column: "TrainerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
