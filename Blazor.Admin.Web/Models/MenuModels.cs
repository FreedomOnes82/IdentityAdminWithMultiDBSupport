namespace Blazor.Admin.Web.Models
{
    public class NavMenuModel
    {
        public string MenuName { get; set; } = string.Empty;
        public string MenuUrl { get; set; } = string.Empty;
        public bool IsAccessRestricted { get; set; }
        public string ClaimValue { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public List<NavMenuModel> ChildMenus { get; set; } = new List<NavMenuModel>();
        public NavMenuModel(string menuName, string menuUrl, string claimValue = null, string iconClass = null,string iconUrl=null)
        {
            MenuName = menuName;
            MenuUrl = menuUrl;
            IconClass = string.IsNullOrEmpty(iconClass) ? string.Empty : iconClass;
            IconUrl= string.IsNullOrEmpty(iconUrl) ? string.Empty : iconUrl;
            if (!string.IsNullOrEmpty(claimValue))
            {
                IsAccessRestricted = true;
                ClaimValue = claimValue;
            }
        }
        public void AddChildMenu(NavMenuModel child)
        {
            if (ChildMenus.Any(x => x.MenuName == child.MenuName || x.MenuUrl == child.MenuUrl))
            {
                Console.WriteLine("Duplicate Menu Name or Menu Url!");
                return;
            }
            else
            {
                ChildMenus.Add(child);
            }
        }
        public bool HasChild()
        {
            return ChildMenus.Count > 0;
        }
    }
}
