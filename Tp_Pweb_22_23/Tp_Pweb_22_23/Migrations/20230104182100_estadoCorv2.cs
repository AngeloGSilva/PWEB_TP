using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class estadoCorv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Entrega",
                table: "EstadoVeiculo");

            migrationBuilder.DropColumn(
                name: "Leventamento",
                table: "EstadoVeiculo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Entrega",
                table: "EstadoVeiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Leventamento",
                table: "EstadoVeiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
