using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyBall.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIsBudgetExempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBudgetExempt",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBudgetExempt",
                table: "Categories");
        }
    }
}
