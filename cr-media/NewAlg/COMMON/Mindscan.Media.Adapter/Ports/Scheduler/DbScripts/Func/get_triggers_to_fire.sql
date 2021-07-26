-- Function
CREATE OR REPLACE FUNCTION scheduler.get_triggers_to_fire()
	RETURNS SETOF scheduler.triggers AS $$
DECLARE
	_row record;
	_now timestamp;
	_ri int;
	_delta interval;
BEGIN
	_now = (now() at time zone 'utc');
	FOR _row IN (select * 
	from scheduler.triggers tt
	where tt."FireTimeUtc" < _now AND tt."Enabled" = true AND tt."RepeatInterval" > '00:00:00' limit 200)
	LOOP
		_ri = extract(epoch from _row."RepeatInterval");
		_delta = make_interval(secs:=ceil(extract(epoch from _now - _row."FireTimeUtc") / _ri) * _ri);
		update scheduler.triggers
			set "FireTimeUtc" = _row."FireTimeUtc" + _delta, "FireCount" = _row."FireCount" + 1
		where "Id" = _row."Id";
		RETURN NEXT _row;
	END LOOP; 
END; $$ LANGUAGE 'plpgsql';