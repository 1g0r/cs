CREATE TABLE collector.materials
(
	"Id" bigserial NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"UpdatedAtUtc" timestamp(0) without time zone NOT NULL,
	
	"OriginalUrlPrefix" character varying(20) COLLATE pg_catalog."default" NOT NULL,
	"OriginalUrlTail" character varying(2000) COLLATE pg_catalog."default" NOT NULL,
	"OriginalUrlHash" character varying(64) COLLATE pg_catalog."default" NOT NULL,
	
	"ActualUrlPrefix" character varying(20) COLLATE pg_catalog."default" NOT NULL,
	"ActualUrlTail" character varying(2000) COLLATE pg_catalog."default" NOT NULL,
	"ActualUrlHash" character varying(64) COLLATE pg_catalog."default" NOT NULL,
	
	CONSTRAINT pk_materials PRIMARY KEY ("Id")
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE collector.materials
	OWNER to postgres;

-- Index: ux_materials_original_url_hash
CREATE UNIQUE INDEX ux_materials_original_url_hash ON collector.materials 
	USING btree ("OriginalUrlHash" COLLATE pg_catalog."default" ASC)
	TABLESPACE pg_default;

-- Index: ux_materials_actual_url_hash
CREATE UNIQUE INDEX ux_materials_actual_url_hash ON collector.materials 
	USING btree ("ActualUrlHash" COLLATE pg_catalog."default" ASC)
	TABLESPACE pg_default;
	
-- Uncomment the line below if pg_trgm not enabled
--CREATE EXTENSION pg_trgm;

-- Index: gx_materials_original_url_tail
CREATE INDEX gx_materials_original_url_tail ON collector.materials USING gin (lower("OriginalUrlTail") gin_trgm_ops);

-- Index: gx_materials_actual_url_tail
CREATE INDEX gx_materials_actual_url_tail ON collector.materials USING gin (lower("ActualUrlTail") gin_trgm_ops);