using System;
using System.Collections.Generic;
using System.Reflection;
using DbUp;
using DbUp.Helpers;

namespace Framework.Core.Blazor.Admin.SqlServer.DatabaseScriptConfig
{
    public class Migrations : IAdminMigration
    {
        private readonly string _connectionString;
        private readonly string _schema;

        public Migrations(DBProviderOptions options)
        {
            _schema = options.DbSchema;
            _connectionString = options.ConnectionString;
        }

        public Migrations(string connectionString, string schema)
        {
            _schema = schema;
            _connectionString = connectionString;
        }

        public bool UpgradeIdentityDatabase()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Trying to upgrade identity database");
            Console.ResetColor();
            EnsureDatabase.For.SqlDatabase(_connectionString);
            var fullSuccess = true;
            var result = UpgradeDatabase(_connectionString, _schema, "IndentityDbScripts");
            if (result == -1)
                fullSuccess = false;

            return fullSuccess;
        }
        public bool UpgradeIdentityAndAuditDatabase()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Trying to upgrade auditlog database");
            Console.ResetColor();
            EnsureDatabase.For.SqlDatabase(_connectionString);
            var fullSuccess = 0;
            var identityRes = UpgradeDatabase(_connectionString, _schema, "IndentityDbScripts");
            if (identityRes != -1) fullSuccess += 1;
            var logRes = UpgradeDatabase(_connectionString, _schema, "AuditLogDbScripts");
            if (logRes != -1) fullSuccess += 1;
            if (fullSuccess == 2) return true;
            return false;
        }

        public static int EnsureSchema(string connectionString, string schema)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Preparing to upgrade {schema}");
            var variableSubstitutions = new Dictionary<string, string>
            {
                { "schemaname", $"{schema}" }
            };

            Console.ResetColor();
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains("EveryRun"))
                .WithVariable("schemaname", $"{schema}")
                .JournalTo(new NullJournal())
                .WithTransaction()
                .LogToConsole();

            var upgrader = upgradeEngine.Build();
            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
#if DEBUG
                    Console.ReadLine();
#endif
                    return -1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }

        private static int UpgradeDatabase(string connectionString, string schema, string scriptFolder)
        {
            EnsureSchema(connectionString, schema);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Preparing to upgrade {schema}");
            var variableSubstitutions = new Dictionary<string, string>
            {
                { "schemaname", $"{schema}" }
            };

            Console.ResetColor();
            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains(scriptFolder))
                .WithVariable("schemaname", $"{schema}")
                .JournalToSqlTable(schema, "SchemaVersions")
                .WithTransaction()
                .LogToConsole();

            var upgrader = upgradeEngine.Build();
            if (upgrader.IsUpgradeRequired())
            {
                var result = upgrader.PerformUpgrade();
                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
#if DEBUG
                    Console.ReadLine();
#endif
                    return -1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();

            return 0;
        }
    }
}