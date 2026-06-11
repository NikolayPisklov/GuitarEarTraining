using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GuitarTrainer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exercises_types_type_id",
                table: "exercises");

            migrationBuilder.DropTable(
                name: "types");

            migrationBuilder.DropIndex(
                name: "ix_exercises_type_id",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "type_id",
                table: "exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "type_id",
                table: "exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exercises_type_id",
                table: "exercises",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_types_title",
                table: "types",
                column: "title",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_exercises_types_type_id",
                table: "exercises",
                column: "type_id",
                principalTable: "types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
