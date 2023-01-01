using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class modelCor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId1",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Empresa_EmpresaId",
                table: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_Reserva_ClienteId1",
                table: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_Reserva_EmpresaId",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "ClienteId1",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Reserva");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Veiculo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idCategoria",
                table: "Veiculo",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClienteId",
                table: "Reserva",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Veiculo_CategoriaId",
                table: "Veiculo",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_ClienteId",
                table: "Reserva",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId",
                table: "Reserva",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculo_Categoria_CategoriaId",
                table: "Veiculo",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculo_Categoria_CategoriaId",
                table: "Veiculo");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropIndex(
                name: "IX_Veiculo_CategoriaId",
                table: "Veiculo");

            migrationBuilder.DropIndex(
                name: "IX_Reserva_ClienteId",
                table: "Reserva");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "idCategoria",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Reserva",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId1",
                table: "Reserva",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "Reserva",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_ClienteId1",
                table: "Reserva",
                column: "ClienteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_EmpresaId",
                table: "Reserva",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_AspNetUsers_ClienteId1",
                table: "Reserva",
                column: "ClienteId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Empresa_EmpresaId",
                table: "Reserva",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id");
        }
    }
}
