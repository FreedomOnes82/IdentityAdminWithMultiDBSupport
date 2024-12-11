namespace Framework.Core.Blazor.Admin.MySql.DatabaseScriptConfig
{
    public interface IAdminMigration
    {
        bool UpgradeIdentityDatabase();
        bool UpgradeIdentityAndAuditDatabase();
    }
}