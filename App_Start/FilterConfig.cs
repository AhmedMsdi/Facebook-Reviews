using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Atreemo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
