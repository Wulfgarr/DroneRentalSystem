using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DroneRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDroneAvailbility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Drones",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Drones",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Drones");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Drones",
                newName: "ID");
        }
    }
}
