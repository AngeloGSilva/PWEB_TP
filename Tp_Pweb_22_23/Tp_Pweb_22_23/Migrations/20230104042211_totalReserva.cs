using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class totalReserva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Reserva",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "Reserva");
        }
    }
}
