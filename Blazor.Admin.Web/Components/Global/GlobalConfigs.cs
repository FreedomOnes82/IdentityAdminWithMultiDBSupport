using Blazor.Admin.Web.Locales;
using Framework.Core.Blazor.Admin.SqlServer.Logs.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Text.RegularExpressions;
using Blazor.Admin.Web.Models;
using System;
namespace Blazor.Admin.Web.Components.Global
{
    public class GlobalConfigs
    {
        private readonly SessionStorageService _sessionStorage;
        private readonly LocalStorageService _localStorage;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<GlobalConfigs> _logger;
        private readonly IStringLocalizer<Resource> _localizer;
        public GlobalConfigs(SessionStorageService sessionStorage, IAuditLogService auditLogService,
            ILogger<GlobalConfigs> logger, IStringLocalizer<Resource> localizer, LocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _auditLogService = auditLogService;
            _logger = logger;
            _localizer = localizer;
            //config the default tab
            //ResetTabs();
        }
        public static string SeletcedTabKey => "SeletcedTabKey";
        public static string TabsUriKey => "TabsUriKey";
        //public static string TabsKey => "TabsKey";
        public static string UserClaimsKey => "UserClaimsKey";
        public static string UserNameKey => "UserNameKey";
        public static string UserIndentityKey => "UserIndentityKey";
        public static string CurrentCultureNameKey => "CurrentCultureNameKey";


        private string _selectedTabUri = "dashboard";
        //private List<TabModel> _tabs { get; set; } = new List<TabModel>();
        private List<string> _tabUriList { get; set; } = new List<string>();
        private List<string> _userClaims { get; set; } = new List<string>();
        private string _userName { get; set; } = string.Empty;
        private string _currentCultureName { get; set; } = string.Empty;
        private string _userIndentity { get; set; } = string.Empty;
        public string UserIndentity { get => _userIndentity; set { _userIndentity = value; UpdateSessionStorage(UserIndentityKey, value); } }
        public string UserName { get => _userName; set { _userName = value; UpdateSessionStorage(UserNameKey, value); } }
        public string CurrentCultureName { get => _currentCultureName; set { _currentCultureName = value; UpdateLocalStorage(CurrentCultureNameKey, value); } }

        public void ResetTabHeaders()
        {//on reset tabs or after page exception
            _tabUriList.Clear();
            _tabUriList.Add("dashboard");
            TabUriList = _tabUriList;
        }
        public string SelectedTabUri { get => _selectedTabUri; set { _selectedTabUri = value; UpdateSessionStorage(SeletcedTabKey, value); } }
        private async void UpdateSessionStorage(string key, object value)
        {
            await _sessionStorage.SetItemAsync(key, value);
        }
        private async void UpdateLocalStorage(string key, object value)
        {
            await _localStorage.SetItemAsync(key, value);
        }
        private static List<TabModel> Tabs { get; set; } = new List<TabModel>();

        private List<string> TabUriList { get => _tabUriList; set { _tabUriList = value; UpdateSessionStorage(TabsUriKey, value); } }

        private List<string> UserClaims { get => _userClaims.Distinct().ToList(); set { _userClaims = value; UpdateSessionStorage(UserClaimsKey, value); } }
        public async Task InitGlobalSessionData()
        {
            _tabUriList = (await _sessionStorage.GetItemAsync<List<string>>(TabsUriKey)) ?? new List<string>();
            if (_tabUriList.Count == 0)
            {//on first loading...
                ResetTabs();
            }
            _selectedTabUri = (await _sessionStorage.GetItemAsync<string>(SeletcedTabKey)) ?? string.Empty;
            _userClaims = (await _sessionStorage.GetItemAsync<List<string>>(UserClaimsKey)) ?? new List<string>();
            _userName = (await _sessionStorage.GetItemAsync<string>(UserNameKey)) ?? string.Empty;
            _userIndentity = (await _sessionStorage.GetItemAsync<string>(UserIndentityKey)) ?? string.Empty;
            //_currentCultureName = (await _sessionStorage.GetItemAsync<string>(CurrentCultureNameKey)) ?? "en-US";
        }
        public async Task InitCulture()
        {
            _currentCultureName = (await _localStorage.GetItemAsync<string>(CurrentCultureNameKey)) ?? "en-US";
        }

        #region Tabs
        public List<TabModel> GetAllTabs()
        {
            return Tabs;
        }
        public List<TabModel> GetTabs()
        {
            return Tabs
                .Where(x => TabUriList.Contains(x.Uri))
                .OrderBy(x => TabUriList.IndexOf(x.Uri))
                .ToList();
        }
        public void AddTab(string uri, string header, string iconClass = null, object parameters = null)
        {
            var para = AnonymousObjectToDictionary(parameters);
            var tab = Tabs.FirstOrDefault(m => m.Uri == uri || m.HeaderFun() == _localizer[header]);
            if (tab != null)
            {
                tab.Content = GetContent(uri, para);
            }
            else
            {
                Tabs.ForEach(tab => tab.Content = null);
                Tabs.Add(new TabModel { HeaderFun = () => { return _localizer[header].ToString(); }, IconClass = string.IsNullOrEmpty(iconClass) ? string.Empty : iconClass, Uri = uri, Content = GetContent(uri, para), Parameters = parameters });
            }
            _tabUriList.Insert(1, uri);
            //_tabUriList.Add(uri);
            SelectedTabUri = uri;
            TabUriList = _tabUriList.Distinct().ToList();
        }

        public void ResetTabs()
        {
            ResetTabHeaders();
            Tabs.Clear();
            Tabs.Add(
                    new TabModel
                    {
                        HeaderFun = () => { return _localizer["Menu.Dashboard"].ToString(); },
                        Uri = "dashboard",
                        IconClass = "fa fa-home",
                        Content = builder =>
                        {
                            builder.OpenComponent<Blazor.Admin.Web.Components.Pages.ManagePage.Dashboard>(0);
                            builder.CloseComponent();
                        }
                    });
            //SelectedTabHeadText = "Dashboard";
        }
        public void ActivateTabContent(string uri)
        {
            var targetTab = Tabs.FirstOrDefault(x => x.Uri == uri);
            SelectedTabUri = uri;
            if (targetTab != null)
            {
                var para = new Dictionary<string, object>();
                if (targetTab.Parameters != null)
                    para = AnonymousObjectToDictionary(targetTab.Parameters);
                targetTab.Content = GetContent(uri, para);
            }
        }
        public string DeleteTab(string deleteTabUri)
        {//final return the latest tab route
            int index = -1;
            string newSelectedUri = string.Empty;
            string returnUri = string.Empty;
            // Find the index of the item to delete and determine the new selected header

            index = _tabUriList.IndexOf(deleteTabUri);
            if (deleteTabUri == _selectedTabUri)
            {
                if (index == _tabUriList.Count - 1 && _tabUriList.Count > 1)
                {
                    newSelectedUri = _tabUriList[_tabUriList.Count - 2];
                    var newSelectedTab = Tabs.FirstOrDefault(x => x.Uri == newSelectedUri);
                    if (newSelectedTab != null)
                        returnUri = newSelectedTab.Uri;
                }
                else //if(index < _tabHeaders.Count - 1 && _tabHeaders.Count > 1)
                {
                    newSelectedUri = _tabUriList[index + 1];
                    var newSelectedTab = Tabs.FirstOrDefault(x => x.Uri == newSelectedUri);
                    if (newSelectedTab != null)
                        returnUri = newSelectedTab.Uri;
                }
            }
            if (index >= 0)
            {
                // Remove the item from both lists
                //Tabs.RemoveAt(index);
                _tabUriList.Remove(deleteTabUri);

                // Update the selected header text if necessary
                if (!string.IsNullOrEmpty(newSelectedUri))
                {
                    _selectedTabUri = newSelectedUri;
                }

                // Rebuild the distinct list of headers only if it has changed
                TabUriList = _tabUriList.Distinct().ToList();
            }
            return returnUri;
        }
        #region GetPageContent(RenderFragment)

        public static Dictionary<string, object> AnonymousObjectToDictionary(object anonymousObject)
        {
            if (anonymousObject == null)
                return null;
            var dictionary = new Dictionary<string, object>();
            var properties = anonymousObject.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(anonymousObject);
                dictionary[property.Name] = value;
            }
            return dictionary;
        }
        private RenderFragment GetContent(string route, Dictionary<string, object> para)
        {
            var componentType = GetType("/" + route);
            return builder =>
            {
                var componentInstance = (IComponent)Activator.CreateInstance(componentType);
                builder.OpenComponent(0, componentType);
                if (para != null)
                {
                    int attributeIndex = 1;
                    foreach (var kvp in para)
                    {
                        var propInfo = componentType.GetProperty(kvp.Key);
                        if (propInfo != null && propInfo.CanWrite)
                        {
                            builder.AddAttribute(attributeIndex++, kvp.Key, kvp.Value);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Property '{kvp.Key}' does not exist on component '{componentType.Name}'.");
                        }
                    }
                }
                builder.CloseComponent();
            };
        }
        private System.Type GetType(string route)
        {
            var assembly = typeof(Program).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                var routeAttr = type.GetCustomAttribute(typeof(RouteAttribute));
                if (routeAttr != null)
                {
                    string routePath = ((RouteAttribute)routeAttr).Template;
                    string pattern = routePath.Replace(Regex.Escape("{"), "{").Replace("}", "}");
                    pattern = Regex.Replace(pattern, @"\{[^}]*\}", ".+");

                    if (Regex.IsMatch(route, "^" + pattern + "$"))
                    {
                        return type;
                    }
                    //Console.WriteLine($"Component {type.Name} is mapped to route '{routePath}'");
                }
            }
            throw new Exception($"Could not found this route for URI: '{route}'.");
        }
        #endregion GetPageContent(RenderFragment)
        #endregion Tabs

        #region UserClaims
        public List<string> GetUserClaims()
        {
            return _userClaims;
        }
        public void AddUserClaim(string claim)
        {
            if (_userClaims.Any(x => x == claim)) return;
            _userClaims.Add(claim);
            UserClaims = _userClaims;
        }
        public void ResetUserClaims()
        {
            _userClaims.Clear();
            UserClaims = _userClaims;
        }
        #endregion UserClaims

        #region AuditLog
        public async Task NewAuditlog(string message, bool isSuccess = true)
        {
            try
            {
                await _auditLogService.InsertAsync(new Framework.Core.Blazor.Admin.SqlServer.Logs.DataModels.AuditLog
                {
                    UserName = UserName,
                    CreatedBy = UserIndentity,
                    LastModifiedBy = UserIndentity,
                    Message = message,
                    Status = isSuccess ? 1 : 0,
                    TimeStamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("New Audio error: '" + message + "' with " + isSuccess + " status;" + ex.Message);
            }
        }
        #endregion AuditLog
    }

    public static class GlobalSettings
    {
        public static string DefaultRoute = "/";
        public static string ErrorRoute = "/Exception/Error";
        public static string NoFoundRoute = "/Exception/NoFound";
        public static string NotAuthorizedRoute = "/Exception/NotAuthorized";

        public static string DefaultUserPassword = "Abcd123.";
        public static string DefaultAdminUserName = "admin";
        public static string DefaultAdminUserEmail = "admin@admin.com";
        public static string DefaultAdminPassword = "Qwer!234";

        public static string NameRegularExpression = @"^[a-zA-Z0-9_]{3,50}$";
        public static string EmailRegularExpression = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public static readonly List<System.Security.Claims.Claim> DefaultClaims =
         new List<System.Security.Claims.Claim>
         {
            new System.Security.Claims.Claim("Permission", "View Users"),
            new System.Security.Claims.Claim("Permission", "Create Users"),
            new System.Security.Claims.Claim("Permission", "Update Users"),
            new System.Security.Claims.Claim("Permission", "Delete Users"),
            new System.Security.Claims.Claim("Permission", "Manage User Claims"),
            new System.Security.Claims.Claim("Permission", "View Roles"),
            new System.Security.Claims.Claim("Permission", "Create Roles"),
            new System.Security.Claims.Claim("Permission", "Update Roles"),
            new System.Security.Claims.Claim("Permission", "Delete Roles"),
            new System.Security.Claims.Claim("Permission", "Manage Role Claims"),
            new System.Security.Claims.Claim("Permission", "View System Logs"),
             new System.Security.Claims.Claim("Permission", "View Audit Logs")
         };
    }
}
