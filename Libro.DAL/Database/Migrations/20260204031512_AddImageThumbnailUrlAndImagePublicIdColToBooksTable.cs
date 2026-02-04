using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.DAL.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddImageThumbnailUrlAndImagePublicIdColToBooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageThumbnailUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImageThumbnailUrl",
                table: "Books");
        }
    }
}
