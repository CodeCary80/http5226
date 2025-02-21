using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passionproject.Migrations
{
    /// <inheritdoc />
    public partial class FixActivityTimeSlotRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityTimeSlots_Activities_ActivityId1",
                table: "ActivityTimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_ActivityTimeSlots_ActivityId1",
                table: "ActivityTimeSlots");

            migrationBuilder.DropColumn(
                name: "ActivityId1",
                table: "ActivityTimeSlots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityId1",
                table: "ActivityTimeSlots",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTimeSlots_ActivityId1",
                table: "ActivityTimeSlots",
                column: "ActivityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityTimeSlots_Activities_ActivityId1",
                table: "ActivityTimeSlots",
                column: "ActivityId1",
                principalTable: "Activities",
                principalColumn: "ActivityId");
        }
    }
}
