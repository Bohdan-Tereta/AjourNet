using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using AjourBT.Filters;
using AjourBT.Domain.ViewModels;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Concrete;
using System.Configuration;
using AjourBT.Infrastructure;
using AjourBT.Domain.Infrastructure;
using AjourBT.Domain.Abstract;
using System.Web.Configuration;
using System.Reflection;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;
        public AccountController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;
        }

        public PartialViewResult GetLoginUser()
        {
            string listOfOnlineUsers = "";
            string CurrentUser = HttpContext.User.Identity.Name;
            Dictionary<string, DateTime> loggedInUsers = ((Dictionary<string, DateTime>)HttpRuntime.Cache["LoggedInUsers"]);
            if (loggedInUsers == null)
            {
                loggedInUsers = new Dictionary<string, DateTime>();
                HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
            }

            if (HttpContext.User.IsInRole("ADM") || HttpContext.User.IsInRole("BTM") || HttpContext.User.IsInRole("DIR") || HttpContext.User.IsInRole("ACC") || HttpContext.User.IsInRole("PU") || HttpContext.User.IsInRole("ABM") || HttpContext.User.IsInRole("BDM") || HttpContext.User.IsInRole("VU"))
            {
                if (!loggedInUsers.ContainsKey(CurrentUser))
                    loggedInUsers.Add(CurrentUser, DateTime.Now.ToLocalTimeAzure());
                else
                    loggedInUsers[CurrentUser] = DateTime.Now.ToLocalTimeAzure();
            }

            List<string> keys = loggedInUsers.ToList()
              .Where(d => d.Value < DateTime.Now.ToLocalTimeAzure().AddMinutes(-5))
              .Select(kvp => kvp.Key)
              .ToList();

            foreach (string theKey in keys)
            {
                loggedInUsers.Remove(theKey);
            }

            if (loggedInUsers != null && loggedInUsers.Keys.Count > 0)
            {
                if (loggedInUsers.Count >= 10)
                {
                    string usersToDisplay = loggedInUsers.Keys.Take(10).Aggregate((s1, s2) => s1 + ", " + s2);
                    ViewBag.UsersToDisplay = usersToDisplay;

                    string onlineUsersIDs = loggedInUsers.Select(x => x.Key).Aggregate((s1, s2) => s1 + ", " + s2);
                    ViewBag.FullListOfOnlineUsers = onlineUsersIDs;

                    return PartialView("_GetLoggedinUsers");
                }
                else
                {
                    listOfOnlineUsers = loggedInUsers.Select(x => x.Key).Aggregate((s1, s2) => s1 + ", " + s2);
                    return PartialView("_GetOnlineUsersShort", listOfOnlineUsers);
                }
            }

            return PartialView("_NoOnlineUsers");
        }

        public ActionResult FullOnlineUsersList(string loggedInUsersIDs)
        {
            if (loggedInUsersIDs == null || loggedInUsersIDs == "")
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.FullListOfOnlineUsers = loggedInUsersIDs;
                return View();
            }
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.Name != "")
            {
                ViewBag.ReturnUrl = returnUrl;
            }

            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: false))
            {
                if (HttpRuntime.Cache["LoggedInUsers"] != null) //if the list exists, add this user to it
                {
                    //get the list of logged in users from the cache
                    Dictionary<string, DateTime> loggedInUsers = (Dictionary<string, DateTime>)HttpRuntime.Cache["LoggedInUsers"];
                    //add this user to the list
                    if (HttpContext.User.IsInRole("ADM") || HttpContext.User.IsInRole("BTM") || HttpContext.User.IsInRole("DIR") || HttpContext.User.IsInRole("ACC") || HttpContext.User.IsInRole("PU") || HttpContext.User.IsInRole("ABM"))
                    {
                        if (!loggedInUsers.ContainsKey(model.UserName))
                            loggedInUsers.Add(model.UserName, DateTime.Now.ToLocalTimeAzure());
                    }
                    //add the list back into the cache
                    HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
                }
                else //the list does not exist so create it
                {
                    //create a new list
                    Dictionary<string, DateTime> loggedInUsers = new Dictionary<string, DateTime>();
                    //add this user to the list
                    loggedInUsers.Add(model.UserName, DateTime.Now.ToLocalTimeAzure());
                    //add the list into the cache
                    HttpRuntime.Cache["LoggedInUsers"] = loggedInUsers;
                }
                HttpRuntime.Cache["CurrentUserName"] = repository.Users.Where(e => e.EID == model.UserName).Select(e => e.FirstName).FirstOrDefault();
                if (model.Password == ConfigurationManager.AppSettings["DefaultPassword"])
                    return RedirectToAction("Manage", new { Message = "You must change your password!" });
                else
                    return RedirectToLocal(returnUrl, model.UserName);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Dictionary<string, DateTime> loggedInUsers = (Dictionary<string, DateTime>)HttpRuntime.Cache["LoggedInUsers"];
            if (loggedInUsers.ContainsKey(HttpContext.User.Identity.Name))//if the user is in the list
            {
                //then remove them
                loggedInUsers.Remove(HttpContext.User.Identity.Name);
            }
            WebSecurity.Logout();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("Login", "Account");
        }

        //

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }        

        #region Helpers
        public ActionResult RedirectToLocal(string returnUrl, string currentUserName)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                string currentUserFirstRole = System.Web.Security.Roles.GetRolesForUser(currentUserName).LastOrDefault();
                if (currentUserFirstRole != null)
                {
                    //RedirectToRouteResult r = RedirectToAction(String.Join("", currentUserFirstRole, "View"), "Home");
                    RedirectToRouteResult r = RedirectToAction("Index", "Home");
                    return r;
                }
                else
                    return RedirectToAction("Login", "Account");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public string ConvertEIDToName(string eid)
        {
            Employee emp = repository.Users.Where(e => e.EID == eid).FirstOrDefault();
            return emp != null ? emp.FirstName : "";
        }

        [AllowAnonymous]
        public ViewResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewResult ForgotPassword(string UserName)
        {
            if (UserName == null)
            {
                ViewBag.Message = "User does not exist.";
                return View();
            }
            var user = Membership.GetUser(UserName);
            if (user == null)
            {
                ViewBag.Message = "User does not exist.";
            }
            else
            {
                var token = WebSecurity.GeneratePasswordResetToken(UserName, Int32.Parse(ConfigurationManager.AppSettings["TokenExpirationInMinutes"]));

                var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { rt = token }, "http") + "'>Reset Password</a>";

                string body = "<b>Password Reset Token</b><br/>" + resetLink +
                    "<br>If you're not " + UserName + " or didn't request verification, you can ignore this email.";

                string subject = "Password Reset Token";
                Message msg = new Message(subject, body, repository.Users.Where(u => u.EID == UserName).FirstOrDefault());
                try
                {
                    messenger.Notify(msg);
                    ViewBag.Message = "Email with password verification token has been sent.";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error occured while sending email." + ex.Message;
                }
            }

            return View();
        }

        [AllowAnonymous]
        public ViewResult ResetPassword(string rt)
        {
            try
            {
                bool response = WebSecurity.ResetPassword(rt, ConfigurationManager.AppSettings["DefaultPassword"]);
                if (response == true)
                {
                    ViewBag.Message = "Reset password success! Your New Password Is " + ConfigurationManager.AppSettings["DefaultPassword"];
                }
                else
                {
                    string[] roles = System.Web.Security.Roles.GetUsersInRole("PU");
                    var powerUsers = String.Join(", ", roles);
                    ViewBag.Message = "Password could not be reseted! Please contact power users: " + powerUsers;
                }
            }
            catch (Exception)
            {
                string[] roles = System.Web.Security.Roles.GetUsersInRole("PU");
                var powerUsers = String.Join(", ", roles);
                ViewBag.Message = "Password could not be reseted! Please contact power users: " + powerUsers;
            }

            return View();
        }

        public PartialViewResult GetProductVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ViewBag.ProductVersion = version;
            return PartialView("_ProductVersion");
        }

    }
}

