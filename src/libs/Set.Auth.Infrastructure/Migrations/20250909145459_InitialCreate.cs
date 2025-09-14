using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Set.Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Resource = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Avatar = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEmailVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PermissionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GrantedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Token = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RevokedByIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReplacedByToken = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AssignedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Action", "CreatedAt", "Description", "IsActive", "Name", "Resource", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0e59639f-c759-450e-9092-88d0530d6cd9"), "read", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9121), "Permission to read users", true, "users.read", "users", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9121) },
                    { new Guid("3c0d6b7f-f303-416c-ad14-dd88035db7ff"), "write", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9123), "Permission to write users", true, "users.write", "users", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9124) },
                    { new Guid("4d67791a-14aa-4590-910d-88593fe908f4"), "write", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9132), "Permission to write profile", true, "profile.write", "profile", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9132) },
                    { new Guid("87c0e273-796d-4c52-a27d-a803883a8c79"), "delete", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9125), "Permission to delete users", true, "users.delete", "users", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9125) },
                    { new Guid("a845595e-b1b8-4f35-9d6e-777e6000aee0"), "delete", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9129), "Permission to delete roles", true, "roles.delete", "roles", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9129) },
                    { new Guid("bdb6b201-eccd-4456-a431-99c18710e693"), "read", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9131), "Permission to read profile", true, "profile.read", "profile", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9131) },
                    { new Guid("c19fc3bf-6204-492a-bf61-5434d627d3f2"), "read", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9127), "Permission to read roles", true, "roles.read", "roles", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9127) },
                    { new Guid("e2861b35-bb82-4896-b7ce-9cad6bb42b6b"), "write", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9128), "Permission to write roles", true, "roles.write", "roles", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9128) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5ee80dca-027d-448d-805c-cb9aa3ce7fc6"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(8953), "Standard user role", true, "User", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(8953) },
                    { new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(8949), "Administrator role with full access", true, "Admin", new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(8951) }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "GrantedAt", "IsActive" },
                values: new object[,]
                {
                    { new Guid("4d67791a-14aa-4590-910d-88593fe908f4"), new Guid("5ee80dca-027d-448d-805c-cb9aa3ce7fc6"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9221), true },
                    { new Guid("bdb6b201-eccd-4456-a431-99c18710e693"), new Guid("5ee80dca-027d-448d-805c-cb9aa3ce7fc6"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9220), true },
                    { new Guid("0e59639f-c759-450e-9092-88d0530d6cd9"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9167), true },
                    { new Guid("3c0d6b7f-f303-416c-ad14-dd88035db7ff"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9169), true },
                    { new Guid("4d67791a-14aa-4590-910d-88593fe908f4"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9172), true },
                    { new Guid("87c0e273-796d-4c52-a27d-a803883a8c79"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9170), true },
                    { new Guid("a845595e-b1b8-4f35-9d6e-777e6000aee0"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9171), true },
                    { new Guid("bdb6b201-eccd-4456-a431-99c18710e693"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9171), true },
                    { new Guid("c19fc3bf-6204-492a-bf61-5434d627d3f2"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9170), true },
                    { new Guid("e2861b35-bb82-4896-b7ce-9cad6bb42b6b"), new Guid("96c0426c-7e36-4689-bf65-2c51a9bf2314"), new DateTime(2025, 9, 9, 14, 54, 58, 629, DateTimeKind.Utc).AddTicks(9170), true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_DeviceId",
                table: "RefreshTokens",
                columns: new[] { "UserId", "DeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
