using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoDengueReportAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacoesToRelatorios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Cpf = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataSolicitacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Arbovirose = table.Column<string>(type: "TEXT", nullable: false),
                    SemanaInicio = table.Column<int>(type: "INTEGER", nullable: false),
                    SemanaFim = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoIBGE = table.Column<string>(type: "TEXT", nullable: false),
                    Municipio = table.Column<string>(type: "TEXT", nullable: false),
                    SolicitanteId = table.Column<int>(type: "INTEGER", nullable: false),
                    quantidadeDeCasosEstimados = table.Column<double>(type: "REAL", nullable: false),
                    notificacoes = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relatorios_Solicitantes_SolicitanteId",
                        column: x => x.SolicitanteId,
                        principalTable: "Solicitantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_SolicitanteId",
                table: "Relatorios",
                column: "SolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitantes_Cpf",
                table: "Solicitantes",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "Solicitantes");
        }
    }
}
