using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FluentMigrator;


namespace EasyMigrator
{
    static public class LastAutoIncrementIdExtensions
    {
        static public void GetLastAutoIncrementInt32(this Migration migration, Action<int> receiveId)
            => migration.GetLastAutoIncrementInt32((id, conn, tran) => receiveId(id));

        static public void GetLastAutoIncrementInt32(this Migration migration, Action<int, IDbConnection, IDbTransaction> receiveId)
            => migration.Execute.WithConnection((conn, tran) => receiveId(Convert.ToInt32(CreateGetLastAutoIncIdCommand(conn, tran).ExecuteScalar()), conn, tran));

        static public void GetLastAutoIncrementInt64(this Migration migration, Action<long> receiveId)
            => migration.GetLastAutoIncrementInt64((id, conn, tran) => receiveId(id));

        static public void GetLastAutoIncrementInt64(this Migration migration, Action<long, IDbConnection, IDbTransaction> receiveId)
            => migration.Execute.WithConnection((conn, tran) => receiveId(Convert.ToInt64(CreateGetLastAutoIncIdCommand(conn, tran).ExecuteScalar()), conn, tran));

        static private IDbCommand CreateGetLastAutoIncIdCommand(IDbConnection conn, IDbTransaction tran)
        {
            var cmd = conn.CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT SCOPE_IDENTITY();";
            return cmd;
        }
    }
}
