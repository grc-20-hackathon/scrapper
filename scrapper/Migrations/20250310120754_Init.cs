using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace scrapper.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobOpening",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    jobDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOpening", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projectView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProjectSourse = table.Column<string>(type: "text", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: true),
                    ProjectDescription = table.Column<string>(type: "text", nullable: true),
                    ProjectWebsite = table.Column<string>(type: "text", nullable: true),
                    ProjectX = table.Column<string>(type: "text", nullable: true),
                    ProjectAvatar = table.Column<string>(type: "text", nullable: true),
                    ProjectCover = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projectView", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpenJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    jobSourse = table.Column<string>(type: "text", nullable: true),
                    jobOpeningId = table.Column<int>(type: "integer", nullable: true),
                    roleId = table.Column<int>(type: "integer", nullable: true),
                    salary = table.Column<string>(type: "text", nullable: true),
                    publishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    location = table.Column<string>(type: "text", nullable: true),
                    responsibilities = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    ProjectViewId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenJobs_JobOpening_jobOpeningId",
                        column: x => x.jobOpeningId,
                        principalTable: "JobOpening",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenJobs_Role_roleId",
                        column: x => x.roleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpenJobs_projectView_ProjectViewId",
                        column: x => x.ProjectViewId,
                        principalTable: "projectView",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    OpenJobsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skill_OpenJobs_OpenJobsId",
                        column: x => x.OpenJobsId,
                        principalTable: "OpenJobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFormat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    OpenJobsId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFormat_OpenJobs_OpenJobsId",
                        column: x => x.OpenJobsId,
                        principalTable: "OpenJobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenJobs_jobOpeningId",
                table: "OpenJobs",
                column: "jobOpeningId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenJobs_ProjectViewId",
                table: "OpenJobs",
                column: "ProjectViewId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenJobs_roleId",
                table: "OpenJobs",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_OpenJobsId",
                table: "Skill",
                column: "OpenJobsId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFormat_OpenJobsId",
                table: "WorkFormat",
                column: "OpenJobsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "WorkFormat");

            migrationBuilder.DropTable(
                name: "OpenJobs");

            migrationBuilder.DropTable(
                name: "JobOpening");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "projectView");
        }
    }
}
