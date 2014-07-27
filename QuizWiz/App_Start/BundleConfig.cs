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
            bundles.UseCdn = true;
            
            bundles.Add(new ScriptBundle("~/bundles/jqueryui", "//ajax.googleapis.com/ajax/libs/jqueryui/1.11.0/jquery-ui.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery", "//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval", "//cdnjs.cloudflare.com/ajax/libs/jquery-validate/1.11.1/jquery.validate.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/ckeditor", "//cdn.ckeditor.com/4.4.3/standard/ckeditor.js"));

            //TODO: enable only if needed
            //var ckBundle = new ScriptBundle("~/bundles/ckeditor_jquery")
            //    .Include("~/Scripts/ckeditor/adapters/jquery.js");
            //ckBundle.Orderer = new NonOrderingBundleOrderer();
            //bundles.Add(ckBundle);

            bundles.Add(new ScriptBundle("~/bundles/ko", "//cdnjs.cloudflare.com/ajax/libs/knockout/3.1.0/knockout-min.js"));
            bundles.Add(new ScriptBundle("~/bundles/sammy", "//cdnjs.cloudflare.com/ajax/libs/sammy.js/0.7.4/sammy.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr", "//cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.2/modernizr.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap", "//maxcdn.bootstrapcdn.com/bootstrap/3.2.0/js/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/respond", "//cdnjs.cloudflare.com/ajax/libs/respond.js/1.4.2/respond.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap_css", "//maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}