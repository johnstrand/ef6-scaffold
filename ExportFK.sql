;WITH tables AS (
	SELECT 
	s.name [schema],
	t.name [table],
	t.object_id
	FROM sys.schemas s
	JOIN sys.tables t ON s.schema_id = t.schema_id
)


SELECT 
fk.name,
CASE WHEN COALESCE(p.value, '') != '' THEN p.value ELSE to_table.[table] + '!' + from_table.[table] END alias,
from_table.[schema] from_schema,
from_table.[table] from_table,
to_table.[schema] to_schema,
to_table.[table] to_table,
(
	SELECT
	from_column.name [from],
	to_column.name [to]
	FROM sys.foreign_key_columns fkc
	JOIN sys.columns from_column ON fkc.parent_column_id = from_column.column_id AND fkc.parent_object_id = from_column.object_id
	JOIN sys.columns to_column ON fkc.referenced_column_id = to_column.column_id AND fkc.referenced_object_id = to_column.object_id
	WHERE fk.object_id = fkc.constraint_object_id
	ORDER BY to_column.column_id
	FOR XML PATH('column'), ELEMENTS, TYPE
)
FROM sys.foreign_keys fk
JOIN tables from_table ON fk.parent_object_id = from_table.object_id
JOIN tables to_table ON fk.referenced_object_id = to_table.object_id
LEFT JOIN sys.extended_properties p ON p.major_id = fk.object_id
ORDER BY from_table.[schema], from_table.[table]
FOR XML PATH('foreign_key'), ROOT('foreign_keys')