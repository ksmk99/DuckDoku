using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DuckDoku.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceIdUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceId",
                table: "Devices",
                column: "DeviceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceId",
                table: "Devices");
        }
    }
}
