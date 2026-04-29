using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanyPathAndCallSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_CallParticipantAnalyses_ParticipantId",
                table: "Scores");

            migrationBuilder.DropTable(
                name: "CallParticipantAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_Scores_CallId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Scores_ParticipantId",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "WordsPerSecondScore",
                table: "Scores",
                newName: "ProfessionalismScore");

            migrationBuilder.RenameColumn(
                name: "PeakVolumeScore",
                table: "Scores",
                newName: "OperatorToneScore");

            migrationBuilder.RenameColumn(
                name: "AvgVolumeScore",
                table: "Scores",
                newName: "ConflictResolutionScore");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Calls",
                newName: "OperatorTranscript");

            migrationBuilder.AddColumn<int>(
                name: "ImprovementTips",
                table: "Scores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AudioFolderRoute",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerMaxVolume",
                table: "Calls",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerSentimentEnd",
                table: "Calls",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerSentimentStart",
                table: "Calls",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerTranscript",
                table: "Calls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralNotes",
                table: "Calls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperatorId",
                table: "Calls",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OperatorMaxVolume",
                table: "Calls",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatorSentiment",
                table: "Calls",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OperatorWordsPerSecond",
                table: "Calls",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_CallId",
                table: "Scores",
                column: "CallId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calls_OperatorId",
                table: "Calls",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calls_Operators_OperatorId",
                table: "Calls",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "OperatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calls_Operators_OperatorId",
                table: "Calls");

            migrationBuilder.DropIndex(
                name: "IX_Scores_CallId",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_Calls_OperatorId",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "ImprovementTips",
                table: "Scores");

            migrationBuilder.DropColumn(
                name: "AudioFolderRoute",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CustomerMaxVolume",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "CustomerSentimentEnd",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "CustomerSentimentStart",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "CustomerTranscript",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "GeneralNotes",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "OperatorId",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "OperatorMaxVolume",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "OperatorSentiment",
                table: "Calls");

            migrationBuilder.DropColumn(
                name: "OperatorWordsPerSecond",
                table: "Calls");

            migrationBuilder.RenameColumn(
                name: "ProfessionalismScore",
                table: "Scores",
                newName: "WordsPerSecondScore");

            migrationBuilder.RenameColumn(
                name: "OperatorToneScore",
                table: "Scores",
                newName: "PeakVolumeScore");

            migrationBuilder.RenameColumn(
                name: "ConflictResolutionScore",
                table: "Scores",
                newName: "AvgVolumeScore");

            migrationBuilder.RenameColumn(
                name: "OperatorTranscript",
                table: "Calls",
                newName: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Scores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "Scores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CallParticipantAnalyses",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallId = table.Column<int>(type: "int", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: true),
                    AvgVolume = table.Column<double>(type: "float", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    ImprovementNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeakVolume = table.Column<double>(type: "float", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    Transcript = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WordsPerSecond = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallParticipantAnalyses", x => x.ParticipantId);
                    table.ForeignKey(
                        name: "FK_CallParticipantAnalyses_Calls_CallId",
                        column: x => x.CallId,
                        principalTable: "Calls",
                        principalColumn: "CallId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CallParticipantAnalyses_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "OperatorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Scores_CallId",
                table: "Scores",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_ParticipantId",
                table: "Scores",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_CallParticipantAnalyses_CallId",
                table: "CallParticipantAnalyses",
                column: "CallId");

            migrationBuilder.CreateIndex(
                name: "IX_CallParticipantAnalyses_OperatorId",
                table: "CallParticipantAnalyses",
                column: "OperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_CallParticipantAnalyses_ParticipantId",
                table: "Scores",
                column: "ParticipantId",
                principalTable: "CallParticipantAnalyses",
                principalColumn: "ParticipantId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
