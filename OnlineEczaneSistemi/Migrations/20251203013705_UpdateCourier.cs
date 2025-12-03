using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEczaneSistemi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "CourierRegistrationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "CourierRegistrationRequests");
        }
    }
}
