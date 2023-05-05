using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrasructure.Migrations;

/// <inheritdoc />
public partial class AddMyEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Courses",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CREATE_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                UPDATE_AT = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Courses", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Groups",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                COURSE_ID = table.Column<int>(type: "int", nullable: false),
                NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CREATE_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                UPDATE_AT = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Groups", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Students",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FIRST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LAST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                GROUP_ID = table.Column<int>(type: "int", nullable: true),
                CREATE_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                UPDATE_AT = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Students", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Courses");

        migrationBuilder.DropTable(
            name: "Groups");

        migrationBuilder.DropTable(
            name: "Students");
    }
}
