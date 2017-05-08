using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace AjourBT.Tests.Messaging_Subsystem
{
    public class TestRoleProvider : RoleProvider
    {
        private readonly RoleProvider _roleProvider;
        public TestRoleProvider(RoleProvider provider)
        {
            _roleProvider = provider;
        }

        public TestRoleProvider()
        {
            _roleProvider = new Mock<RoleProvider>().Object;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            RoleProvider.AddUsersToRoles(usernames, roleNames);
        }

        public override string ApplicationName
        {
            get { return RoleProvider.ApplicationName; }
            set { RoleProvider.ApplicationName = value; }
        }

        public RoleProvider RoleProvider
        {
            get { return _roleProvider; }
        }

        public override void CreateRole(string roleName)
        {
            RoleProvider.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return RoleProvider.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return RoleProvider.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string[] GetAllRoles()
        {
            return RoleProvider.GetAllRoles();
        }

        public override string[] GetRolesForUser(string username)
        {
            return RoleProvider.GetRolesForUser(username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            switch(roleName)
            { 
                case "BTM":
            return new string [] {"abc",  "xyz"};
                case "ADM":
                    return new string[] { "qaz", "wsx"};
                case "ACC":
                    return new string[] { "edc", "rfv" };
                case "DIR":
                    return new string[] { "tgb", "yhn" };
                case "PU":
                    return new string[] { "ujm", "ik," };
                case "VU":
                    return new string[] { "ol.", "p;/" };
                case "EMP":
                    return new string[] { "cev", "afs" };
                default:
                    return null;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if(username == "tedk" && roleName == "ADM")
            {
                return true;
            }
            return false;
            //return _roleProvider.IsUserInRole(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            RoleProvider.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override bool RoleExists(string roleName)
        {
            return RoleProvider.RoleExists(roleName);
        }
    }
}
