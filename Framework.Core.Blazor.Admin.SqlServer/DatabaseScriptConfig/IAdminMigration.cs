namespace Framework.Core.Blazor.Admin.SqlServer.DatabaseScriptConfig
{
    public interface IAdminMigration
    {
        bool UpgradeIdentityDatabase();
        bool UpgradeIdentityAndAuditDatabase();
    }
}