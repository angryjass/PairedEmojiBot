using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PairedEmojiBot.Migrations
{
    /// <inheritdoc />
    public partial class Add_СrayfishGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "СrayfishGameProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<long>(type: "INTEGER", nullable: false),
                    FirstUsername = table.Column<string>(type: "TEXT", nullable: false),
                    SecondUsername = table.Column<string>(type: "TEXT", nullable: false),
                    ApproveFirstUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApproveSecondUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    Winner = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_СrayfishGameProcesses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "СrayfishGameProcesses");
        }
    }
}
