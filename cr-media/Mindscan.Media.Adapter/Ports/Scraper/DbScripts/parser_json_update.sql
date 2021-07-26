create or replace function scraper.parser_json_update(
	UpdatedAtUtc timestamp(0) without time zone,
	Id bigint,
	Value text
) RETURNS SETOF scraper.jsons
AS $$
DECLARE
	newDate timestamp(0) without time zone := (now() at time zone 'utc');
BEGIN
	if (UpdatedAtUtc is null) THEN RAISE EXCEPTION 'UpdatedAtUtc can not be null';	end if;
	if (Value is null or trim(Value) = '') then RAISE EXCEPTION 'Value can not be empty';	end if;
	
	if exists(select * from scraper.parsers where "Id"=Id and "UpdatedAtUtc"=UpdatedAtUtc)
	then
		update scraper.jsons set "Enabled"=false where "Id"=Id and "Enabled"=true;
		
		return query
		insert into scraper.jsons("Id", "CreatedAtUtc", "Enabled", "Value")
		values(Id, newDate, true, Value)
		returning *;
	end if;
END $$ LANGUAGE 'plpgsql';