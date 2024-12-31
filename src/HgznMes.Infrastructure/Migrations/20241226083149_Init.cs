using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HgznMes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "building",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false, comment: "建筑物名称"),
                    code = table.Column<string>(type: "varchar(50)", nullable: true, comment: "建筑物编号"),
                    address = table.Column<string>(type: "varchar(255)", nullable: true, comment: "地址"),
                    latitude = table.Column<double>(type: "double precision", nullable: true, comment: "纬度"),
                    longitude = table.Column<double>(type: "double precision", nullable: true, comment: "经度"),
                    construction_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, comment: "建造日期"),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_num = table.Column<int>(type: "integer", nullable: false),
                    soft_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    delete_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_building", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "menu",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    order_num = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "text", nullable: false),
                    icon_url = table.Column<string>(type: "text", nullable: true),
                    is_link = table.Column<bool>(type: "boolean", nullable: false),
                    route = table.Column<string>(type: "text", nullable: true),
                    scope_code = table.Column<string>(type: "text", nullable: true),
                    visible = table.Column<bool>(type: "boolean", nullable: false),
                    favorite = table.Column<bool>(type: "boolean", nullable: false),
                    state = table.Column<bool>(type: "boolean", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    soft_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    delete_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_menu", x => x.id);
                    table.ForeignKey(
                        name: "fk_menu_menu_parent_id",
                        column: x => x.parent_id,
                        principalTable: "menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    role_code = table.Column<string>(type: "text", nullable: false),
                    order_num = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<bool>(type: "boolean", nullable: false),
                    soft_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    delete_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    passphrase = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    salt = table.Column<string>(type: "text", nullable: false),
                    nick = table.Column<string>(type: "text", nullable: true),
                    icon = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    register_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    state = table.Column<bool>(type: "boolean", nullable: false),
                    soft_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    delete_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "floor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true, comment: "楼层名称"),
                    code = table.Column<string>(type: "text", nullable: true, comment: "楼层编号"),
                    area = table.Column<double>(type: "double precision", nullable: true, comment: "楼层面积（平方米）"),
                    number_of_rooms = table.Column<int>(type: "integer", nullable: true, comment: "房间数量"),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_num = table.Column<int>(type: "integer", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_floor", x => x.id);
                    table.ForeignKey(
                        name: "fk_floor_building_building_id",
                        column: x => x.building_id,
                        principalTable: "building",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_floor_building_parent_id",
                        column: x => x.parent_id,
                        principalTable: "building",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_menu",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    menu_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_menu", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_menu_menu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_menu_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detail",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    about_me = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detail", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_detail_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "setting",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_setting", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_setting_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_role_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true, comment: "房间名称"),
                    code = table.Column<string>(type: "text", nullable: true, comment: "房间编号"),
                    length = table.Column<double>(type: "double precision", nullable: false, comment: "房间长度（米）"),
                    width = table.Column<double>(type: "double precision", nullable: false, comment: "房间宽度（米）"),
                    height = table.Column<double>(type: "double precision", nullable: false, comment: "房间高度（米）"),
                    purpose = table.Column<int>(type: "integer", nullable: false, comment: "房间用途"),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "属于同一个房间组"),
                    creation_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_modification_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modifier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_num = table.Column<int>(type: "integer", nullable: false),
                    floor_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_floor_floor_id",
                        column: x => x.floor_id,
                        principalTable: "floor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_room_floor_parent_id",
                        column: x => x.parent_id,
                        principalTable: "floor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "menu",
                columns: new[] { "id", "code", "creation_time", "creator_id", "delete_time", "description", "favorite", "icon_url", "is_link", "last_modification_time", "last_modifier_id", "level", "name", "order_num", "parent_id", "path", "route", "scope_code", "soft_deleted", "state", "type", "visible" },
                values: new object[] { new Guid("e9df3280-8ab1-4b45-8d6a-6c3e669317ac"), "root", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "root menu", false, null, false, null, null, 0, "Root", -1, null, "root", null, null, false, false, 1, true });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "creation_time", "creator_id", "delete_time", "description", "last_modification_time", "last_modifier_id", "name", "order_num", "role_code", "soft_deleted", "state" },
                values: new object[,]
                {
                    { new Guid("4a15f57a-0cb7-4cc9-95c0-91ba672a341c"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "normal user with some basic resources", null, null, "member", 0, "member", false, true },
                    { new Guid("4fe6ebb8-5001-40b4-a59e-d193ad9186f8"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "super user with all catchable resources", null, null, "super", 0, "super", false, true },
                    { new Guid("e1f23f37-919c-453b-aff1-1214415e54b8"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "admin to manage user resourcs", null, null, "administrator", 0, "admin", false, true },
                    { new Guid("e8df3280-8ab1-4b45-8d6a-6c3e669317ac"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "developer with all cathable resources even it was obselete", null, null, "developer", 0, "dev", false, true }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "birth_date", "creation_time", "creator_id", "delete_time", "email", "icon", "last_modification_time", "last_modifier_id", "name", "nick", "passphrase", "phone", "register_time", "salt", "soft_deleted", "state", "username" },
                values: new object[,]
                {
                    { new Guid("181256b6-4c8c-4fca-8b5d-5150f831c3f3"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "unknow", null, null, null, null, "initial-developer", "ZLdMAg0N2xN8NbXr5wsoevc/bBay/lJT4sLFbUClwTI=", "unknow", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ue9OQmiW1aH5gzkFKXEB84ToTcHjuroMdzDxymov0CA=", false, false, "developer" },
                    { new Guid("7b0c873e-8437-4a8c-8712-bb81c2c796d5"), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "unknow", null, null, null, null, "initial-admin", "qLGu+48XZDn5UC5TmgIgwb+29lIXYVA1i1vjPAjSY1A=", "unknow", new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hxF4RZh/IdmJmTuzjBChb1d5vdotQmESgTkxJ1Yede0=", false, false, "admin" }
                });

            migrationBuilder.InsertData(
                table: "role_menu",
                columns: new[] { "id", "menu_id", "role_id" },
                values: new object[,]
                {
                    { 1L, new Guid("e9df3280-8ab1-4b45-8d6a-6c3e669317ac"), new Guid("e8df3280-8ab1-4b45-8d6a-6c3e669317ac") },
                    { 2L, new Guid("e9df3280-8ab1-4b45-8d6a-6c3e669317ac"), new Guid("4fe6ebb8-5001-40b4-a59e-d193ad9186f8") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_floor_building_id",
                table: "floor",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_floor_parent_id",
                table: "floor",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_menu_code",
                table: "menu",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_menu_order_num",
                table: "menu",
                column: "order_num");

            migrationBuilder.CreateIndex(
                name: "ix_menu_parent_id",
                table: "menu",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_name_soft_deleted",
                table: "role",
                columns: new[] { "name", "soft_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_soft_deleted",
                table: "role",
                column: "soft_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_role_menu_menu_id",
                table: "role_menu",
                column: "menu_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_menu_role_id",
                table: "role_menu",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_floor_id",
                table: "room",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_parent_id",
                table: "room",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_soft_deleted",
                table: "user",
                column: "soft_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_user_username_soft_deleted",
                table: "user",
                columns: new[] { "username", "soft_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_role_id",
                table: "user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                table: "user_role",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detail");

            migrationBuilder.DropTable(
                name: "role_menu");

            migrationBuilder.DropTable(
                name: "room");

            migrationBuilder.DropTable(
                name: "setting");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "menu");

            migrationBuilder.DropTable(
                name: "floor");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "building");
        }
    }
}
