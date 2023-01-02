using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class VeiculoCor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condicao",
                table: "Veiculo");

            migrationBuilder.DropColumn(
                name: "Danos",
                table: "Veiculo");

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "Veiculo");

            migrationBuilder.AddColumn<string>(
                name: "Condicao",
                table: "Veiculo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Danos",
                table: "Veiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
