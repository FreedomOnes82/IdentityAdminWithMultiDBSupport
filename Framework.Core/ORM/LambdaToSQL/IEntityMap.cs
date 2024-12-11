using System.Reflection;

namespace Framework.Core.ORM.LambdaToSQL
{
    public interface IEntityMap
    {
        void SetTableName(string tableName);
        Type Type();
        string GetTableName();
        string Name();
    }

    public interface IPropertyMap
    {
        void SetColumnName(string columnName);
        PropertyInfo Type();
        string GetColumnName();
    }
}