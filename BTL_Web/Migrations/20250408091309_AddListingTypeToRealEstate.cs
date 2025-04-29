using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddListingTypeToRealEstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListingType",
                table: "RealEstates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingType",
                table: "RealEstates");
        }
    }
}
