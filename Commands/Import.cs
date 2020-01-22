using System;
using System.Collections.Generic;
using System.Text;

namespace ScaffoldEF.Commands
{
    [Command("import")]
    static class Import
    {
        [Command("fk")]
        internal static void ForeignKey(string file)
        {
            /*
            DECLARE @v sql_variant 
            SET @v = N'Test'
            EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'<SCHEMA>', N'TABLE', N'<TABLE>', N'CONSTRAINT', N'<FK_NAME>'
            */

        }
    }
}
