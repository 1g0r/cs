CREATE OR REPLACE FUNCTION collector.insert_material(
	CreatedAtUtc timestamp(0) without time zone,
	UpdatedAtUtc timestamp(0) without time zone,
	
	OriginalUrlPrefix character varying(20),
	OriginalUrlTail character varying(2000),
	OriginalUrlHash character varying(64),

	ActualUrlPrefix character varying(20),
	ActualUrlTail character varying(2000),
	ActualUrlHash character varying(64),
	
	SourceId bigint,
	ParserId bigint,
	FeedUrl character varying(2000),
	Title character varying(1000),
	Text text,
	Host text,
	PublishedAtUtc timestamp(0) without time zone,
	
	Authors text,	
	Tags text,
	Images text,
	Links text,
	Categories text,
	Videos text,
	Pdfs text,
	Metrics text
) RETURNS SETOF collector.material
AS $$
DECLARE
	new_id bigint;
BEGIN
	insert into collector.materials as m (
		"CreatedAtUtc", "UpdatedAtUtc", "OriginalUrlPrefix", "OriginalUrlTail", "OriginalUrlHash",
		"ActualUrlPrefix", "ActualUrlTail", "ActualUrlHash")
	values(CreatedAtUtc, UpdatedAtUtc, OriginalUrlPrefix, OriginalUrlTail, OriginalUrlHash, 
		ActualUrlPrefix, ActualUrlTail, ActualUrlHash)
	ON CONFLICT ("OriginalUrlHash") DO UPDATE SET "CreatedAtUtc" = EXCLUDED."CreatedAtUtc"
	RETURNING m."Id" into new_id;

	insert into collector.materials_data(
		"MaterialId", "SourceId", "ParserId", "FeedUrl", "Title", "Text", "Host", "PublishedAtUtc", "Authors", 
		"Tags", "Images", "Links", "Categories", "Videos", "Pdfs", "Metrics"
	)values(new_id, SourceId, ParserId, FeedUrl, Title, Text, Host, PublishedAtUtc, Authors,
		   Tags, Images, Links, Categories, Videos, Pdfs, Metrics)
	ON CONFLICT ("MaterialId") DO NOTHING;

	return query
	select m."Id", m."CreatedAtUtc", m."UpdatedAtUtc", m."OriginalUrlPrefix", m."OriginalUrlTail", 
		m."OriginalUrlHash", m."ActualUrlPrefix", m."ActualUrlTail", m."ActualUrlHash",
		md."SourceId", md."ParserId", md."FeedUrl", md."Title", md."Text", md."Host", md."PublishedAtUtc", md."Authors",
		md."Tags", md."Images", md."Links", md."Categories", md."Videos", md."Pdfs", md."Metrics"
	from collector.materials m
		inner join collector.materials_data md on md."MaterialId"=m."Id"
	where m."Id" = new_id;
END; $$ LANGUAGE 'plpgsql';

