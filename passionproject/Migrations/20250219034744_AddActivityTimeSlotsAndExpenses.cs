using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passionproject.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityTimeSlotsAndExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityTimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActivityId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityTimeSlots_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityTimeSlots_Activities_ActivityId1",
                        column: x => x.ActivityId1,
                        principalTable: "Activities",
                        principalColumn: "ActivityId");
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActivityId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "ActivityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Activities_ActivityId1",
                        column: x => x.ActivityId1,
                        principalTable: "Activities",
                        principalColumn: "ActivityId");
                });

            migrationBuilder.CreateTable(
                name: "ExpenseSplits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExpenseId = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    Share = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    MemberId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseSplits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseSplits_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSplits_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseSplits_Members_MemberId1",
                        column: x => x.MemberId1,
                        principalTable: "Members",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTimeSlots_ActivityId",
                table: "ActivityTimeSlots",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTimeSlots_ActivityId1",
                table: "ActivityTimeSlots",
                column: "ActivityId1");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ActivityId",
                table: "Expenses",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ActivityId1",
                table: "Expenses",
                column: "ActivityId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSplits_ExpenseId",
                table: "ExpenseSplits",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSplits_MemberId",
                table: "ExpenseSplits",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSplits_MemberId1",
                table: "ExpenseSplits",
                column: "MemberId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityTimeSlots");

            migrationBuilder.DropTable(
                name: "ExpenseSplits");

            migrationBuilder.DropTable(
                name: "Expenses");
        }
    }
}
