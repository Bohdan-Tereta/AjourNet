using System.Web;
using System.Web.Optimization;

namespace AjourBT
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region jquery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));
            #endregion

            #region Modernizr
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            #endregion

            #region DataTables

            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include(
                        "~/Scripts/DataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/columnFilter").Include(
                        "~/Scripts/columnFilter.js"));
                        
            #endregion

            #region fn.gantt

            bundles.Add(new ScriptBundle("~/bundles/fngantt").Include(
                        "~/Scripts/jquery.fn.gantt.js"));
            
            #endregion

            #region Custom Validations

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/DatesValidation").Include(
                        "~/Scripts/AjourBTScript/DatesValidation.js"));

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/RequiredIfValidation").Include(
                        "~/Scripts/AjourBTScript/RequiredIfValidation.js"));

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/ABMDatesValidation").Include(
                        "~/Scripts/AjourBTScript/ABMDatesValidation.js"));
                        
            #endregion

            #region VU bundles

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/CalculateWeek").Include(
                        "~/Scripts/AjourBTScript/CalculateWeek.js"));

            #endregion

            #region Help bundles

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/helpLink").Include(
                       "~/Scripts/AjourBTScript/helpLink.js"));


            #endregion

            #region AjouBTScripts bundles

            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/EmpScript").Include("~/Scripts/AjourBTScript/EmpScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/DepScript").Include("~/Scripts/AjourBTScript/DepScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/LocScript").Include("~/Scripts/AjourBTScript/LocScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/PermitScript").Include("~/Scripts/AjourBTScript/PermitScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/VisaScript").Include("~/Scripts/AjourBTScript/VisaScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForAcc").Include("~/Scripts/AjourBTScript/BTsForAcc.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForBTM").Include("~/Scripts/AjourBTScript/BTsForBTM.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForADM").Include("~/Scripts/AjourBTScript/BTsForADM.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BusinessTripForDirector").Include("~/Scripts/AjourBTScript/BusinessTripForDirector.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForVU").Include("~/Scripts/AjourBTScript/BTsForVU.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/PTsForBTM").Include("~/Scripts/AjourBTScript/PTsForBTM.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/PassportScript").Include("~/Scripts/AjourBTScript/PassportScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/PositionScript").Include("~/Scripts/AjourBTScript/PositionScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/UnitScript").Include("~/Scripts/AjourBTScript/UnitScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/OnlineUsers").Include("~/Scripts/AjourBTScript/OnlineUsers.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForEMP").Include("~/Scripts/AjourBTScript/BTsForEMP.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/JourneyScript").Include("~/Scripts/AjourBTScript/JourneyScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/CountryScript").Include("~/Scripts/AjourBTScript/CountryScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/HolidayScript").Include("~/Scripts/AjourBTScript/HolidayScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/OvertimeScript").Include("~/Scripts/AjourBTScript/OvertimeScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/CRUD_CalendarItems").Include("~/Scripts/AjourBTScript/CRUD_CalendarItems.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/GreetingScript").Include("~/Scripts/AjourBTScript/GreetingScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/ResetPasswordScript").Include("~/Scripts/AjourBTScript/ResetPasswordScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/VUchangeDateFormat").Include("~/Scripts/AjourBTScript/VUchangeDateFormat.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/InsuranceScript").Include("~/Scripts/AjourBTScript/InsuranceScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/SaveSortAfterTableRefresh").Include("~/Scripts/AjourBTScript/SaveSortAfterTableRefresh.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/UserScript").Include("~/Scripts/AjourBTScript/UserScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/BTsForPU").Include("~/Scripts/AjourBTScript/BTsForPU.js"));
            bundles.Add(new ScriptBundle("~/bundles/AjourBTScript/HRScript").Include("~/Scripts/AjourBTScript/HRScript.js"));
            bundles.Add(new ScriptBundle("~/bundles/knockout").Include("~/Scripts/knockout-{version}.debug.js")); 

            #endregion

            #region Styles

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/site.css",
                    "~/Content/PagedList.css",
                    "~/Content/style.css"));
            
            bundles.Add(new StyleBundle("~/Content/themes/base/jquery-ui").Include("~/Content/themes/base/jquery-ui.css"));
           
            bundles.Add(new StyleBundle("~/Content/redmond/jquery-ui").Include("~/Content/redmond/jquery-ui.css"));

            #endregion

            //BundleTable.EnableOptimizations = true;
        }
    }
}