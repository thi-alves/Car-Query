using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarQuery.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Engine",
                table: "Car");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Car",
                type: "nvarchar(max)",
                maxLength: 5100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2100)",
                oldMaxLength: 2100);

            migrationBuilder.AlterColumn<string>(
                name: "EnginePosition",
                table: "Car",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "Drivetrain",
                table: "Car",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "Aspiration",
                table: "Car",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BodyStyle",
                table: "Car",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CylinderConfiguration",
                table: "Car",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Cylinders",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Displacement",
                table: "Car",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "Car",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Valves",
                table: "Car",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aspiration",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "BodyStyle",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "CylinderConfiguration",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Cylinders",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Displacement",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Car");

            migrationBuilder.DropColumn(
                name: "Valves",
                table: "Car");

            migrationBuilder.AlterColumn<string>(
                name: "FullDescription",
                table: "Car",
                type: "nvarchar(2100)",
                maxLength: 2100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5100);

            migrationBuilder.AlterColumn<string>(
                name: "EnginePosition",
                table: "Car",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Drivetrain",
                table: "Car",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45);

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "Car",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
