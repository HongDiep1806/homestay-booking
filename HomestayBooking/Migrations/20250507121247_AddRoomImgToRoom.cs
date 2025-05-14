using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomestayBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomImgToRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomImg",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomImg",
                table: "Rooms");
        }

    }
}
