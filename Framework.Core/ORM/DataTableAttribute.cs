using Framework.Core.Abstractions;
using System;

namespace Framework.Core.ORM
{
    public class DataTableAttribute : Attribute, IAttributeName
    {
        public DataTableAttribute(string tableName)
        {
            this.TableName = tableName;
        }

        public string TableName { get; }

        public string GetName()
        {
            return TableName;
        }
    }
}
