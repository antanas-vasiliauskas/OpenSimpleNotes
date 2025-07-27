-- Entity framework should automatically create database and tables.
-- You need to:
-- 1) provide default user (usually postgres) that has all the access privileges
-- 2) default users password that was configured during installation. If don't remember there is way to change it without reinstalling.
-- 3) migrations: dotnet ef migrations add InitialCreate --project ../OSN.Infrastructure
-- 4) in Program.cs code that automaticlly applies new migrations (ideally in developemnt environment only)

-- check for user:
-- psql -U postgres
-- 123

-- possible errors:
-- Since application is multiproject, add EntityFramework.Design to OSN
-- and run migrations command from OSN
-- because you need to run it from same place where appsettings.json is, your STARTUP project.
-- while adding --project ../OSN.Infrastructure to the command
-- do: dotnet ef migrations add InitialCreate --project ../OSN.Infrastructure
-- don't: dotnet ef migrations add InitialCreate