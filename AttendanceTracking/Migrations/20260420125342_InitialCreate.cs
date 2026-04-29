using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceTracking.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentID_CourseID_Date",
                table: "Attendances",
                columns: new[] { "StudentID", "CourseID", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentID_CourseID_Date",
                table: "Attendances");
        }
    }
}
