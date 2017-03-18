using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Migrator.Framework;


namespace EasyMigrator
{
    static public class ExecuteExtensions
    {
        static public void ExecuteWithConnection(this ITransformationProvider Database, Action<IDbConnection, IDbTransaction> operation)
        {
            var cmd = Database.GetCommand();
            operation(cmd.Connection, cmd.Transaction);
        }
    }
}
