-- Create table
CREATE TABLE scraper.jsons
(
	"Id" bigint NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"Enabled" boolean NOT NULL DEFAULT false,
	"Value" text COLLATE pg_catalog."default" NOT NULL,
	
	CONSTRAINT ux_jsons_parser_id_created_at_utc UNIQUE ("Id", "CreatedAtUtc"),
	CONSTRAINT fk_jsons_parsers FOREIGN KEY ("Id")
		REFERENCES scraper.parsers ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scraper.jsons
	OWNER to postgres;