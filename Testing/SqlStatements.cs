using SimpleCrud;
using Testing.Models;

namespace Testing;

[TestClass]
public class SqlStatements
{
    [TestMethod]
    public void InsertPlain()
    {
        var result = SqlServer.Insert("this", "that", new[] { "Column1", "Column2", "Column3" });
        Assert.IsTrue(result.Equals(
            @"INSERT INTO [this].[that] (
                [Column1], [Column2], [Column3]
            ) VALUES (
                @Column1, @Column2, @Column3
            ); SELECT SCOPE_IDENTITY()"));
    }

    [TestMethod]
    public void UpdatePlain()
    {
        var result = SqlServer.Update("this", "that", new[] { "Column1", "Column2", "Column3", "Id" });
        Assert.IsTrue(result.Equals(
            @"UPDATE [this].[that] SET [Column1]=@Column1, [Column2]=@Column2, [Column3]=@Column3
            WHERE [Id]=@Id"));
    }

    [TestMethod]
    public void DeletePlain()
    {
        var result = SqlServer.Delete("this", "that", new[] { "Id" });
        Assert.IsTrue(result.Equals("DELETE [this].[that] WHERE [Id]=@Id"));
    }

    [TestMethod]
    public void InsertGeneric()
    {
        var result = SqlServer.Insert<SampleEntity>();
        Assert.IsTrue(result.Equals(
            @"INSERT INTO [whatever].[Sample] (
                [Name], [Description]
            ) VALUES (
                @Name, @Description
            ); SELECT SCOPE_IDENTITY()"));
    }

    [TestMethod]
    public void InsertGenericWithOverride()
    {
        var result = SqlServer.Insert<SampleEntity>("dbo.Hello");
        Assert.IsTrue(result.Equals(
            @"INSERT INTO [dbo].[Hello] (
                [Name], [Description]
            ) VALUES (
                @Name, @Description
            ); SELECT SCOPE_IDENTITY()"));
    }

    [TestMethod]
    public void UpdateGeneric()
    {
        var result = SqlServer.Update<SampleEntity>();
        Assert.IsTrue(result.Equals(
            @"UPDATE [whatever].[Sample] SET [Name]=@Name, [Description]=@Description
            WHERE [Id]=@Id"));
    }

    [TestMethod]
    public void UpdateGenericWithOverride()
    {
        var result = SqlServer.Update<SampleEntity>("dbo.Hello");
        Assert.IsTrue(result.Equals(
            @"UPDATE [dbo].[Hello] SET [Name]=@Name, [Description]=@Description
            WHERE [Id]=@Id"));
    }

    [TestMethod]
    public void DeleteGeneric() 
    {
        var result = SqlServer.Delete<SampleEntity>();
        Assert.IsTrue(result.Equals("DELETE [whatever].[Sample] WHERE [Id]=@Id"));
    }
}