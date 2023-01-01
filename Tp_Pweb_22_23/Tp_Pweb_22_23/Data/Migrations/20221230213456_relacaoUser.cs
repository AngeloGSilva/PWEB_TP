using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Data.Migrations
{
    public partial class relacaoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_AspNetUsers_ApplicationUserId",
                table: "Reserva");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Reserva",
                newName: "ClienteId1");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_ApplicationUserId",
                table: "Reserva",
                newName: "IX_Reserva_ClienteId1");

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Reserva",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuncionarioId",
                table: "EstadoVeiculo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuncionarioId1",
                table: "EstadoVeiculo",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Empresa",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadoVeiculo_FuncionarioId1",
                table: "EstadoVeiculo",
                column: "FuncionarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId1",
                table: "EstadoVeiculo",
                column: "FuncionarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId1",
                table: "Reserva",
                column: "ClienteId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Empresa_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EstadoVeiculo_AspNetUsers_FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId1",
                table: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_EstadoVeiculo_FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "FuncionarioId",
                table: "EstadoVeiculo");

            migrationBuilder.DropColumn(
                name: "FuncionarioId1",
                table: "EstadoVeiculo");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Empresa");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ClienteId1",
                table: "Reserva",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_ClienteId1",
                table: "Reserva",
                newName: "IX_Reserva_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_AspNetUsers_ApplicationUserId",
                table: "Reserva",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
