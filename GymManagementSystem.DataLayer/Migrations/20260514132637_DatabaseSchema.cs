using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class _20260514132637_DatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Gym");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Photo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingNumber = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    City = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                    table.CheckConstraint("PlanDurationCheck", "DurationDays BETWEEN 1 AND 365");
                });

            migrationBuilder.CreateTable(
                name: "Trainers",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Specialty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "varchar(11)", maxLength: 11, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingNumber = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    City = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HealthRecords",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    BloodType = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthRecords_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Gym",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.CheckConstraint("MembershipDatesCheck", "EndDate > StartDate");
                    table.ForeignKey(
                        name: "FK_Memberships_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Gym",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Memberships_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "Gym",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.CheckConstraint("SessionCapacityCheck", "Capacity > 0");
                    table.CheckConstraint("SessionDatesCheck", "EndDate > StartDate");
                    table.ForeignKey(
                        name: "FK_Sessions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Gym",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalSchema: "Gym",
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "Gym",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, defaultValue: "System"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Members_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Gym",
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "Gym",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingDate",
                schema: "Gym",
                table: "Bookings",
                column: "BookingDate");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MemberId",
                schema: "Gym",
                table: "Bookings",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SessionId",
                schema: "Gym",
                table: "Bookings",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_MemberId",
                schema: "Gym",
                table: "HealthRecords",
                column: "MemberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                schema: "Gym",
                table: "Members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_JoinDate",
                schema: "Gym",
                table: "Members",
                column: "JoinDate");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Phone",
                schema: "Gym",
                table: "Members",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_EndDate",
                schema: "Gym",
                table: "Memberships",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_MemberId",
                schema: "Gym",
                table: "Memberships",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_PlanId",
                schema: "Gym",
                table: "Memberships",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_StartDate",
                schema: "Gym",
                table: "Memberships",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CategoryId",
                schema: "Gym",
                table: "Sessions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_EndDate",
                schema: "Gym",
                table: "Sessions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_StartDate",
                schema: "Gym",
                table: "Sessions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TrainerId",
                schema: "Gym",
                table: "Sessions",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Email",
                schema: "Gym",
                table: "Trainers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_HireDate",
                schema: "Gym",
                table: "Trainers",
                column: "HireDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_Phone",
                schema: "Gym",
                table: "Trainers",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "HealthRecords",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Memberships",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Gym");

            migrationBuilder.DropTable(
                name: "Trainers",
                schema: "Gym");
        }
    }
}
