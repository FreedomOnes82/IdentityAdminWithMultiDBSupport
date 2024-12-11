using Framework.Core.Abstractions;

namespace Framework.Core.ORM
{
    public class SqlBuilderFactory
    {
        public enum DBTypes
        { 
            None,
            SQLITE,
            SQLSERVER,
            MYSQL
        }

        public static ISqlBuilder<T> CreateInstance<T>(DBTypes dbType, 
            IDataTablePrefixProvider dataTablePrefixProvider = null,
            IUpdateCommandGroupProvider updateCmdGroupProvider= null
            ) where T:class 
        {
            switch (dbType)
            {
                case DBTypes.SQLITE:
                    return new SqlBuilder_Sqlite<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                case DBTypes.SQLSERVER:
                    return new SqlBuilder_MsSql<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                case DBTypes.MYSQL:
                    return new SqlBuilder_MySql<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                default:
                    return null;
            }
        }


        public static ISqlBuilderPartition CreatePartitionInstance<T>(DBTypes dbType, 
            IDataTablePrefixProvider dataTablePrefixProvider = null,
            IUpdateCommandGroupProvider updateCmdGroupProvider = null) where T : class
        {
            switch (dbType)
            {
                case DBTypes.SQLITE:
                    return new SqlBuilder_Sqlite<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                case DBTypes.SQLSERVER:
                    return new SqlBuilder_MsSql<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                case DBTypes.MYSQL:
                    return new SqlBuilder_MySql<T>(dataTablePrefixProvider, updateCmdGroupProvider);
                default:
                    return null;
            }
        }
    }
}
