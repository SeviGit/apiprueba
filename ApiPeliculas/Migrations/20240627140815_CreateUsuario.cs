using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peliculas_Categorias_CategoriaId",
                table: "Peliculas");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Peliculas",
                newName: "categoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Peliculas_CategoriaId",
                table: "Peliculas",
                newName: "IX_Peliculas_categoriaId");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Peliculas_Categorias_categoriaId",
                table: "Peliculas",
                column: "categoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peliculas_Categorias_categoriaId",
                table: "Peliculas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "categoriaId",
                table: "Peliculas",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Peliculas_categoriaId",
                table: "Peliculas",
                newName: "IX_Peliculas_CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Peliculas_Categorias_CategoriaId",
                table: "Peliculas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
