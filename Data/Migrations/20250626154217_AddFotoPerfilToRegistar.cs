using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoPack_project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFotoPerfilToRegistar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPerfil",
                table: "Registars",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Registars");
        }
    }
}
