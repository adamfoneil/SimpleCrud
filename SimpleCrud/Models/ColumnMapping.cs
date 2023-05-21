namespace SimpleCrud.Models;

public class ColumnMapping
{
    public string ColumnName { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public bool ForInsert { get; set; }
    public bool ForUpdate { get; set; }  
    public bool IsKey { get; set; }
}
