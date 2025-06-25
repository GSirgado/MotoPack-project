using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoPack_project.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAdminToRegistar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Registars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Registars");
        }
    }
}
