--SELECT * FROM "Users" WHERE "Email" ~ '^[^\s@]+@[^\s@]+\.[^\s@]+$';
--UPDATE "Users" SET "Email" = 'bennymonahan14@gmail.com' WHERE "Email" = 'benny.monahan14@gmail.com';

SELECT * FROM "PendingVerifications" WHERE "Email" ~ '^[^\s@]+@[^\s@]+\.[^\s@]+$';
--UPDATE "PendingVerifications" SET "Email" = 'traukiniooo@gmail.com' WHERE "Email" = 'traukinio..@gmail.com';

--DELETE FROM "Users" WHERE "Email" = '';

-- CONVERT TO LOWERCASE EMAILS
--UPDATE "Users" SET "Email" = LOWER("Email");

--SELECT * FROM "Users" 
--WHERE SPLIT_PART("Email", '@', 2) IN ('gmail.com', 'googlemail.com') 
--  AND SPLIT_PART("Email", '@', 1) ~ '\.';