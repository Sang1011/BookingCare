using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBookingAndMedicalRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_email");

            migrationBuilder.RenameColumn(
                name: "Prescription",
                table: "MedicalRecords",
                newName: "Treatment");

            migrationBuilder.RenameColumn(
                name: "WorkDate",
                table: "DoctorSchedules",
                newName: "work_date");

            migrationBuilder.RenameColumn(
                name: "SlotStart",
                table: "DoctorSchedules",
                newName: "slot_start");

            migrationBuilder.RenameColumn(
                name: "SlotEnd",
                table: "DoctorSchedules",
                newName: "slot_end");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FollowUpDate",
                table: "MedicalRecords",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoctorRescheduleCount",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RescheduleCount",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MedicalRecordAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecordAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecordAttachments_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    MedicineName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Frequency = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Instructions = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionItems_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_VisitDate",
                table: "MedicalRecords",
                column: "VisitDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Status",
                table: "Bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordAttachments_MedicalRecordId",
                table: "MedicalRecordAttachments",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_MedicalRecordId",
                table: "PrescriptionItems",
                column: "MedicalRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecordAttachments");

            migrationBuilder.DropTable(
                name: "PrescriptionItems");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_VisitDate",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Status",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DoctorRescheduleCount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RescheduleCount",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "Treatment",
                table: "MedicalRecords",
                newName: "Prescription");

            migrationBuilder.RenameColumn(
                name: "work_date",
                table: "DoctorSchedules",
                newName: "WorkDate");

            migrationBuilder.RenameColumn(
                name: "slot_start",
                table: "DoctorSchedules",
                newName: "SlotStart");

            migrationBuilder.RenameColumn(
                name: "slot_end",
                table: "DoctorSchedules",
                newName: "SlotEnd");
        }
    }
}
