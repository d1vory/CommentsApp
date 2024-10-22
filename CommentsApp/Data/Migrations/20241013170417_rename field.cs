using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentsApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class renamefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Comments",
                newName: "File");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "File",
                table: "Comments",
                newName: "Photo");
        }
    }
}
