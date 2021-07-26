CREATE TYPE collector.material AS (
	"Id" bigint,
	"CreatedAtUtc" timestamp(0) without time zone,
	"UpdatedAtUtc" timestamp(0) without time zone,
	
	"OriginalUrlPrefix" character varying(20),
	"OriginalUrlTail" character varying(2000),
	"OriginalUrlHash" character varying(64),
	"ActualUrlPrefix" character varying(20),
	"ActualUrlTail" character varying(2000),
	"ActualUrlHash" character varying(64),
	
	"SourceId" bigint,
	"ParserId" bigint,
	"FeedUrl" character varying(2000),
	"Title" character varying(1000),
	"Text" text,
	"Host" text COLLATE pg_catalog."default",
	"PublishedAtUtc" timestamp(0) without time zone,
	
	"Authors" text,
	"Tags" text,
	"Images" text,
	"Links" text,
	"Categories" text,
	"Videos" text,
	"Pdfs" text,
	"Metrics" text
);