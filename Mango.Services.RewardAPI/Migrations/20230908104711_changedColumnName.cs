using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.RewardAPI.Migrations
{
    /// <inheritdoc />
    public partial class changedColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RewardsActivity",
                table: "Rewards",
                newName: "RewardPoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RewardPoints",
                table: "Rewards",
                newName: "RewardsActivity");
        }
    }
}
