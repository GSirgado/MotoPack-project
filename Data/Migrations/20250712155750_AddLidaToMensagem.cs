﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoPack_project.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLidaToMensagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Lida",
                table: "Mensagens",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lida",
                table: "Mensagens");
        }
    }
}
