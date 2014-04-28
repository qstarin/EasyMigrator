using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentMigrator.Builders.Create;


namespace EasyMigrator
{
    static public class Extensions
    {
        static public void Table<T>(this ICreateExpressionRoot Create) { Create.Table<T>(Parsing.Parser.Default); }
        static public void Table<T>(this ICreateExpressionRoot Create, Parsing.Parser parser)
        {
            var table = parser.ParseTable(typeof(T));
        }
    }
}
