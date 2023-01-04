using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class estadoCor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.DropIndex(
                name: "IX_EstadoVeiculo_FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.DropColumn(
                name: "FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.AlterColumn<string>(
                name: "FuncionarioId",
                table: "EstadoVeiculo",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadoVeiculo_FuncionarioId",
                table: "EstadoVeiculo",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId",
                table: "EstadoVeiculo",
                column: "FuncionarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId",
                table: "EstadoVeiculo");

            migrationBuilder.DropIndex(
                name: "IX_EstadoVeiculo_FuncionarioId",
                table: "EstadoVeiculo");

            migrationBuilder.AlterColumn<int>(
                name: "FuncionarioId",
                table: "EstadoVeiculo",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuncionarioId1",
                table: "EstadoVeiculo",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadoVeiculo_FuncionarioId1",
                table: "EstadoVeiculo",
                column: "FuncionarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId1",
                table: "EstadoVeiculo",
                column: "FuncionarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
