-- Table	
CREATE TABLE scheduler.triggers
(
	"Id" bigserial NOT NULL,
	"FeedId" bigint NOT NULL,
	"RoutingKey" character varying(255) COLLATE pg_catalog."default" NOT NULL,
	"VirtualHost" character varying(255) COLLATE pg_catalog."default" NULL,
	"Enabled" boolean NOT NULL,
	"RepeatInterval" interval NOT NULL,
	"CreatedAtUtc" timestamp(0) without time zone NOT NULL,
	"StartAtUtc" timestamp(0) without time zone NOT NULL,
	"FireTimeUtc" timestamp(0) without time zone NOT NULL,
	"FireCount" integer NOT NULL,
	"Payload" text COLLATE pg_catalog."default",
	"UpdatedAtUtc" timestamp(0) without time zone NOT NULL,
	CONSTRAINT pk_triggers PRIMARY KEY ("Id"),
	CONSTRAINT ux_triggers_feed_id_routing_key UNIQUE ("FeedId", "RoutingKey"),
	CONSTRAINT fk_triggers_feeds FOREIGN KEY ("FeedId")
		REFERENCES scheduler.feeds ("Id") MATCH SIMPLE
		ON UPDATE NO ACTION
		ON DELETE CASCADE
)
WITH (
	OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE scheduler.triggers
	OWNER to postgres;