using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Acme.IdentityServer.WebApi.Data.Migrations.IdentityServer.PersistedGrantDb {
    public partial class initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants");

            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId",
                table: "PersistedGrants");

            migrationBuilder.DropIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId",
                table: "PersistedGrants");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.RenameTable(
                name: "PersistedGrants",
                newName: "PersistedGrants",
                newSchema: "auth");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersistedGrants",
                schema: "auth",
                table: "PersistedGrants",
                column: "Key");

            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                schema: "auth",
                columns: table => new {
                    UserCode = table.Column<string>(maxLength: 200, nullable: false),
                    DeviceCode = table.Column<string>(maxLength: 200, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_DeviceCodes", x => x.UserCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_DeviceCode",
                schema: "auth",
                table: "DeviceCodes",
                column: "DeviceCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "DeviceCodes",
                schema: "auth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersistedGrants",
                schema: "auth",
                table: "PersistedGrants");

            migrationBuilder.RenameTable(
                name: "PersistedGrants",
                schema: "auth",
                newName: "PersistedGrants");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersistedGrants",
                table: "PersistedGrants",
                columns: new[] { "Key", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId",
                table: "PersistedGrants",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId" });
        }
    }
}
