﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyMigrator.Tests.Integration.Migrators
{
    public class MigratorDotNet : IMigrator
    {
        public MigratorDotNet(string connectionString) { }

        public void Up(Action<NPoco.Database> action) { }
        public void Down(Action<NPoco.Database> action) { }
        public void Up(Type poco) { }
        public void Down(Type poco) { }
    }
}