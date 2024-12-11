namespace Framework.Core.Abstractions
{
    public interface ISqlBuilderPartition
    {
        string GetInsertSql<T>(T model) where T:PartitionModelBase;

        string GetInsertAndReturnSql<T>(T model) where T : PartitionModelBase;

        string GetSelectByKeySql<T>(T model) where T : PartitionModelBase;

        string GetUpdateSql<T>(T model) where T : PartitionModelBase;

        string GetUpdateAndReturnSql<T>(T model) where T : PartitionModelBase;

        string GetDeleteByKey<T>(T model) where T : PartitionModelBase;

        string GetUpdateSql<T>(string group, T model) where T:PartitionModelBase;

        string GetUpdateAndReturnSql<T>(string group, T model) where T:PartitionModelBase;
        string GetInsertSql<T>(string group, T model) where T : PartitionModelBase;
        string GetInsertAndReturnSql<T>(string group, T model) where T : PartitionModelBase;

        string GetSelectByKeySql<T1>(T1 model, string group) where T1 : PartitionModelBase;

    }
}
