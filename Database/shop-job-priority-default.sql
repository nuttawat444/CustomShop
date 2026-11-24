-- Keep historical priority values and allow new jobs to omit the legacy column.
ALTER TABLE shop_jobs ALTER COLUMN Priority SET DEFAULT '';
