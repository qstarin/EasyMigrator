using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EasyMigrator.Extensions;
using NUnit.Framework;


namespace EasyMigrator.Tests
{
    [TestFixture]
    public class ReflectionTests
    {
        class Poco
        {
            public string Field1;
        }

        [Test]
        public void FieldExpression()
        {
            var e = ((Expression<Func<Poco, object>>)(p => p.Field1));
            var f = e.GetExpressionField();
            Assert.AreEqual(typeof(Poco).GetField("Field1"), f);
        }
    }
}
