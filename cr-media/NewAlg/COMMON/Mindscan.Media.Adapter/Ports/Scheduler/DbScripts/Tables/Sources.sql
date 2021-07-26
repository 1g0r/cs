-- Create table
CREATE TABLE scheduler.sources
(
	"Id" bigserial NOT NULL,
	"Url" character varying(2000) COLLATE pg_catalog."default" NOT NULL,
	"SourceType" character varying(255) COLLATE pg_catalog."default" NOT NULL,
	"Name" text COLLATE pg_catalog."default" NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"UpdatedAtUtc" timestamp(0) without time zone,
	"AdditionalInfo" text COLLATE pg_catalog."default",
	CONSTRAINT pk_sources PRIMARY KEY ("Id"),
	CONSTRAINT ux_sources_url UNIQUE ("Url")
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scheduler.sources
	OWNER to postgres;

-- Index: source_url_asc
CREATE UNIQUE INDEX source_url_asc
	ON scheduler.sources USING btree
	("Url" COLLATE pg_catalog."default")
	TABLESPACE pg_default;