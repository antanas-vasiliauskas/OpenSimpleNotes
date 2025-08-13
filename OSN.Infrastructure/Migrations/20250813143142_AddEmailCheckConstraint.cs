using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSN.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The check constraints should pass on null valued email
            // allowing guest users.

            // Add check constraint for basic email format validation (regex)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Email_Format"" 
                CHECK (""Email"" ~ '^[^\s@]+@[^\s@]+\.[^\s@]+$');
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PendingVerifications"" 
                ADD CONSTRAINT ""CK_PendingVerifications_Email_Format"" 
                CHECK (""Email"" ~ '^[^\s@]+@[^\s@]+\.[^\s@]+$');
            ");

            // Add check constraint for email normalization rules
            // No uppercase letters
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Email_NoUppercase"" 
                CHECK (""Email"" = LOWER(""Email""));
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PendingVerifications"" 
                ADD CONSTRAINT ""CK_PendingVerifications_Email_NoUppercase"" 
                CHECK (""Email"" = LOWER(""Email""));
            ");

            // Add check constraint for no plus signs in local part
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Email_NoPlus"" 
                CHECK (SPLIT_PART(""Email"", '@', 1) !~ '\+');
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PendingVerifications"" 
                ADD CONSTRAINT ""CK_PendingVerifications_Email_NoPlus"" 
                CHECK (SPLIT_PART(""Email"", '@', 1) !~ '\+');
            ");

            // Add check constraint for no dots in Gmail/GoogleMail local part
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" 
                ADD CONSTRAINT ""CK_Users_Email_GmailNoDots"" 
                CHECK (
                    CASE 
                        WHEN SPLIT_PART(""Email"", '@', 2) IN ('gmail.com', 'googlemail.com') 
                        THEN SPLIT_PART(""Email"", '@', 1) !~ '\.'
                        ELSE TRUE 
                    END
                );
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE ""PendingVerifications"" 
                ADD CONSTRAINT ""CK_PendingVerifications_Email_GmailNoDots"" 
                CHECK (
                    CASE 
                        WHEN SPLIT_PART(""Email"", '@', 2) IN ('gmail.com', 'googlemail.com') 
                        THEN SPLIT_PART(""Email"", '@', 1) !~ '\.'
                        ELSE TRUE 
                    END
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop check constraints in reverse order
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DROP CONSTRAINT IF EXISTS ""CK_Users_Email_GmailNoDots"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PendingVerifications"" DROP CONSTRAINT IF EXISTS ""CK_PendingVerifications_Email_GmailNoDots"";");
            
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DROP CONSTRAINT IF EXISTS ""CK_Users_Email_NoPlus"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PendingVerifications"" DROP CONSTRAINT IF EXISTS ""CK_PendingVerifications_Email_NoPlus"";");
            
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DROP CONSTRAINT IF EXISTS ""CK_Users_Email_NoUppercase"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PendingVerifications"" DROP CONSTRAINT IF EXISTS ""CK_PendingVerifications_Email_NoUppercase"";");
            
            migrationBuilder.Sql(@"ALTER TABLE ""Users"" DROP CONSTRAINT IF EXISTS ""CK_Users_Email_Format"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PendingVerifications"" DROP CONSTRAINT IF EXISTS ""CK_PendingVerifications_Email_Format"";");
        }
    }
}
