CREATE TABLE collector.materials_data
(
	"MaterialId" bigint NOT NULL,
	"SourceId" bigint NOT NULL,
	"ParserId" bigint NULL,
	"FeedUrl" character varying(2000) COLLATE pg_catalog."default" NULL,
	
	"Title" character varying(1000) COLLATE pg_catalog."default" NOT NULL,
	"Text" text COLLATE pg_catalog."default",
	"Host" text COLLATE pg_catalog."default",
	"PublishedAtUtc" timestamp(0) without time zone NOT NULL,
	
	"Authors" text COLLATE pg_catalog."default" NULL,	
	"Tags" text COLLATE pg_catalog."default",
	"Images" text COLLATE pg_catalog."default",
	"Links" text COLLATE pg_catalog."default",
	"Categories" text COLLATE pg_catalog."default",
	"Videos" text COLLATE pg_catalog."default",
	"Pdfs" text COLLATE pg_catalog."default",
	"Metrics" text COLLATE pg_catalog."default",
	
	CONSTRAINT fk_materials_data_materials FOREIGN KEY ("MaterialId")
		REFERENCES collector.materials ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE collector.materials
	OWNER to postgres;

CREATE UNIQUE INDEX ux_materials_data_material_id ON collector.materials_data 
	USING btree ("MaterialId" ASC)
	TABLESPACE pg_default;

CREATE INDEX ix_materials_data_source_id
	ON collector.materials_data USING btree ("SourceId") TABLESPACE pg_default;

-- Uncomment the line below if pg_trgm not enabled
--CREATE EXTENSION pg_trgm;
CREATE INDEX gx_materials_data_feed_url ON collector.materials_data USING gin (lower("FeedUrl") gin_trgm_ops);