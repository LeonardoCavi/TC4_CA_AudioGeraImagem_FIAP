using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioGeraImagemWorker.Infra.Migrations
{
    /// <inheritdoc />
    public partial class novo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Criacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Descricao = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    UrlAudio = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    Transcricao = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    UrlImagem = table.Column<string>(type: "VARCHAR(MAX)", nullable: true),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    InstanteAtualizacao = table.Column<DateTime>(type: "DATETIME2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessamentosCriacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    InstanteCriacao = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    MensagemErro = table.Column<string>(type: "VARCHAR(256)", nullable: true),
                    CriacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessamentosCriacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessamentosCriacao_Criacao_CriacaoId",
                        column: x => x.CriacaoId,
                        principalTable: "Criacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessamentosCriacao_CriacaoId",
                table: "ProcessamentosCriacao",
                column: "CriacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessamentosCriacao");

            migrationBuilder.DropTable(
                name: "Criacao");
        }
    }
}
