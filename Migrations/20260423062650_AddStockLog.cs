using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStockLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    OperateType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeCount = table.Column<int>(type: "int", nullable: false),
                    BeforeStock = table.Column<int>(type: "int", nullable: false),
                    AfterStock = table.Column<int>(type: "int", nullable: false),
                    OperateUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockLogs");
        }
    }
}
