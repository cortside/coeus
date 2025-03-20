using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class OutboxRemainingAttemptsDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RemainingAttempts",
                schema: "dbo",
                table: "Outbox",
                type: "int",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RemainingAttempts",
                schema: "dbo",
                table: "Outbox",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 10);
        }
    }
}
