using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClientFromProjectMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMasters_Clients_ClientId",
                table: "ProjectMasters");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMasters_ClientId",
                table: "ProjectMasters");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ProjectMasters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "ProjectMasters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMasters_ClientId",
                table: "ProjectMasters",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMasters_Clients_ClientId",
                table: "ProjectMasters",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
