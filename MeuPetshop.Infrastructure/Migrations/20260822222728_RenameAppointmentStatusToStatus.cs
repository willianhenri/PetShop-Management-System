using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuPetshop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameAppointmentStatusToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppointmentStatus",
                table: "Appointments",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Appointments",
                newName: "AppointmentStatus");
        }
    }
}
