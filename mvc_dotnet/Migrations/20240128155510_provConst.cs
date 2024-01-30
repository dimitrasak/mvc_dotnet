using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc_dotnet.Migrations
{
    public partial class provConst : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cinemas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    SEATS = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    _3D = table.Column<string>(name: "3D", type: "varchar(45)", unicode: false, maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cinemas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    username = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true),
                    password = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    salt = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    role = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.username);
                });

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    user_username = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.ID);
                    table.ForeignKey(
                        name: "FK_admins_user",
                        column: x => x.user_username,
                        principalTable: "user",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "content_admin",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    user_username = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_admin", x => x.ID);
                    table.ForeignKey(
                        name: "FK_content_admin_user",
                        column: x => x.user_username,
                        principalTable: "user",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    user_username = table.Column<string>(type: "varchar(32)", unicode: false, maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Table_1_user",
                        column: x => x.user_username,
                        principalTable: "user",
                        principalColumn: "username");
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    CONTENT = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    LENGTH = table.Column<int>(type: "int", nullable: true),
                    TYPE = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    SUMMARY = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    DIRECTOR = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    CONTENT_ADMIN_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => new { x.ID, x.NAME });
                    table.ForeignKey(
                        name: "FK_movies_content_admin",
                        column: x => x.CONTENT_ADMIN_ID,
                        principalTable: "content_admin",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "provoles",
                columns: table => new
                {
                    CINEMAS_ID = table.Column<int>(type: "int", nullable: false),
                    MOVIES_ID = table.Column<int>(type: "int", nullable: false),
                    MOVIES_NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    ID = table.Column<int>(type: "int", nullable: false),
                    CONTENT_ADMIN_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provoles", x => new { x.CINEMAS_ID, x.MOVIES_ID, x.MOVIES_NAME });
                    table.ForeignKey(
                        name: "FK_prov_cin",
                        column: x => x.CINEMAS_ID,
                        principalTable: "cinemas",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_prov_content_admin",
                        column: x => x.CONTENT_ADMIN_ID,
                        principalTable: "content_admin",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_prov_movies",
                        columns: x => new { x.MOVIES_ID, x.MOVIES_NAME },
                        principalTable: "movies",
                        principalColumns: new[] { "ID", "NAME" });
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    NUMBER_OF_SEATS = table.Column<int>(type: "int", nullable: true),
                    PROVOLES_CINEMAS_ID = table.Column<int>(type: "int", nullable: true),
                    PROVOLES_MOVIES_ID = table.Column<int>(type: "int", nullable: true),
                    PROVOLES_MOVIES_NAME = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    CUSTOMERS_ID = table.Column<int>(type: "int", nullable: true),
                    PROVOLES_CONTENT_ADMIN_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_reservations_provoles",
                        columns: x => new { x.PROVOLES_CINEMAS_ID, x.PROVOLES_MOVIES_ID, x.PROVOLES_MOVIES_NAME },
                        principalTable: "provoles",
                        principalColumns: new[] { "CINEMAS_ID", "MOVIES_ID", "MOVIES_NAME" });
                    table.ForeignKey(
                        name: "FK_resrv_custom",
                        column: x => x.CUSTOMERS_ID,
                        principalTable: "customers",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_admins_user_username",
                table: "admins",
                column: "user_username");

            migrationBuilder.CreateIndex(
                name: "IX_content_admin_user_username",
                table: "content_admin",
                column: "user_username");

            migrationBuilder.CreateIndex(
                name: "IX_customers_user_username",
                table: "customers",
                column: "user_username");

            migrationBuilder.CreateIndex(
                name: "IX_movies_CONTENT_ADMIN_ID",
                table: "movies",
                column: "CONTENT_ADMIN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_provoles_CONTENT_ADMIN_ID",
                table: "provoles",
                column: "CONTENT_ADMIN_ID");

            migrationBuilder.CreateIndex(
                name: "IX_provoles_MOVIES_ID_MOVIES_NAME",
                table: "provoles",
                columns: new[] { "MOVIES_ID", "MOVIES_NAME" });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_CUSTOMERS_ID",
                table: "reservations",
                column: "CUSTOMERS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_PROVOLES_CINEMAS_ID_PROVOLES_MOVIES_ID_PROVOLES_MOVIES_NAME",
                table: "reservations",
                columns: new[] { "PROVOLES_CINEMAS_ID", "PROVOLES_MOVIES_ID", "PROVOLES_MOVIES_NAME" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "provoles");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "cinemas");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "content_admin");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
