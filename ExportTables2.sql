WITH primary_keys as (SELECT c.object_id, c.column_id
FROM sys.key_constraints kc
JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE kc.type = 'PK'),
x AS (
	SELECT s.name + '.' + t.name [name], c.name [column], c.object_id, c.column_id
		FROM sys.schemas s
		JOIN sys.tables t ON s.schema_id = t.schema_id
		JOIN sys.columns c ON t.object_id = c.object_id)

SELECT
	s.name '@name',
	(
		SELECT 
			t.name '@name',
			(
				SELECT 
					fk.name,
					p.value,
					(
						SELECT 
							[ref].name [ref_table],
							[ref].[column] [ref_column],
							[parent].name [parent_table],
							[parent].[column] [parent_column]
							FROM sys.foreign_key_columns fkc
							JOIN x [ref] ON [ref].object_id = fkc.referenced_object_id AND [ref].column_id = fkc.referenced_column_id
							JOIN x [parent] ON [parent].object_id = fkc.parent_object_id AND [parent].column_id = fkc.parent_column_id
							WHERE fk.object_id = fkc.constraint_object_id
							FOR XML PATH('column'), TYPE
					)
					FROM sys.foreign_keys fk
					LEFT JOIN sys.extended_properties p ON fk.object_id = p.major_id
					WHERE fk.parent_object_id = t.object_id
					FOR XML PATH('foreign_key'), TYPE
			) 'foreign_keys',
			(
				SELECT
					c.name,
					c.is_identity,
					c.is_nullable,
					c.is_rowguidcol,
					c.max_length,
					c.precision,
					(CASE WHEN EXISTS (SELECT NULL FROM primary_keys pk WHERE pk.column_id = c.column_id AND pk.object_id = c.object_id) THEN 1 ELSE 0 END) [primary_key]
					FROM sys.columns c
					WHERE c.object_id = t.object_id
					FOR XML PATH('column'), TYPE
			) 'columns'
			FROM sys.tables t
			WHERE t.schema_id = s.schema_id
			FOR XML PATH('table'), TYPE
	)
	FROM sys.schemas s
	WHERE s.schema_id = ANY (SELECT schema_id FROM sys.tables) AND s.name NOT IN ('Temp')
	FOR XML PATH('schema'), ROOT('schemas'), TYPE