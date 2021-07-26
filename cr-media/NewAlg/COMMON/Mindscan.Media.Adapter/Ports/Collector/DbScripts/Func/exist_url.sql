CREATE OR REPLACE FUNCTION collector.exist_url(
	hash character varying(64)
) RETURNS boolean
AS $$
DECLARE
BEGIN
	if(hash is null or trim(hash) = '') then
		return false;
	end if;
	
	return exists(
		select 1 
		from collector.materials  
		where "OriginalUrlHash" = hash OR "ActualUrlHash" = hash
	);
END; $$ LANGUAGE 'plpgsql';

