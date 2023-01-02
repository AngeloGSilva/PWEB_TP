using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tp_Pweb_22_23.Migrations
{
    public partial class fotoCarro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "Veiculo",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Veiculo");
        }
    }
}
