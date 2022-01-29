using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Acme.ShoppingCart.Data.Migrations {
    public partial class initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Subject",
                schema: "dbo",
                columns: table => new {
                    SubjectId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    GivenName = table.Column<string>(maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(maxLength: 100, nullable: true),
                    UserPrincipalName = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Subject", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "Widget",
                schema: "dbo",
                columns: table => new {
                    WidgetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreateSubjectId = table.Column<Guid>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedSubjectId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(maxLength: 100, nullable: true),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Widget", x => x.WidgetId);
                    table.ForeignKey(
                        name: "FK_Widget_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "SubjectId");
                    table.ForeignKey(
                        name: "FK_Widget_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalSchema: "dbo",
                        principalTable: "Subject",
                        principalColumn: "SubjectId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Widget_CreateSubjectId",
                schema: "dbo",
                table: "Widget",
                column: "CreateSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_LastModifiedSubjectId",
                schema: "dbo",
                table: "Widget",
                column: "LastModifiedSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "Widget",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Subject",
                schema: "dbo");
        }
    }
}
