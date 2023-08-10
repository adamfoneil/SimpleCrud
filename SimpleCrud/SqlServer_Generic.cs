using SimpleCrud.Extensions;
using SimpleCrud.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SimpleCrud;

public static partial class SqlServer
{
    public const string DefaultIdentityCol = "Id";

    private static (string Schema, string Name) ParseTableName<T>(string? tableName = null)
    {
        if (!string.IsNullOrWhiteSpace(tableName)) return ParseSchemaAndName(tableName);

        var schema = "dbo";
        var name = typeof(T).Name;

        if (typeof(T).HasAttribute<TableAttribute>(out var tableAttr))
        {
            if (!string.IsNullOrWhiteSpace(tableAttr.Schema))
            {
                schema = tableAttr.Schema;
            }

            if (!string.IsNullOrWhiteSpace(tableAttr.Name))
            {
                name = tableAttr.Name;
            }
        }

        return (schema, name);
    }

    private static (string Schema, string Name) ParseSchemaAndName(string objectName)
    {
        var parts = objectName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        return
            (parts.Length == 1) ? ("dbo", objectName) :
            (parts.Length == 2) ? (parts[0], parts[1]) :
            throw new Exception($"Unexpected name format: {objectName}");
    }

    public static string Insert<T>(string? tableName = null, Func<PropertyInfo, ColumnMapping>? mappingConvention = null)
    {
        var mappings = GetColumnMappings<T>(mappingConvention ?? DefaultColumnMapping);
        var table = ParseTableName<T>(tableName);
        return Insert(table.Schema, table.Name, mappings);
    }

    public static string Update<T>(string? tableName = null, Func<PropertyInfo, ColumnMapping>? mappingConvention = null) 
    {
        var mappings = GetColumnMappings<T>(mappingConvention ?? DefaultColumnMapping);
        var table = ParseTableName<T>(tableName);
        return Update(table.Schema, table.Name, mappings);
    }

    public static string Delete<T>(string? tableName = null, Func<PropertyInfo, ColumnMapping>? mappingConvention = null) 
    {
        var mappings = GetColumnMappings<T>(mappingConvention ?? DefaultColumnMapping);
        var table = ParseTableName<T>(tableName);
        return Delete(table.Schema, table.Name, mappings);
    }

    private static IEnumerable<ColumnMapping> GetColumnMappings<T>(Func<PropertyInfo, ColumnMapping> mappingMethod) => 
        typeof(T).GetProperties()
        .Where(pi => !pi.HasAttribute<NotMappedAttribute>(out _))
        .Select(pi => mappingMethod.Invoke(pi));

    public static ColumnMapping DefaultColumnMapping(PropertyInfo propertyInfo)
    {
        if (propertyInfo.Name.Equals(DefaultIdentityCol)) return new()
        {
            IsKey = true,
            ForUpdate = false,
            ForInsert = false,
            ColumnName = GetColumnName(propertyInfo),
            ParameterName = propertyInfo.Name
        };

        return new()
        {
            IsKey = propertyInfo.HasAttribute<KeyAttribute>(out _),
            ForUpdate = true,
            ForInsert = true,
            ColumnName = GetColumnName(propertyInfo),
            ParameterName = propertyInfo.Name
        };

        string GetColumnName(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<ColumnAttribute>();
            return (attr is not null && !string.IsNullOrEmpty(attr.Name)) ? attr.Name : propertyInfo.Name;
        }            
    }
}
