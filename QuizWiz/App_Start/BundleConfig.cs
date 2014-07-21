namespace QuizWiz
{
    using System.Collections.Generic;
    using System.Web.Optimization;

    /// <summary>
    /// 
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// 
        /// </summary>
        class NonOrderingBundleOrderer : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundles"></param>
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

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            var angularBundle = new ScriptBundle("~/bundles/angular")
                .Include("~/Scripts/angular.js")
                .Include("~/Scripts/angular-route.js")
                .Include("~/Scripts/angular-loader.js");

            angularBundle.Orderer = new NonOrderingBundleOrderer();

            bundles.Add(angularBundle);

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}