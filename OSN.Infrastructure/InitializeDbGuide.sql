psql -U postgres
123

CREATE DATABASE osndb;

CREATE USER osnadmin WITH PASSWORD 'password';
GRANT ALL PRIVILEGES ON DATABASE osndb TO osnadmin;

\c osndb
GRANT ALL ON SCHEMA public TO osnadmin;
ALTER DEFAULT PRIVELEGES IN SCHEMA public GRANT ALL ON TABLES TO osnadmin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO osnadmin;

\q
psql -U osnadmin -d osndb
password
\l
\du