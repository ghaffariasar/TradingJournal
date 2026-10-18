using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingProject.Migrations
{
    /// <inheritdoc />
    public partial class SeedRegistrationSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$XwwBt0EOZpl6KZN1FnDLUuboMlgx/beC7slNJfG7oFAneCHvzZd3e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Gl1v/EM9YQgLgUt0y6xUQ.3ppEI5J.V0gBWXjMDFSMUbPPJ6ENMoe");
        }
    }
}
