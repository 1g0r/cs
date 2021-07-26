-- Create table
CREATE OR REPLACE FUNCTION integration.update_publication_date(su text, nd timestamp(0) with time zone, pu text)
 RETURNS TABLE ("PublicationDate" timestamp(0) with time zone, "Url" text, "Source" text)
AS $$
DECLARE
BEGIN
	IF EXISTS (SELECT 1 FROM integration.last_by_published_at WHERE source=su)
	THEN
		return query
		update integration.last_by_published_at set published_at = nd
		where source=su and published_at < nd 
		returning published_at as "PublicationDate", url as "Url", source as "Source";  
	ELSE
		return query
		insert into integration.last_by_published_at(published_at, source, url)
		values(nd, su, pu)
		returning published_at as "PublicationDate", url as "Url", source as "Source";
	END IF;
END; $$ LANGUAGE 'plpgsql';