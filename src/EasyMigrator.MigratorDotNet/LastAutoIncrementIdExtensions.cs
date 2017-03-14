using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class LastAutoIncrementIdExtensions
    {
        static public int GetLastAutoIncrementInt32(this ITransformationProvider Database)
            => Convert.ToInt32(Database.ExecuteScalar("SELECT SCOPE_IDENTITY();"));

        static public long GetLastAutoIncrementInt64(this ITransformationProvider Database)
            => Convert.ToInt64(Database.ExecuteScalar("SELECT SCOPE_IDENTITY();"));
    }
}
