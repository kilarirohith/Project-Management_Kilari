using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVendorWorkFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorWorks_ProjectMasters_ProjectId",
                table: "VendorWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorWorks_Users_CoordinatorUserId",
                table: "VendorWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorWorks_Users_ResearchPersonUserId",
                table: "VendorWorks");

            migrationBuilder.DropIndex(
                name: "IX_VendorWorks_CoordinatorUserId",
                table: "VendorWorks");

            migrationBuilder.DropIndex(
                name: "IX_VendorWorks_ProjectId",
                table: "VendorWorks");

            migrationBuilder.DropIndex(
                name: "IX_VendorWorks_ResearchPersonUserId",
                table: "VendorWorks");

            migrationBuilder.DropColumn(
                name: "CoordinatorUserId",
                table: "VendorWorks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "VendorWorks");

            migrationBuilder.DropColumn(
                name: "ResearchPersonUserId",
                table: "VendorWorks");

            migrationBuilder.AddColumn<string>(
                name: "CoordinatorName",
                table: "VendorWorks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "VendorWorks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoordinatorName",
                table: "VendorWorks");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "VendorWorks");

            migrationBuilder.AddColumn<int>(
                name: "CoordinatorUserId",
                table: "VendorWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "VendorWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResearchPersonUserId",
                table: "VendorWorks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VendorWorks_CoordinatorUserId",
                table: "VendorWorks",
                column: "CoordinatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorWorks_ProjectId",
                table: "VendorWorks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorWorks_ResearchPersonUserId",
                table: "VendorWorks",
                column: "ResearchPersonUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorWorks_ProjectMasters_ProjectId",
                table: "VendorWorks",
                column: "ProjectId",
                principalTable: "ProjectMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorWorks_Users_CoordinatorUserId",
                table: "VendorWorks",
                column: "CoordinatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorWorks_Users_ResearchPersonUserId",
                table: "VendorWorks",
                column: "ResearchPersonUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
