using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoPack_project.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaRegistar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfPass",
                table: "Registars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfPass",
                table: "Registars",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
