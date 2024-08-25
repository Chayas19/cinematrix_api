using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMatrix_API.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureReviewRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "Actors",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Actors");
        }
    }
}
