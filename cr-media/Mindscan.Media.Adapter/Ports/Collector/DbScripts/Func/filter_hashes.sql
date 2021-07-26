CREATE OR REPLACE FUNCTION collector.filter_hashes(
	hashes character varying(64)[]
) RETURNS SETOF character varying(64)
AS $$
DECLARE
	existed character varying(64)[];
BEGIN
	existed = array(select h 
	from unnest(hashes) h
	inner join collector.materials m 
		on "ActualUrlHash" = h OR m."OriginalUrlHash" = h);
	
	return query 
	select h from unnest(hashes) h
	except 
	select * from unnest(existed);
END; $$ LANGUAGE 'plpgsql';