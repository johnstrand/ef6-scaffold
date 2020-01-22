WITH pk AS (SELECT ixc.object_id, ixc.column_id FROM sys.indexes ix
JOIN sys.index_columns ixc ON ix.object_id = ixc.object_id AND ix.index_id = ixc.index_id
WHERE ix.is_primary_key = 1)

SELECT
	s.name [@schema],
	t.name [@name],
	(
		SELECT
		c.name,
		c.is_identity,
		c.is_nullable,
		c.is_rowguidcol,
		c.max_length,
		c.precision,
		c.scale,
		tt.name [type],
		CASE WHEN EXISTS (SELECT NULL FROM pk p WHERE c.object_id = p.object_id AND c.column_id = p.column_id) THEN 1 ELSE 0 END [is_primary_key]
		FROM sys.columns c
		JOIN sys.types tt ON c.user_type_id = tt.user_type_id
		WHERE t.object_id = c.object_id
		FOR XML PATH('column'), TYPE
	)
	FROM sys.schemas s
	JOIN sys.tables t ON s.schema_id = t.schema_id
	WHERE s.name = @schema AND t.name = @table
	FOR XML PATH('table')