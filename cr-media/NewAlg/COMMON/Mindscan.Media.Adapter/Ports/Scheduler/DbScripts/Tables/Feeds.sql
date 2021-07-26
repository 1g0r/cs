-- Create table
CREATE TABLE scheduler.feeds
(
	"Id" bigserial NOT NULL,
	"SourceId" bigint NOT NULL,
	"FeedType" character varying(255) COLLATE pg_catalog."default" NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"UpdatedAtUtc" timestamp(0) without time zone,
	"OriginalUrlPrefix" character varying(20) COLLATE pg_catalog."default" NOT NULL,
	"OriginalUrl" character varying(2000) COLLATE pg_catalog."default" NOT NULL,
	"ActualUrlPrefix" character varying(20) COLLATE pg_catalog."default",
	"ActualUrl" character varying(2000) COLLATE pg_catalog."default",
	"AdditionalInfo" text COLLATE pg_catalog."default",
	"Encoding" character varying(50) COLLATE pg_catalog."default",
	"Description" text COLLATE pg_catalog."default",
	CONSTRAINT pk_feeds PRIMARY KEY ("Id"),
	CONSTRAINT ux_feeds_original_url UNIQUE ("OriginalUrl"),
	CONSTRAINT fk_feeds_sources FOREIGN KEY ("SourceId")
		REFERENCES scheduler.sources ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scheduler.feeds
	OWNER to postgres;

-- Index: feed_original_url_asc
CREATE UNIQUE INDEX feed_original_url_asc
	ON scheduler.feeds USING btree
	("OriginalUrl" COLLATE pg_catalog."default")
	TABLESPACE pg_default;