CREATE DATABASE identity_db;
CREATE DATABASE recruitment_db;

-- Налаштування Recruitment Service (Пошук)
\c recruitment_db

CREATE EXTENSION IF NOT EXISTS pg_trgm;  -- Fuzzy search (схожість слів)
CREATE EXTENSION IF NOT EXISTS unaccent; -- Ігнорування акцентів/діакритики

\c postgres
SELECT datname FROM pg_database WHERE datname IN ('identity_db', 'recruitment_db');