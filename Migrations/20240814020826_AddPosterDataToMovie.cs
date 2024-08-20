using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMatrix_API.Migrations
{
    /// <inheritdoc />
    public partial class AddPosterDataToMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoviePosterId",
                table: "WatchHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoviePosterId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoviePosterId",
                table: "MoviesLanguages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PosterData",
                table: "Movies",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "MoviePosterId",
                table: "MovieGenres",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoviePosterId",
                table: "MovieActors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MoviePosters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PosterData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubscriptionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePosters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchHistories_MoviePosterId",
                table: "WatchHistories",
                column: "MoviePosterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MoviePosterId",
                table: "Reviews",
                column: "MoviePosterId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesLanguages_MoviePosterId",
                table: "MoviesLanguages",
                column: "MoviePosterId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MoviePosterId",
                table: "MovieGenres",
                column: "MoviePosterId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_MoviePosterId",
                table: "MovieActors",
                column: "MoviePosterId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieActors_MoviePosters_MoviePosterId",
                table: "MovieActors",
                column: "MoviePosterId",
                principalTable: "MoviePosters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieGenres_MoviePosters_MoviePosterId",
                table: "MovieGenres",
                column: "MoviePosterId",
                principalTable: "MoviePosters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviesLanguages_MoviePosters_MoviePosterId",
                table: "MoviesLanguages",
                column: "MoviePosterId",
                principalTable: "MoviePosters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_MoviePosters_MoviePosterId",
                table: "Reviews",
                column: "MoviePosterId",
                principalTable: "MoviePosters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchHistories_MoviePosters_MoviePosterId",
                table: "WatchHistories",
                column: "MoviePosterId",
                principalTable: "MoviePosters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieActors_MoviePosters_MoviePosterId",
                table: "MovieActors");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieGenres_MoviePosters_MoviePosterId",
                table: "MovieGenres");

            migrationBuilder.DropForeignKey(
                name: "FK_MoviesLanguages_MoviePosters_MoviePosterId",
                table: "MoviesLanguages");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_MoviePosters_MoviePosterId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_WatchHistories_MoviePosters_MoviePosterId",
                table: "WatchHistories");

            migrationBuilder.DropTable(
                name: "MoviePosters");

            migrationBuilder.DropIndex(
                name: "IX_WatchHistories_MoviePosterId",
                table: "WatchHistories");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MoviePosterId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_MoviesLanguages_MoviePosterId",
                table: "MoviesLanguages");

            migrationBuilder.DropIndex(
                name: "IX_MovieGenres_MoviePosterId",
                table: "MovieGenres");

            migrationBuilder.DropIndex(
                name: "IX_MovieActors_MoviePosterId",
                table: "MovieActors");

            migrationBuilder.DropColumn(
                name: "MoviePosterId",
                table: "WatchHistories");

            migrationBuilder.DropColumn(
                name: "MoviePosterId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "MoviePosterId",
                table: "MoviesLanguages");

            migrationBuilder.DropColumn(
                name: "PosterData",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MoviePosterId",
                table: "MovieGenres");

            migrationBuilder.DropColumn(
                name: "MoviePosterId",
                table: "MovieActors");
        }
    }
}
