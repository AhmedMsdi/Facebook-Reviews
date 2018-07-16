using Atreemo.Views.Tools;
using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace Atreemo
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build asset at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/Buttons.css"));
            string KendouiVersion = ConfigurationManager.AppSettings["kendoui-version"].ToString();

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
            "~/Scripts/kendo/" + KendouiVersion + "/kendo.all.min.js",
            "~/Scripts/kendo/" + KendouiVersion + "/kendo.kendo.core.js",
            "~/Scripts/kendo/" + KendouiVersion + "/kendo.kendo.userevents.js",
            "~/Scripts/kendo/" + KendouiVersion + "/kendo.kendo.draganddrop.js",
            "~/Scripts/kendo/" + KendouiVersion + "/kendo.aspnetmvc.min.js",
            "~/Scripts/kendo/" + KendouiVersion + "/jszip.min.js"));

            

            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
            "~/Content/kendo/" + KendouiVersion + "/kendo.common-bootstrap.min.css"
            , "~/Content/Css/AtreemoAwesome.css"
            ,"~/Content/kendo/" + KendouiVersion + "/kendo.atreemo.min.css"));

            BundleTable.EnableOptimizations = false;

            bundles.IgnoreList.Clear();

        }
    }
}
