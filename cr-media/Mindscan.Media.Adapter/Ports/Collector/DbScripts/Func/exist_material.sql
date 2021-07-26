CREATE OR REPLACE FUNCTION collector.exist_material(
	OriginalUrlHash character varying(64),
	ActualUrlHash character varying(64)
) RETURNS boolean
AS $$
DECLARE
BEGIN
	if (OriginalUrlHash is null) then RAISE EXCEPTION 'OriginalUrlHash can not be null'; end if;
	if (ActualUrlHash is null) then RAISE EXCEPTION 'ActualUrlHash can not be null'; end if;
	
	return exists(select 1 from collector.materials m where m."OriginalUrlHash" = OriginalUrlHash AND m."ActualUrlHash" = ActualUrlHash);

END; $$ LANGUAGE 'plpgsql';