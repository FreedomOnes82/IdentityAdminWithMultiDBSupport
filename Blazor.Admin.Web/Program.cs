using Autofac.Core;
using Blazor.Admin.Web.Components;
using Framework.Core.Abstractions;
using Framework.Core.Blazor.Admin.SqlServer.Auth;
using Framework.Core.Blazor.Admin.SqlServer.DatabaseScriptConfig;
using Framework.Core.Blazor.Admin.SqlServer.Auth.Models;
using Framework.Core.Blazor.Admin.SqlServer.Logs;
using Framework.Core.Extensions;
using Framework.Core.UOW;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Serilog;
using OfficeOpenXml;
using Blazor.Admin.Web.Components.Global;
ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddLogging(loggingBuilder =>
    loggingBuilder.AddSerilog(dispose: true));

builder.Services.AddScoped<GlobalConfigs>();
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<LocalStorageService>();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

/*
            Configuration omitted
            */
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
}).AddDefaultTokenProviders()
.AddDapperStores();

//These code use to setup database, but seems some error here ...
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=.;database=identityDB;User id=sa;password=! 123abcd;TrustServerCertificate=true";
builder.Services.AddAdminDbUpDatabaseScripts(options =>
{
    options.ConnectionString = connectionString;
    options.DbSchema = "my schema";
});
//connectionString = "Server=.;database=TestBlazor;User id=sa;password=! 123abcd;TrustServerCertificate=true";
var schema = "dbo";
var adminMigrations = new Framework.Core.Blazor.Admin.SqlServer.DatabaseScriptConfig.Migrations(connectionString, schema);
var adminResult = adminMigrations.UpgradeIdentityAndAuditDatabase();

builder.Services.AddAuditLogDependencyInjection();
builder.Services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.UseSqlServerDataAccessor();
//builder.Services.AddTransient<IDbConnectionAccessor, MySqlDataAccessor>();
builder.Services.UseDefaultDataTablePrefixProvider();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<ProtectedSessionStorage>();

builder.Services.AddLocalization();
builder.Services.AddControllers();


var app = builder.Build();
string[] supportedCultures = ["en-US", "zh-CN"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[1])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
