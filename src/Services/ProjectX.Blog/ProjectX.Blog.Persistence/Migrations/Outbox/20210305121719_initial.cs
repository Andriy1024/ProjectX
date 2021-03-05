using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectX.Blog.Persistence.Migrations.Outbox
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ProjectX.Outbox");

            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "ProjectX.Outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "ProjectX.Outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    SerializedMessage = table.Column<string>(type: "json", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "ProjectX.Outbox");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "ProjectX.Outbox");
        }
    }
}
