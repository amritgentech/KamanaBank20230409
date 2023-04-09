using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Db;
using Db.IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReconUi.Models;

namespace ReconUi.UiHelpers
{
    public static class UserType
    {
        public static Boolean isAdminUser()
        {
            ReconContext context = new ReconContext();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = HttpContext.Current.User.Identity;
                var roles = context.AspNetUserRoles.ToList();
                var allowedroles = roles.Where(x => x.UserId.Equals(user.GetUserId())).ToList();
                foreach (var role in allowedroles)
                {
                    var roleName = context.AspNetRoles.Where(x => x.Id.Equals(role.RoleId)).Select(x => x.Name).FirstOrDefault();
                    if (roleName.ToLower().Equals("admin") || roleName.ToLower().Equals("systemadmin"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public static Boolean isMakerUser()
        {
            ReconContext context = new ReconContext();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = HttpContext.Current.User.Identity;
                var roles = context.AspNetUserRoles.ToList();
                var allowedroles = roles.Where(x => x.UserId.Equals(user.GetUserId())).ToList();
                foreach (var role in allowedroles)
                {
                    var roleName = context.AspNetRoles.Where(x => x.Id.Equals(role.RoleId)).Select(x => x.Name).FirstOrDefault();
                    if (roleName.ToLower().Equals("maker") || roleName.ToLower().Equals("systemadmin"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public static Boolean isCheckerUser()
        {
            ReconContext context = new ReconContext();

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = HttpContext.Current.User.Identity;
                var roles = context.AspNetUserRoles.ToList();
                var allowedroles = roles.Where(x => x.UserId.Equals(user.GetUserId())).ToList();
                foreach (var role in allowedroles)
                {
                    var roleName = context.AspNetRoles.Where(x => x.Id.Equals(role.RoleId)).Select(x => x.Name).FirstOrDefault();
                    if (roleName.ToLower().Equals("checker") || roleName.ToLower().Equals("systemadmin"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}