using System.Web;
using System.Web.Optimization;

namespace II_VI_Incorporated_SCM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Content/kendo/2017.2.621/js/jquery.min.js",
            //          //"~/Content/kendo/2017.2.621/js/angular.min.js",
            //          "~/Content/kendo/2017.2.621/js/jszip.min.js",
            //          "~/Content/kendo/2017.2.621/js/kendo.all.min.js",
            //          "~/Content/kendo/2017.2.621/js/kendo.aspnetmvc.min.js",
            //          "~/Scripts/bootstrap.min.js",
            //          "~/Scripts/respond.min.js",
            //          "~/Scripts/jquery.blockUI.js",
            //          "~/Scripts/bootbox.min.js",
            //          "~/Theme/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
            //          "~/Theme/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
            //          "~/Theme/assets/global/scripts/app.min.js",
            //         "~/Theme/assets/layouts/layout6/scripts/layout.min.js",
            //          //   "~/Theme/assets/layouts/layout/scripts/layout.min.js",
            //          "~/Theme/assets/global/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js",
            //          "~/Scripts/bootstrap-datepicker.min.js"
            //          ));

            //bundles.Add(new StyleBundle("~/bundles/allcss").Include(
            //     "~/Theme/assets/global/css/FirstStyles.min.css",
            //      "~/Theme/assets/global/plugins/font-awesome/css/font-awesome.min.css",
            //       "~/Theme/assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
            //        "~/Content/bootstrap.min.css",
            //         "~/Theme/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
            //         "~/Theme/assets/global/css/components.min.css",
            //         "~/Theme//assets/layouts/layout/css/themes/darkblue.min.css",
            //         "~/Theme/assets/global/css/plugins.min.css",
            //            "~/Theme/assets/layouts/layout/css/layout.min.js",
            //               "~/Theme/assets/layouts/layout/themes/darkblue.min.css",
            //           "~/Theme/assets/layouts/layout/css/custom.min.css",
            //         "~/Content/kendo/2017.2.621/kendo.common-bootstrap.min.css",
            //         "~/Content/kendo/2017.2.621/kendo.mobile.all.min.css",
            //         "~/Content/kendo/2017.2.621/kendo.dataviz.min.css",
            //         "~/Content/kendo/2017.2.621/kendo.bootstrap.min.css",
            //         "~/Content/kendo/2017.2.621/kendo.dataviz.bootstrap.min.css",
            //         "~/Content/bootstrap-datepicker.min.css",
            //         "~/Theme/assets/global/plugins/bootstrap-multiselect/css/bootstrap-multiselect.css",
            //         "~/Content/Site.css",
            //         "~/Theme/assets/global/plugins/select2/css/select2.min.css",
            //         "~/Theme/assets/global/plugins/select2/css/select2-bootstrap.min.css"
            //        ));

            bundles.Add(new StyleBundle("~/bundles/allcssloggin").Include(
                 "~/Theme/assets/global/css/FirstStyles.min.css",
                  "~/Theme/assets/global/plugins/font-awesome/css/font-awesome.min.css",
                   "~/Theme/assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
                    "~/Content/bootstrap.min.css",
                     "~/Theme/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                     "~/Theme/assets/global/plugins/select2/css/select2.min.css",
                     "~/Theme/assets/global/plugins/select2/css/select2-bootstrap.min.css",
                     "~/Theme/assets/global/css/components.min.css",
                     "~/Theme/assets/global/css/plugins.min.css",
                     "~/Content/login-4.css",
                     "~/Content/Site.css"
                    ));


            bundles.Add(new ScriptBundle("~/bundles/bootstrapLogin").Include(
                "~/Content/kendo/2017.2.621/js/jquery.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Theme/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/Theme/assets/layouts/global/scripts/quick-sidebar.js",
                   "~/Theme/assets/layouts/global/scripts/quick-sidebar.min.js",
                       "~/Theme/assets/layouts/global/scripts/quick-nav.min.js",
                "~/Scripts/jquery.blockUI.js",
                "~/Theme/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/Theme/assets/global/plugins/select2/js/select2.full.min.js",
                "~/Theme/assets/global/plugins/backstretch/jquery.backstretch.min.js",
                "~/Theme/assets/global/scripts/app.min.js",
                "~/Scripts/login-4.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.unobtrusive*",
            "~/Scripts/jquery.validate*",
            "~/Scripts/additional-methods.min.js",
            "~/Scripts/custom-validators.js",
            "~/Theme/assets/global/plugins/select2/js/select2.min.js"));


            //tuanlua add
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                 "~/Theme/assets/global/plugins/font-awesome/css/font-awesome.min.css",
                  "~/Theme/assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
                 "~/Theme/assets/global/plugins/bootstrap/css/bootstrap.min.css",
                  "~/Theme/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                  "~/Theme/assets/global/plugins/select2/css/select2.min.css",
                     "~/Theme/assets/global/plugins/select2/css/select2-bootstrap.min.css",
                       "~/Content/kendo/2017.2.621/kendo.common-bootstrap.min.css",
                     "~/Content/kendo/2017.2.621/kendo.mobile.all.min.css",
                     "~/Content/kendo/2017.2.621/kendo.dataviz.min.css",
                     "~/Content/kendo/2017.2.621/kendo.bootstrap.min.css",
                     "~/Content/kendo/2017.2.621/kendo.dataviz.bootstrap.min.css",
                     "~/Content/bootstrap-datepicker.min.css",
                     "~/Theme/assets/global/plugins/bootstrap-multiselect/css/bootstrap-multiselect.css",
                     "~/Content/Site.css",
                     "~/Theme/assets/global/plugins/select2/css/select2.min.css",
                     "~/Theme/assets/global/plugins/select2/css/select2-bootstrap.min.css"

                   ));
            bundles.Add(new StyleBundle("~/bundles/style").Include(
          "~/Theme/assets/global/css/components.min.css",
                "~/Theme/assets/global/css/plugins.min.css"
               
                  ));
            bundles.Add(new StyleBundle("~/bundles/stylelayoutcss").Include(
               "~/Theme/assets/layouts/layout/css/layout.min.css",
                 "~/Theme/assets/layouts/layout/css/themes/light.min.css",
              "~/Theme/assets/layouts/layout/css/custom.min.css"
                  ));

            bundles.Add(new ScriptBundle("~/bundles/BEGINCOREPLUGINS").Include(            
                  "~/Content/kendo/2017.2.621/js/jquery.min.js",
                      "~/Content/kendo/2017.2.621/js/jszip.min.js",
                      "~/Content/kendo/2017.2.621/js/kendo.all.min.js",
                      "~/Content/kendo/2017.2.621/js/kendo.aspnetmvc.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js",
                      "~/Scripts/jquery.blockUI.js",
                      "~/Scripts/bootbox.min.js",
                      "~/Theme/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                      "~/Theme/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                      "~/Theme/assets/global/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js",
                      "~/Scripts/bootstrap-datepicker.min.js"
             
           ));
            bundles.Add(new ScriptBundle("~/bundles/GLOBAL SCRIPTS").Include(
            "~/Theme/assets/global/scripts/app.min.js"
         ));
            bundles.Add(new ScriptBundle("~/bundles/ENDCOREPLUGINS").Include(                                                 
                "~/Theme/assets/layouts/layout/scripts/layout.min.js",
            "~/Theme/assets/layouts/layout/scripts/demo.min.js",
              "~/Theme/assets/layouts/global/scripts/quick-sidebar.min.js",
            "~/Theme/assets/layouts/global/scripts/quick-nav.min.js"             
          ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
