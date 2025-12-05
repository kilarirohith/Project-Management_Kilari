using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalDesks_Projects_ProjectId",
                table: "ApprovalDesks");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalDesks_ProjectId",
                table: "ApprovalDesks");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "VendorWorks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ApprovalDesks");

            migrationBuilder.RenameColumn(
                name: "WorkDescription",
                table: "VendorWorks",
                newName: "ReportFilePath");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "VendorWorks",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "VendorWorks",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "VendorWorks",
                newName: "Category");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "VendorWorks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Vendors",
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

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Users_UserId",
                table: "Vendors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Users_UserId",
                table: "Vendors");

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

            migrationBuilder.DropIndex(
                name: "IX_Vendors_UserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "ReportFilePath",
                table: "VendorWorks",
                newName: "WorkDescription");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "VendorWorks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "VendorWorks",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "VendorWorks",
                newName: "ProjectName");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "VendorWorks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ApprovalDesks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDesks_ProjectId",
                table: "ApprovalDesks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalDesks_Projects_ProjectId",
                table: "ApprovalDesks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
