using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tareas.Migrations
{
    public partial class AdminRol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(select Id from AspNetRoles where Id = 'e8ad165b-5695-4fa0-ad4d-416623dc976f')
                                    BEGIN
                                    insert AspNetRoles(Id, [Name],[NormalizedName])
                                    values('e8ad165b-5695-4fa0-ad4d-416623dc976f','admin','ADMIN')
                                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE AspNetRoles where Id = 'e8ad165b-5695-4fa0-ad4d-416623dc976f'");
        }
    }
}
