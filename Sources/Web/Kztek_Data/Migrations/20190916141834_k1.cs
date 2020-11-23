using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kztek_Data.Migrations
{
    public partial class k1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SY_Map_Role_Menu",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    MenuId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SY_Map_Role_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SY_Map_User_Role",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SY_Map_User_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SY_MenuFunction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    MenuName = table.Column<string>(nullable: false),
                    ControllerName = table.Column<string>(nullable: true),
                    ActionName = table.Column<string>(nullable: true),
                    MenuType = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    Active = table.Column<short>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    Dept = table.Column<int>(nullable: true),
                    Breadcrumb = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SY_MenuFunction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SY_Role",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RoleName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Active = table.Column<short>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SY_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SY_User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PasswordSalat = table.Column<string>(nullable: true),
                    isAdmin = table.Column<short>(type: "bit", nullable: false),
                    Active = table.Column<short>(type: "bit", nullable: false),
                    Avatar = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SY_User", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SY_Map_Role_Menu");

            migrationBuilder.DropTable(
                name: "SY_Map_User_Role");

            migrationBuilder.DropTable(
                name: "SY_MenuFunction");

            migrationBuilder.DropTable(
                name: "SY_Role");

            migrationBuilder.DropTable(
                name: "SY_User");
        }
    }
}
