using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGeraImagemWorker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Adicionadescricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Comandos",
                type: "VARCHAR(256)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Comandos");
        }
    }
}
