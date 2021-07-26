-- Create table
CREATE TABLE scraper.parsers
(
	"Id" bigserial NOT NULL,
	"SourceId" bigint NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"UpdatedAtUtc" timestamp(0) without time zone NOT NULL,
	"Host" character varying(500) COLLATE pg_catalog."default" NOT NULL,
	"Path" character varying(1000) COLLATE pg_catalog."default",
	"Encoding" character varying(30) COLLATE pg_catalog."default" NOT NULL DEFAULT 'utf-8',
	"UseBrowser" boolean NOT NULL DEFAULT false,
	"Tag" character varying(500) COLLATE pg_catalog."default" NULL,
	"AdditionalInfo" text COLLATE pg_catalog."default",
	CONSTRAINT pk_parsers PRIMARY KEY ("Id"),
	CONSTRAINT fk_parsers_sources FOREIGN KEY ("SourceId")
		REFERENCES scheduler.sources ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scraper.parsers
	OWNER to postgres;

-- Index: ux_parsers_host
CREATE UNIQUE INDEX ux_parsers_host ON scraper.parsers USING btree("Host" COLLATE pg_catalog."default")
	TABLESPACE pg_default WHERE "Path" IS NULL AND "Tag" IS NULL;

-- Index: ux_parsers_host_path
CREATE UNIQUE INDEX ux_parsers_host_path ON scraper.parsers USING btree ("Host" COLLATE pg_catalog."default", "Path" COLLATE pg_catalog."default")
	TABLESPACE pg_default WHERE "Path" IS NOT NULL AND "Tag" IS NULL;

-- Index: ux_parsers_host_tag
CREATE UNIQUE INDEX ux_parsers_host_tag ON scraper.parsers USING btree ("Host" COLLATE pg_catalog."default", "Tag" COLLATE pg_catalog."default")
	TABLESPACE pg_default WHERE "Tag" IS NOT NULL AND "Path" IS NULL;

-- Index: ux_parsers_host_path_tag
CREATE UNIQUE INDEX ux_parsers_host_path_tag ON scraper.parsers USING btree (
	"Host" COLLATE pg_catalog."default", 
	"Path" COLLATE pg_catalog."default", 
	"Tag" COLLATE pg_catalog."default")
	TABLESPACE pg_default WHERE "Tag" IS NOT NULL AND "Path" IS NOT NULL;