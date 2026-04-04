using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialCode",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MaterialName",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "StockNum",
                table: "Materials",
                newName: "StockNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "StockNumber",
                table: "Materials",
                newName: "StockNum");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                table: "Materials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialCode",
                table: "Materials",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MaterialName",
                table: "Materials",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }
    }
}
