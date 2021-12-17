# Easy Migrator

EasyMigrator allows you to specify database schema using simple POCOs with minimally attributed fields to represent columns. EasyMigrator's core can be adapted to sit on top of various migration libraries. Adapters for FluentMigrator ([NuGet Package](https://www.nuget.org/packages/EasyMigrator.FluentMigrator/), [Project site](https://github.com/schambers/fluentmigrator)) and Migrator.Net ([NuGet Package](https://www.nuget.org/packages/EasyMigrator.MigratorDotNet), [Project site](https://code.google.com/p/migratordotnet/)) are provided. If you have your own favorite migration library, contributions are welcome, and new migration adapters are reasonably easy to add even with integration tests.

Note: Currently there are workarounds for Migrator.Net (supporting max length columns and decimal precision, quoting table and column names that are reserved words) that use Microsoft Sql Server specific SQL statements. Therefore, at this time other database engines are not officially supported (if there's demand it will be relatively easy to remove this restriction).

## Poco's for Table Schema

It doesn't get much easier. Simply define a class with fields. The class name will be used for the table name, and the field names and types will be used for the column names and types. A few attributes are available to provide additional schema metadata for columns. Fields can be private or public, and these poco classes are handy to use along with a Micro ORM (like [NPoco](https://github.com/schotime/NPoco)) for easy DML in your migrations.

Here is a simple example class to demonstrate many of the available features:

```csharp
[Name("MyTableData")]
class Poco
{
    [Pk(Clustered = false)] Guid Uuid;
    [AutoInc(int.MinValue, 1)] int Sequence;
    bool Accepted = true;
    [NotNull, Index] string Name;
    [Fixed(8), Ansi, NotNull, Clustered(Name = "idx_code")] string Code;
    [Length(500)] string Title;
    [Long] string ShortDesc;
    [Max] string Desc;
    [DbType(DbType.Currency)] decimal? Rate;
    [Default("GETUTCDATE()")] DateTime CreatedOn;
    [Fk("OtherTable")] int OtherTableId;
    [Fk(typeof(Reference))] int? RefId;

    CompositeIndex<Poco> index1 = new CompositeIndex<Poco>(x => x.Name, x => x.Code);

    [Unique(Name = "IX_Custom_Name")] 
    CompositeIndex<Poco> index2 = new CompositeIndex<Poco>(
        new Descending<Poco>(x => x.Sequence), 
        new Ascending<Poco>(x => x.Code));
}

class MyTableData 
{
    [Precision(Length.Short, 3)] decimal Adjustment;
    [Length(Length.Medium)] string Headline;
    CompositeIndex index1 = new CompositeIndex(new Descending("Adjustment"), new Ascending("Headline"));
    CompositeIndex index2 = new CompositeIndex(new IndexColumn("Adjustment"), new IndexColumn("Name"));
    CompositeIndex index3 = new CompositeIndex("Sequence", "Headline DESC");
}

class Reference 
{
    [Pk] int LegacyId;
    string Department;
}
```

## Conventions

Exactly how POCO classes are converted into table schema is based on conventions, and those conventions are configurable by using the `EasyMigrator.Parsing.Conventions` class. The extension methods for migration frameworks use `EasyMigrator.Parsing.Parser.Default` unless an alternate Parser object is passed into the method. The Default parser property is settable, so it can be replaced in it's entirety. The Convention's properties are also settable can be modified individually. You can see an example of this in the unit tests, where the default table name function is over-ridden (in `EasyMigrator.Tests.TableTestBase`).

Most of the conventions are self-explanatory. The default conventions and unit tests provide guidance. Further documentation on these will be added to xml comments.

### Lengths

Personally, when I use variable length columns, I stick to just a couple sizes. With a variable length column, what difference does the maximum length make, as long as it is long enough? Storage space and ability to index. Rather than plop a 50 or 100 or 255 or 4000 in all over the place, I thought it useful to have some lengths defined by convention. `Short`, `Medium`, `Long`, and `Max` are currently available. In the future, I intend to implement user-definable lengths. This would allow more domain-appropriate named conventions for length like Name or Url, instead of Medium or Long. Lengths are available for strings, binary, and decimal precision.

### Attributes

Several attributes are available to provide additional schema definition. Most are self-explanatory; some notes are here to clarify.

### Pk

Putting the `Pk` attribute on a field will make it part of the primary key. Composite primary keys can be created by placing `Pk` attributes on multiple fields. Unclustered primary keys are supported.

If none of your fields sport the `Pk` attribute, the `Conventions.PrimaryKey` function will be used to create one. If you do not want a default primary key to be created, change the PrimaryKey convention function to null or put a `NoPk` attribute on the class definition.

### Fk

The `Fk` attribute makes the field it is applied to a foreign key. Conventions are used to determine the primary table's primary key field if it is not specified in the attribute. Composite foreign keys are not supported.

### NotNull

If the .Net type of your field is nullable (a reference type or `Nullable<T>`), your column will be nullable - unless you apply this attribute. If the .Net type of your field is not nullable, but you need your column to be nullable, simply make the .Net type of your field nullable (`T?`) or if you really need to use the `Null` attribute (though this is deprecated).

Nullable reference types are unsupported.

### Name

The `Name` attribute can be used to set a specific name on either the class/table or field/column.

### DbType

The `DbType` attribute will force a field/column to the specified database type.

### Precision

The `Precision` attribute allows you to set the precision and scale on decimal fields.

### Ansi

The `Ansi` attribute creates a text column with ansi instead of unicode characters.

### Fixed

The `Fixed` attribute creates a fixed length instead of variable length text column.

### Default

The `Default` attribute allows you to specify the default value for a column. This is particularly useful for using SQL functions as a default value. The other way to set a default value is to assign a non-default value to the column's field in your class.

### Index

The `Index` attribute creates an index on the single field it appears on.

### Unique

The `Unique` attribute creates a unique index on the single field it appears on.

### Clustered

The `Clustered` attribute creates a clustered index on the single field it appears on. All clustered indices are also set to unique.

### Composite Indices

To add a composite index to a table you can create a field in your class of type `CompositeIndex` or `CompositeIndex<TTable>`. The generic type does not have to be the same as the enclosing type. When order is not important you can simply pass string column names or field expressions to the constructors of `CompositeIndex`. If you need specific ordering you can create instances of `Ascending`, `Descending`, `Ascending<TTable>`, and `Descending<TTable>` classes to pass to the constructors. Indices are added when you are creating a table or adding new columns to a table.

There are also migration framework specific api's for creating composite indices as a separate operation.

## Migration Framework Adapters

### Migrator.Net

Add the [EasyMigrator.MigratorDotNet](https://www.nuget.org/packages/EasyMigrator.MigratorDotNet/) package to your migration project. Extension methods are in the `EasyMigrator` namespace.

#### Creating tables and adding columns to tables

```csharp
using EasyMigrator;
using Migrator.Framework;

namespace MyProject.Migrations 
{
    [Migration(1)]
    public class CreatePocoTable : Migration
    {
        public override void Up() {
            Database.AddTable<Reference>();
            Database.AddTable<Poco>();
            Database.AddColumns<MyTableData>();
        }

        public override void Down() {
            Database.RemoveColumns<MyTableData>();
            Database.RemoveTable<Poco>();
            Database.RemoveTable<Reference>();
        }
    }

    [Name("MyTableData")]
    class Poco
    {
        [Pk(Clustered = false)] Guid Uuid;
        [AutoInc(int.MinValue, 1)] int Sequence;
        bool Accepted = true;
        [NotNull, Index] string Name;
        [Fixed(8), Ansi, NotNull, Clustered(Name = "idx_code")] string Code;
        [Length(500)] string Title;
        [Long] string ShortDesc;
        [Max] string Desc;
        [DbType(DbType.Currency)] decimal? Rate;
        [Default("GETUTCDATE()")] DateTime CreatedOn;
        [Fk("OtherTable")] int OtherTableId;
        [Fk(typeof(Reference))] int? RefId;

        CompositeIndex<Poco> index1 = new CompositeIndex<Poco>(x => x.Name, x => x.Code);

        [Unique(Name = "IX_Custom_Name")] 
        CompositeIndex<Poco> index2 = new CompositeIndex<Poco>(
            new Descending<Poco>(x => x.Sequence), 
            new Ascending<Poco>(x => x.Code));
    }

    class MyTableData 
    {
        [Precision(Length.Short, 3)] decimal Adjustment;
        [Length(Length.Medium)] string Headline;
        CompositeIndex index1 = new CompositeIndex(new Descending("Adjustment"), new Ascending("Headline"));
        CompositeIndex index2 = new CompositeIndex(new IndexColumn("Adjustment"), new IndexColumn("Name"));
        CompositeIndex index3 = new CompositeIndex("Sequence", "Headline DESC");
    }

    class Reference 
    {
        [Pk] int LegacyId;
        string Department;
    }
}
```

#### Creating indices

```csharp
using EasyMigrator;
using Migrator.Framework;

namespace MyProject.Migrations 
{
    [Migration(1)]
    public class CreatePocoTable : Migration
    {
        public override void Up() {
            Database.AddTable<Poco>();
            Database.AddIndex<Poco>(t => t.Name, t => t.Code);
            Database.AddIndex("IX_Custom_Name", true, 
                  new Descending<Poco>(t => t.Sequence), 
                  new Ascending<Poco>(t => t.Code));
        }

        public override void Down() {
            Database.RemoveIndexByName<Poco>("IX_Custom_Name");
            Database.RemoveIndex<Poco>(t => t.Name, t => t.Code);
            Database.RemoveTable<Poco>();
        }
    }

    [Name("MyTableData")]
    class Poco
    {
        [Pk(Clustered = false)] Guid Uuid;
        [AutoInc(int.MinValue, 1)] int Sequence;
        bool Accepted = true;
        [NotNull, Index] string Name;
        [Fixed(8), Ansi, NotNull, Clustered(Name = "idx_code")] string Code;
        [Length(500)] string Title;
        [Long] string ShortDesc;
        [Max] string Desc;
        [DbType(DbType.Currency)] decimal? Rate;
        [Default("GETUTCDATE()")] DateTime CreatedOn;
        [Fk("OtherTable")] int OtherTableId;
        [Fk(typeof(Reference))] int? RefId;
        [Precision(Length.Short, 3)] decimal Adjustment;
        [Length(Length.Medium)] string Headline;
    }
}
```

### FluentMigrator

Support for FluentMigrator has fallen behind as I no longer use this migration library day-to-day. Many features now supported by EasyMigrator.MigratorDotNet are not supported by EasyMigrator.FluentMigrator.

Add the [EasyMigrator.FluentMigrator](https://www.nuget.org/packages/EasyMigrator.FluentMigrator/) package to your migration project. Extension methods are in the `EasyMigrator` namespace.

#### Creating tables and adding columns to tables

```csharp
using EasyMigrator;
using FluentMigrator;

namespace MyProject.Migrations 
{
    [Migration(1)]
    public class CreatePocoTable : Migration
    {
        public override void Up() {
            Create.Table<Reference>();
            Create.Table<Poco>();
            Create.Columns<MyTableData>();
        }

        public override void Down() {
            Delete.Columns<MyTableData>();
            Delete.Table<Poco>();
            Delete.Table<Reference>();
        }
    }

    [Name("MyTableData")]
    class Poco
    {
        [Pk(Clustered = false)] Guid Uuid;
        [AutoInc(int.MinValue, 1)] int Sequence;
        bool Accepted = true;
        [NotNull, Index] string Name;
        [Fixed(8), Ansi, NotNull, Clustered(Name = "idx_code")] string Code;
        [Length(500)] string Title;
        [Long] string ShortDesc;
        [Max] string Desc;
        [DbType(DbType.Currency)] decimal? Rate;
        [Default("GETUTCDATE()")] DateTime CreatedOn;
        [Fk("OtherTable")] int OtherTableId;
        [Fk(typeof(Reference))] int? RefId;

        CompositeIndex<Poco> index1 = new CompositeIndex<Poco>(x => x.Name, x => x.Code);

        [Unique(Name = "IX_Custom_Name")] 
        CompositeIndex<Poco> index2 = new CompositeIndex<Poco>(
            new Descending<Poco>(x => x.Sequence), 
            new Ascending<Poco>(x => x.Code));
    }

    class MyTableData 
    {
        [Precision(Length.Short, 3)] decimal Adjustment;
        [Length(Length.Medium)] string Headline;
        CompositeIndex index1 = new CompositeIndex(new Descending("Adjustment"), new Ascending("Headline"));
        CompositeIndex index2 = new CompositeIndex(new IndexColumn("Adjustment"), new IndexColumn("Name"));
        CompositeIndex index3 = new CompositeIndex("Sequence", "Headline DESC");
    }

    class Reference 
    {
        [Pk] int LegacyId;
        string Department;
    }
}
```

#### Creating indices

```csharp
using EasyMigrator;
using FluentMigrator;

namespace MyProject.Migrations 
{
    [Migration(1)]
    public class CreatePocoTable : Migration
    {
        public override void Up() {
            Create.Table<Poco>();
            Create.Index<Poco>().OnColumns(t => t.Name, t => t.Code);
            Create.Index("IX_Custom_Name")
                  .OnTable<Poco>()
                  .OnColumns(new Descending<Poco>(t => t.Sequence), 
                             new Ascending<Poco>(t => t.Code))
                  .WithOptions()
                  .Unique();
        }

        public override void Down() {
            Delete.Index("IX_Custom_Name").OnTable<Poco>();
            Delete.Index<Poco>().OnColumns(t => t.Name, t => t.Code);
            Delete.Table<Poco>();
        }
    }

    [Name("MyTableData")]
    class Poco
    {
        [Pk(Clustered = false)] Guid Uuid;
        [AutoInc(int.MinValue, 1)] int Sequence;
        bool Accepted = true;
        [NotNull, Index] string Name;
        [Fixed(8), Ansi, NotNull, Clustered(Name = "idx_code")] string Code;
        [Length(500)] string Title;
        [Long] string ShortDesc;
        [Max] string Desc;
        [DbType(DbType.Currency)] decimal? Rate;
        [Default("GETUTCDATE()")] DateTime CreatedOn;
        [Fk("OtherTable")] int OtherTableId;
        [Fk(typeof(Reference))] int? RefId;
        [Precision(Length.Short, 3)] decimal Adjustment;
        [Length(Length.Medium)] string Headline;
    }
}
```
