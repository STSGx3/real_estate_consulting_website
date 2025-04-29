using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddBedroomAndBathroomToRealEstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BathroomCount",
                table: "RealEstates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BedroomCount",
                table: "RealEstates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BathroomCount",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "BedroomCount",
                table: "RealEstates");
        }
    }
}
