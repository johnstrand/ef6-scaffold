namespace ScaffoldEF
{
	public static class Query
	{
		public static string ExportForeignKeys { get; } = typeof(Program).Assembly.GetManifestResourceStream("ScaffoldEF.Queries.ExportForeignKeys.sql").ReadAndClose();
		public static string ExportSingleTable { get; } = typeof(Program).Assembly.GetManifestResourceStream("ScaffoldEF.Queries.ExportSingleTable.sql").ReadAndClose();
		public static string ExportTables { get; } = typeof(Program).Assembly.GetManifestResourceStream("ScaffoldEF.Queries.ExportTables.sql").ReadAndClose();
	}
}