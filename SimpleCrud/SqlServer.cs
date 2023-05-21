using SimpleCrud.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCrud;

public static partial class SqlServer
{
    public static string Insert(string schema, string tableName, IEnumerable<string> columnNames) =>
        Insert(schema, tableName, columnNames.Select(col => new ColumnMapping() 
        { 
            ColumnName = col, 
            ParameterName = col, 
            ForInsert = true 
        }));

    public static string Update(string schema, string tableName, IEnumerable<string> columnNames) =>
        Update(schema, tableName, columnNames.Select(col => new ColumnMapping()
        {
            ColumnName = col,
            ParameterName = col,            
            ForUpdate = !col.Equals(DefaultIdentityCol),
            IsKey = col.Equals(DefaultIdentityCol)
        }));

    public static string Delete(string schema, string tableName, IEnumerable<string> columnNames) =>
        Delete(schema, tableName, columnNames.Select(col => new ColumnMapping()
        {
            ColumnName = col,
            ParameterName = col,
            IsKey = col.Equals(DefaultIdentityCol)
        }));

    public static string Insert(string schema, string tableName, IEnumerable<ColumnMapping> columns)
    {
        if (!columns.Any(col => col.ForInsert)) throw new ArgumentException("Must have at least one insert column");

        return 
            $@"INSERT INTO [{schema}].[{tableName}] (
                {string.Join(", ", columns.Where(col => col.ForInsert).Select(col => $"[{col.ColumnName}]"))}
            ) VALUES (
                {string.Join(", ", columns.Where(col => col.ForInsert).Select(col => $"@{col.ParameterName}"))}
            ); SELECT SCOPE_IDENTITY()";
    }
        

    public static string Update(string schema, string tableName, IEnumerable<ColumnMapping> columns)
    {
        if (!columns.Any(col => col.IsKey)) throw new ArgumentException("Must have at least one key column");
        if (!columns.Any(col => col.ForUpdate)) throw new ArgumentException("Must have at least one update column");

        return 
            $@"UPDATE [{schema}].[{tableName}] SET {string.Join(", ", columns.Where(col => col.ForUpdate).Select(SetExpression))}
            WHERE {string.Join(", ", columns.Where(col => col.IsKey).Select(SetExpression))}";
    }
        
    public static string Delete(string schema, string tableName, IEnumerable<ColumnMapping> columns)
    {
        if (!columns.Any(col => col.IsKey)) throw new ArgumentException("Must have at least one key column");

        return $"DELETE [{schema}].[{tableName}] WHERE {string.Join(" AND ", columns.Where(col => col.IsKey).Select(SetExpression))}";
    }

    private static string SetExpression(ColumnMapping columnMapping) => $"[{columnMapping.ColumnName}]=@{columnMapping.ParameterName}";
}
