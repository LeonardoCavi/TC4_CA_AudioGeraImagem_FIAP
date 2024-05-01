using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGeraImagemWorker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comandos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UrlAudio = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    Transcricao = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    UrlImagem = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    InstanteAtualizacao = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessamentoComandos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    MensagemErro = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    ComandoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessamentoComandos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessamentoComandos_Comandos_ComandoId",
                        column: x => x.ComandoId,
                        principalTable: "Comandos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentoComandos_ComandoId",
                table: "ProcessamentoComandos",
                column: "ComandoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessamentoComandos");

            migrationBuilder.DropTable(
                name: "Comandos");
        }
    }
}
