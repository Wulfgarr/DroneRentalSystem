using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DroneRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRentalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Drones_DroneID",
                table: "Rentals");

            migrationBuilder.RenameColumn(
                name: "DroneID",
                table: "Rentals",
                newName: "DroneId");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "Rentals",
                newName: "TotalPrice");

            migrationBuilder.RenameIndex(
                name: "IX_Rentals_DroneID",
                table: "Rentals",
                newName: "IX_Rentals_DroneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Drones_DroneId",
                table: "Rentals",
                column: "DroneId",
                principalTable: "Drones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Drones_DroneId",
                table: "Rentals");

            migrationBuilder.RenameColumn(
                name: "DroneId",
                table: "Rentals",
                newName: "DroneID");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Rentals",
                newName: "TotalCost");

            migrationBuilder.RenameIndex(
                name: "IX_Rentals_DroneId",
                table: "Rentals",
                newName: "IX_Rentals_DroneID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Drones_DroneID",
                table: "Rentals",
                column: "DroneID",
                principalTable: "Drones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
