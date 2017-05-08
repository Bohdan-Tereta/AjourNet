using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using AjourBT.Domain.Infrastructure;
using System.Web.Configuration;
using System.Data.Entity.Migrations;

namespace AjourBT.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                string DBInitType = System.Web.Configuration.WebConfigurationManager.AppSettings["DBInitType"].ToString();

                try
                {
                    WebSecurity.InitializeDatabaseConnection("AjourBTConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    switch (DBInitType)
                    {
                        case ("InitForTest"):
                            InitForTest_Membership();
                            break;
                        case ("InitDbClear"):
                            InitDBClear_Membership();
                            break;
                        default:
                            InitDbNotChanged_Membership();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }

            }

            public void CreateRoles()
            {
                if (!System.Web.Security.Roles.RoleExists("EMP"))
                {
                    System.Web.Security.Roles.CreateRole("EMP");
                }

                if (!System.Web.Security.Roles.RoleExists("PU"))
                {
                    System.Web.Security.Roles.CreateRole("PU");
                }

                if (!System.Web.Security.Roles.RoleExists("BTM"))
                {
                    System.Web.Security.Roles.CreateRole("BTM");
                }

                if (!System.Web.Security.Roles.RoleExists("ADM"))
                {
                    System.Web.Security.Roles.CreateRole("ADM");
                }

                if (!System.Web.Security.Roles.RoleExists("ACC"))
                {
                    System.Web.Security.Roles.CreateRole("ACC");
                }

                if (!System.Web.Security.Roles.RoleExists("DIR"))
                {
                    System.Web.Security.Roles.CreateRole("DIR");
                }

                if (!System.Web.Security.Roles.RoleExists("VU"))
                {
                    System.Web.Security.Roles.CreateRole("VU");
                }

                if (!System.Web.Security.Roles.RoleExists("ABM"))
                {
                    System.Web.Security.Roles.CreateRole("ABM");
                }

                if (!System.Web.Security.Roles.RoleExists("BDM"))
                {
                    System.Web.Security.Roles.CreateRole("BDM");
                }

                if (!System.Web.Security.Roles.RoleExists("HR"))
                {
                    System.Web.Security.Roles.CreateRole("HR");
                }
            }


            public void InitForTest_Membership()
            {
                #region initWebSecurity

                #region PU - password added

                if (!WebSecurity.UserExists("apat"))
                {
                    WebSecurity.CreateUserAndAccount("apat", "lokmop");
                }

                if (!WebSecurity.UserExists("mlan"))
                {
                    WebSecurity.CreateUserAndAccount("mlan", "iremiw");
                }

                if (!WebSecurity.UserExists("mhan"))
                {
                    WebSecurity.CreateUserAndAccount("mhan", "iremiq");
                }

                #endregion

                #region User - all rolles - password added
                if (!WebSecurity.UserExists("User"))
                {
                    WebSecurity.CreateUserAndAccount("User", "qwezxc");
                }

                #endregion

                #region briv - all rolles - password added
                if (!WebSecurity.UserExists("briv"))
                {
                    WebSecurity.CreateUserAndAccount("briv", "qwezxc");
                }

                #endregion

                #region ADMs - LMs  - password added
                if (!WebSecurity.UserExists("lgon"))
                {
                    WebSecurity.CreateUserAndAccount("lgon", "corsad");
                }

                if (!WebSecurity.UserExists("ncru"))
                {
                    WebSecurity.CreateUserAndAccount("ncru", "uwotin");
                }

                if (!WebSecurity.UserExists("cbur"))
                {
                    WebSecurity.CreateUserAndAccount("cbur", "aragez");
                }

                if (!WebSecurity.UserExists("lmor"))
                {
                    WebSecurity.CreateUserAndAccount("lmor", "iqurat");
                }

                if (!WebSecurity.UserExists("rkni"))
                {
                    WebSecurity.CreateUserAndAccount("rkni", "feharo");
                }

                if (!WebSecurity.UserExists("jhan"))
                {
                    WebSecurity.CreateUserAndAccount("jhan", "tedyuy");
                }

                if (!WebSecurity.UserExists("moph"))
                {
                    WebSecurity.CreateUserAndAccount("moph", "guhimi");
                }

                if (!WebSecurity.UserExists("maad"))
                {
                    WebSecurity.CreateUserAndAccount("maad", "regayl");
                }

                if (!WebSecurity.UserExists("dsto"))
                {
                    WebSecurity.CreateUserAndAccount("dsto", "abrowy");
                }

                #endregion

                #region BTMs - password added
                if (!WebSecurity.UserExists("wsim"))
                {
                    WebSecurity.CreateUserAndAccount("wsim", "1234rt");
                }

                if (!WebSecurity.UserExists("jton"))
                {
                    WebSecurity.CreateUserAndAccount("jton", "gillay");
                }

                if (!WebSecurity.UserExists("khal"))
                {
                    WebSecurity.CreateUserAndAccount("khal", "cicsag");
                }

                #endregion

                #region ACCs  - password added
                if (!WebSecurity.UserExists("lark"))
                {
                    WebSecurity.CreateUserAndAccount("lark", "12345t");
                }
                if (!WebSecurity.UserExists("mtuc"))
                {
                    WebSecurity.CreateUserAndAccount("mtuc", "gomyuq");
                }

                if (!WebSecurity.UserExists("jmil"))
                {
                    WebSecurity.CreateUserAndAccount("jmil", "opebik");
                }

                if (!WebSecurity.UserExists("sfis"))
                {
                    WebSecurity.CreateUserAndAccount("sfis", "yowsab");
                }

                if (!WebSecurity.UserExists("dkim"))
                {
                    WebSecurity.CreateUserAndAccount("dkim", "amicoh");
                }

                #endregion

                #region DIR - password added
                if (!WebSecurity.UserExists("carn"))
                {
                    WebSecurity.CreateUserAndAccount("carn", "gredsa");
                }

                if (!WebSecurity.UserExists("oweh"))
                {
                    WebSecurity.CreateUserAndAccount("oweh", "ugyane");
                }
                #endregion

                #region VU  - password added
                if (!WebSecurity.UserExists("ayou"))
                {
                    WebSecurity.CreateUserAndAccount("ayou", "123456");
                }

                #endregion

                #region EMP- password added
                if (!WebSecurity.UserExists("ealv"))
                {
                    WebSecurity.CreateUserAndAccount("ealv", "654321");
                }
                #endregion

                #region ABM  - password added
                if (!WebSecurity.UserExists("tmas"))
                {
                    WebSecurity.CreateUserAndAccount("tmas", "aserty");
                }

                #endregion

                #region BDM  - password added
                if (!WebSecurity.UserExists("sdea"))
                {
                    WebSecurity.CreateUserAndAccount("sdea", "trew21");
                }

                #endregion 

                #region HR  - password added
                if (!WebSecurity.UserExists("mter"))
                {
                    WebSecurity.CreateUserAndAccount("mter", "654321");
                }

                #endregion


                CreateRoles();

                #region briv - all rolles - rolles added 

                if (!System.Web.Security.Roles.IsUserInRole("briv", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "VU" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "ADM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "BTM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "ACC" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "PU" });
                } 

                if (!System.Web.Security.Roles.IsUserInRole("briv", "DIR"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "DIR" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "EMP"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "EMP" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "ABM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "ABM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "BDM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "BDM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("briv", "HR"))
                {
                    System.Web.Security.Roles.IsUserInRole("briv", "HR");
                } 

                #endregion

                #region User - all rolles - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("User", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "VU" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "ADM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "BTM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "ACC" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "DIR"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "DIR" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "PU" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "EMP"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "EMP" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "ABM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "ABM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("User", "BDM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("User", new string[] { "BDM" });
                }

                if(!System.Web.Security.Roles.IsUserInRole("User", "HR"))
                {
                    System.Web.Security.Roles.AddUserToRoles("briv", new string[] { "HR" });
                }

                #endregion

                #region PU - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("apat", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRole("apat", "PU");
                }

                if (!System.Web.Security.Roles.IsUserInRole("mlan", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRole("mlan", "PU");
                }

                if (!System.Web.Security.Roles.IsUserInRole("mlan", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("mlan", "ADM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("mhan", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRole("mhan", "PU");
                }

                if (!System.Web.Security.Roles.IsUserInRole("mhan", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("mhan", "ADM");
                }
                #endregion

                #region ADM(LMs) - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("lgon", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("lgon", "ADM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("ncru", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("ncru", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("ncru", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("ncru", "VU");
                }



                if (!System.Web.Security.Roles.IsUserInRole("dsto", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("dsto", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("dsto", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("dsto", "VU");
                }



                if (!System.Web.Security.Roles.IsUserInRole("cbur", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("cbur", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("cbur", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("cbur", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("rkni", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("rkni", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "VU");
                }
                if (!System.Web.Security.Roles.IsUserInRole("rkni", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "BTM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("rkni", "DIR"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "DIR");
                }
                if (!System.Web.Security.Roles.IsUserInRole("rkni", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "ACC");
                }
                if (!System.Web.Security.Roles.IsUserInRole("rkni", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "PU");
                }

                if (!System.Web.Security.Roles.IsUserInRole("rkni", "ABM"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "ABM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("rkni", "EMP"))
                {
                    System.Web.Security.Roles.AddUserToRole("rkni", "EMP");
                }

                if (!System.Web.Security.Roles.IsUserInRole("rkni", "BDM"))
                {
                    System.Web.Security.Roles.AddUserToRoles("rkni", new string[] { "BDM" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("rkni", "HR"))
                {
                    System.Web.Security.Roles.AddUserToRoles("rkni", new string[] { "HR" });
                }

                if (!System.Web.Security.Roles.IsUserInRole("lmor", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("lmor", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("lmor", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("lmor", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("jhan", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("jhan", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("jhan", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("jhan", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("moph", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("moph", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("moph", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("moph", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("maad", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("maad", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("maad", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("maad", "VU");
                }

                #endregion

                #region BTMs - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("wsim", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRole("wsim", "BTM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("jton", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRole("jton", "BTM");
                }

                if (!System.Web.Security.Roles. IsUserInRole("jton", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("jton", "ADM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("jton", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("jton", "VU");
                }

                if (!System.Web.Security.Roles.IsUserInRole("khal", "BTM"))
                {
                    System.Web.Security.Roles.AddUserToRole("khal", "BTM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("khal", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("khal", "ADM");
                }

                if (!System.Web.Security.Roles.IsUserInRole("khal", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("khal", "VU");
                }

                #endregion

                #region ACCs - rolles added
                if (!System.Web.Security.Roles.IsUserInRole("lark", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("lark", "ACC");
                }

                if (!System.Web.Security.Roles.IsUserInRole("mtuc", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("mtuc", "ACC");
                }
                if (!System.Web.Security.Roles.IsUserInRole("mtuc", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("mtuc", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("mtuc", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("mtuc", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("jmil", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("jmil", "ACC");
                }
                if (!System.Web.Security.Roles.IsUserInRole("jmil", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("jmil", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("jmil", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("jmil", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("sfis", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("sfis", "ACC");
                }
                if (!System.Web.Security.Roles.IsUserInRole("sfis", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("sfis", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("sfis", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("sfis", "VU");
                }


                if (!System.Web.Security.Roles.IsUserInRole("dkim", "ACC"))
                {
                    System.Web.Security.Roles.AddUserToRole("dkim", "ACC");
                }
                if (!System.Web.Security.Roles.IsUserInRole("dkim", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("dkim", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("dkim", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("dkim", "VU");
                }


                #endregion

                #region DIR - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("carn", "DIR"))
                {
                    System.Web.Security.Roles.AddUserToRole("carn", "DIR");
                }

                if (!System.Web.Security.Roles.IsUserInRole("oweh", "DIR"))
                {
                    System.Web.Security.Roles.AddUserToRole("oweh", "DIR");
                }
                if (!System.Web.Security.Roles.IsUserInRole("oweh", "ADM"))
                {
                    System.Web.Security.Roles.AddUserToRole("oweh", "ADM");
                }
                if (!System.Web.Security.Roles.IsUserInRole("oweh", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("oweh", "VU");
                }

                #endregion

                #region VU - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("ayou", "VU"))
                {
                    System.Web.Security.Roles.AddUserToRole("ayou", "VU");
                }

                #endregion

                #region EMP - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("ealv", "EMP"))
                {
                    System.Web.Security.Roles.AddUserToRole("ealv", "EMP");
                }
                #endregion

                #region ABM - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("tmas", "ABM"))
                {
                    System.Web.Security.Roles.AddUserToRole("tmas", "ABM");
                }

                #endregion

                #region BDM - rolles added

                if (!System.Web.Security.Roles.IsUserInRole("sdea", "BDM"))
                {
                    System.Web.Security.Roles.AddUserToRole("sdea", "BDM");
                }

                #endregion 

                #region HR - roles added

                if (!System.Web.Security.Roles.IsUserInRole("mter", "HR"))
                {
                    System.Web.Security.Roles.AddUserToRole("mter", "HR");
                }

                #endregion

                #endregion

            }

            public void InitDBClear_Membership()
            {
                CreateRoles();

                if (!WebSecurity.UserExists("admin"))
                {
                    WebSecurity.CreateUserAndAccount("admin", "epol01");
                }

                if (!System.Web.Security.Roles.IsUserInRole("admin", "PU"))
                {
                    System.Web.Security.Roles.AddUserToRole("admin", "PU");
                }
            }

            public void InitDbNotChanged_Membership()
            {
            }



        }
    }
}