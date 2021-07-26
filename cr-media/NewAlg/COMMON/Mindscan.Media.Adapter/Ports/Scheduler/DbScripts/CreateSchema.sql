-- DataBase
CREATE DATABASE media
	WITH 
	OWNER = postgres
	ENCODING = 'UTF8'
	LC_COLLATE = 'Russian_Russia.1251'
	LC_CTYPE = 'Russian_Russia.1251'
	TABLESPACE = pg_default
	CONNECTION LIMIT = -1;
	
-- Schema
CREATE SCHEMA scheduler
	AUTHORIZATION postgres;