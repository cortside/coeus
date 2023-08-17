using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    public partial class AddCustomerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerTypeId",
                schema: "dbo",
                table: "Customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerType",
                schema: "dbo",
                columns: table => new
                {
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxExempt = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time entity was created"),
                    CreateSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Date and time entity was last modified"),
                    LastModifiedSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerType", x => x.CustomerTypeId);
                    table.ForeignKey(
                        name: "FK_CustomerType_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "SubjectId");
                    table.ForeignKey(
                        name: "FK_CustomerType_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "SubjectId");
                },
                comment: "Customer types");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerTypeId",
                schema: "dbo",
                table: "Customer",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerType_CreateSubjectId",
                schema: "dbo",
                table: "CustomerType",
                column: "CreateSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerType_LastModifiedSubjectId",
                schema: "dbo",
                table: "CustomerType",
                column: "LastModifiedSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_CustomerType_CustomerTypeId",
                schema: "dbo",
                table: "Customer",
                column: "CustomerTypeId",
                principalSchema: "dbo",
                principalTable: "CustomerType",
                principalColumn: "CustomerTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customer_CustomerType_CustomerTypeId",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropTable(
                name: "CustomerType",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CustomerTypeId",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CustomerTypeId",
                schema: "dbo",
                table: "Customer");
        }
    }
}
