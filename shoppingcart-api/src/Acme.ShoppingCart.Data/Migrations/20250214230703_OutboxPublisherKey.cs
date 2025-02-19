using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class OutboxPublisherKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                schema: "dbo",
                table: "Outbox",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Key",
                schema: "dbo",
                table: "Outbox");
        }
    }
}
