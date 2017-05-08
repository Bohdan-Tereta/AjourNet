using AjourBT.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Entities;
using PagedList;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    public class MessageController : Controller
    {
        //
        // GET: /Message/

        private IRepository repository;

        public MessageController(IRepository repo)
        {
            repository = repo;
        }//////////////

        #region Messages

        public ActionResult GetMessagesForRole(int tab, string role, string actionNameForPagination, string searchString = "", int page = 1)
        {
            ViewBag.SearchString = searchString;
            ViewBag.Role = role;
            ViewBag.Tab = tab;
            ViewBag.ActionNameForPagination = actionNameForPagination;
            ViewBag.Page = page;
            return View();
        }

        public ActionResult GetMessagesDataForRole(int tab, string role, string actionNameForPagination, string searchString = "", int page = 1)
        {
            searchString = searchString != null ? searchString : "";
            searchString = searchString != "" ? searchString.Trim() : "";
            IList<IMessage> selectedMessages = repository.Messages;
            if (role != null && role != String.Empty)
            {
                if (searchString != "")
                {
                    string[] searchWords = searchString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                    ViewBag.ShowLinks = true;
                    if (role == "PU")
                    {
                            for (int i = 0; i < searchWords.Length; i++)
                            {
                                selectedMessages = SearchPUMessageData(selectedMessages, searchWords[i]);
                            }
                        ViewBag.ShowLinks = false;
                    }
                    else
                    {
                        for (int i = 0; i < searchWords.Length; i++)
                        {
                            selectedMessages = SearchOtherRoleMessageData(selectedMessages, searchWords[i], role);
                        }
                    }
                }
                else
                {
                    ViewBag.ShowLinks = true;
                    if (role == "PU")
                    {
                        selectedMessages = SearchPUMessageData(selectedMessages, searchString);
                        ViewBag.ShowLinks = false;
                    }
                    else
                    {
                        selectedMessages = SearchOtherRoleMessageData(selectedMessages, searchString, role);
                    }

                }
            }
            else
                if (page == 0)
                    return HttpNotFound("Wrong Role and Page");
                else
                    return HttpNotFound("Wrong Role");

            if (page != 0)
            {
                var onePageOfMessages = selectedMessages.ToPagedList(page, 4);

                ViewBag.OnePageOfMessages = onePageOfMessages;
                ViewBag.ActionNameForPagination = actionNameForPagination;
                ViewBag.TabID = tab;
                ViewBag.Role = role;
                ViewBag.SearchString = searchString;
                return View();
            }
            else
                return HttpNotFound("Wrong Page");
        }

        public IList<IMessage> SearchPUMessageData(IList<IMessage> messages, string searchString)
        {
            List<IMessage> selected = messages.OrderByDescending(m => m.TimeStamp).
                                           Where(m => m.ReplyTo.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Body.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Role.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Subject.ToLower().Contains(searchString.ToLower()) ||
                                                      m.TimeStamp.ToString().Contains(searchString) ||
                                                      m.messageType.ToString().ToLower().Contains(searchString.ToLower()) ||
            (m.FullName != null && m.FullName.ToString().ToLower().Contains(searchString.ToLower()))).ToList();
            return selected;
        }

        public IList<IMessage> SearchOtherRoleMessageData(IList<IMessage> messages, string searchString, string role)
        {
            IList<IMessage> selected = messages.OrderByDescending(m => m.TimeStamp).
                                           Where(m => m.Role == role && (
                                                      m.ReplyTo.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Body.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Role.ToLower().Contains(searchString.ToLower()) ||
                                                      m.Subject.ToLower().Contains(searchString.ToLower()) ||
                                                      m.TimeStamp.ToString().Contains(searchString) ||
                                                      m.messageType.ToString().ToLower().Contains(searchString.ToLower()))).ToList();
            return selected;
        }


        #endregion
    }
}

