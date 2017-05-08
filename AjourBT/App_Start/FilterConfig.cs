using AjourBT.Filters;
using System.Web;
using System.Web.Mvc;

namespace AjourBT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new DisableCache());
        }
    }
}