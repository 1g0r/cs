-- Create table
CREATE TABLE scraper.tests(
	"Id" bigserial NOT NULL,
	"ParserId" bigint NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"UpdatedAtUtc" timestamp(0) without time zone,
	"PageUrlPrefix" character varying(20) COLLATE pg_catalog."default" NOT NULL,
	"PageUrl" character varying(2000) COLLATE pg_catalog."default" NOT NULL,
	"Enabled" boolean not null default true,
	"ResultJson" text not null,
	"Passed" boolean not null default false,
	"LastPassedAtUtc" timestamp(0) without time zone,
	"ExecutionsCount" integer not null default 0,
	CONSTRAINT pk_tests PRIMARY KEY ("Id"),
	CONSTRAINT ux_tests_parser_id_url_hash UNIQUE ("ParserId", "PageUrl"),
	CONSTRAINT fk_tests_parsers FOREIGN KEY ("ParserId")
		REFERENCES scraper.parsers ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scraper.tests OWNER to postgres;

-- Index: ux_tests_page_url
CREATE UNIQUE INDEX ux_tests_url_hash
	ON scraper.tests USING btree
	("PageUrl" COLLATE pg_catalog."default")
	TABLESPACE pg_default;