WITH primary_keys as (SELECT c.object_id, c.column_id
FROM sys.key_constraints kc
JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE kc.type = 'PK'),
tables AS (
	SELECT s.name [schema], t.name [table], t.object_id
		FROM sys.schemas s
		JOIN sys.tables t ON s.schema_id = t.schema_id)

SELECT
(
	SELECT 
		fk.name,
		fk_from.[schema] [from_schema],
		fk_from.[table] [from_table],
		fk_to.[schema] [to_schema],
		fk_to.[table] [to_table],
		p.value [alias],
		(
			SELECT 
				[fk_from].name [from],
				[fk_to].name [to]
				FROM sys.foreign_key_columns fkc
				JOIN sys.columns [fk_to] ON [fk_to].object_id = fkc.referenced_object_id AND [fk_to].column_id = fkc.referenced_column_id
				JOIN sys.columns [fk_from] ON [fk_from].object_id = fkc.parent_object_id AND [fk_from].column_id = fkc.parent_column_id
				WHERE fk.object_id = fkc.constraint_object_id
				FOR XML PATH('column'), TYPE
		)
		FROM sys.foreign_keys fk
		LEFT JOIN sys.extended_properties p ON fk.object_id = p.major_id
		JOIN tables [fk_to] ON [fk_to].object_id = fk.referenced_object_id
		JOIN tables [fk_from] ON [fk_from].object_id = fk.parent_object_id
		FOR XML PATH('foreign_key'), TYPE
) 'foreign_keys',
(
	SELECT
		s.name '@name',
		(
			SELECT 
				t.name '@name',
				(
					SELECT
						c.name,
						c.is_identity,
						c.is_nullable,
						c.is_rowguidcol,
						c.max_length,
						c.precision,
						c.scale,
						ty.name [type],
						(CASE WHEN EXISTS (SELECT NULL FROM primary_keys pk WHERE pk.column_id = c.column_id AND pk.object_id = c.object_id) THEN 1 ELSE 0 END) [is_primary_key]
						FROM sys.columns c
						JOIN sys.types ty ON c.user_type_id = ty.user_type_id
						WHERE c.object_id = t.object_id
						FOR XML PATH('column'), TYPE
				) 'columns'
				FROM sys.tables t
				WHERE t.schema_id = s.schema_id
				FOR XML PATH('table'), TYPE
		)
		FROM sys.schemas s
		WHERE s.schema_id = ANY (SELECT schema_id FROM sys.tables) AND s.name NOT IN ('Temp')
		FOR XML PATH('schema'), TYPE
) 'schemas'
FOR XML RAW ('export'), TYPE