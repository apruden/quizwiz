using System.Collections.Generic;
using System.Web.Optimization;

namespace QuizWiz
{
    public class BundleConfig
    {
        class NonOrderingBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            var ckBundle = new ScriptBundle("~/bundles/ckeditor")
                .Include("~/Scripts/ckeditor/ckeditor.js")
                .Include("~/Scripts/ckeditor/adapters/jquery.js");

            ckBundle.Orderer = new NonOrderingBundleOrderer();

            bundles.Add(ckBundle);
             
            bundles.Add(new ScriptBundle("~/bundles/ko").Include(
                        "~/Scripts/knockout-*", "~/Scripts/sammy-*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}