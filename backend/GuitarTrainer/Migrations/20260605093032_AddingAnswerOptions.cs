using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GuitarTrainer.Migrations
{
    /// <inheritdoc />
    public partial class AddingAnswerOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswerId",
                table: "Samples",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_CorrectAnswerId",
                table: "Samples",
                column: "CorrectAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_ExerciseId_Code",
                table: "AnswerOptions",
                columns: new[] { "ExerciseId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Samples_AnswerOptions_CorrectAnswerId",
                table: "Samples",
                column: "CorrectAnswerId",
                principalTable: "AnswerOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Samples_AnswerOptions_CorrectAnswerId",
                table: "Samples");

            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropIndex(
                name: "IX_Samples_CorrectAnswerId",
                table: "Samples");

            migrationBuilder.DropColumn(
                name: "CorrectAnswerId",
                table: "Samples");
        }
    }
}
