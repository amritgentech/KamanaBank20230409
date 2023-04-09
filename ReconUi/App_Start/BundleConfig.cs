using System.Web;
using System.Web.Optimization;

namespace ReconUi
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/Moment.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //                      "~/Content/bootstrap.css",
                      //                      "~/Content/bootstrap-datetimepicker.css",
                      //                      "~/Content/site.css"));
                      "~/Content/Proj/Custom.css"));
            //edit here
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Scripts/DataTables/jquery.dataTables.min.js",
                        "~/Scripts/DataTables/dataTables.buttons.js",
                        "~/Scripts/DataTables/buttons.html5.js",
                        "~/Scripts/DataTables/buttons.print.js",
                        "~/Scripts/DataTables/dataTables.bootstrap.js",
                        //new for report add
            "~/Content/DataTables/buttons.colVis.min.js",
            "~/Content/DataTables/dataTables.bootstrap.js",
            "~/Content/DataTables/jszip.min.js"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
                      "~/Content/DataTables/css/dataTables.bootstrap.css",
                      //new for report add
                      "~/Content/DataTables/css/jquery.dataTables.css"));

            /* Jquery for user added jquery files */
            bundles.Add(new ScriptBundle("~/bundles/summary").Include(
                      "~/Scripts/proj/summary.js"));

            //bundles.Add(new ScriptBundle("~/bundles/demo").Include(
            //           "~/assets/js/chartlist.min.js",
            //           "~/assets/js/paper-dashboard.js",
            //           "~/assets/js/demo.js"));
            //            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //                        "~/Scripts/jquery-{version}.js"));
            //
            //            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //                        "~/Scripts/jquery.validate*"));
            //
            //            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            //            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //                        "~/Scripts/modernizr-*"));
            //
            //            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //                      "~/Scripts/bootstrap.js",
            //                      "~/Scripts/respond.js"));
            //
            //            bundles.Add(new StyleBundle("~/Content/css").Include(
            //                      "~/Content/bootstrap.css",
            //                      "~/Content/site.css"));
        }
    }
}
