-- Create function
CREATE OR REPLACE FUNCTION scraper.parser_update(
	Id bigint,
	UpdatedAtUtc timestamp(0) without time zone,
	Host character varying(500),
	Path character varying(1000),
	AdditionalInfo text,
	Tag character varying(500),
	Encoding character varying(30) default 'utf-8',
	UseBrowser boolean default false
) RETURNS SETOF scraper.parsers
AS $$
DECLARE
	newDate timestamp(0) without time zone := (now() at time zone 'utc');
BEGIN
	if (Id is null) then RAISE EXCEPTION 'Id can not be null';	end if;
	if (UpdatedAtUtc is null) THEN RAISE EXCEPTION 'UpdatedAtUtc can not be null';	end if;
	if (Host is null) then RAISE EXCEPTION 'Host can not be null';	end if;
	if (Path = '') then Path:= null; end if;
	if (Tag = '') then Tag:=null; end if;

	return query
	update scraper.parsers 
		set 
			"UpdatedAtUtc"=newDate, 
			"Host"=Host,
			"Path"=Path,
			"AdditionalInfo"=AdditionalInfo,
			"Encoding"=Encoding,
			"UseBrowser"=UseBrowser,
			"Tag"=Tag
	where "Id"=Id and "UpdatedAtUtc"=UpdatedAtUtc and (
			"Host" != Host or
			"Path" is distinct from Path or
			"Tag" is distinct from Tag or
			"AdditionalInfo" != AdditionalInfo or
			"Encoding" != Encoding or
			"UseBrowser" != UseBrowser
		)
	returning *;
END $$ LANGUAGE 'plpgsql';