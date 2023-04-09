using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Db;
using Db.Model;
using Db.IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity.EntityFramework;
using ReconUi.Models;

namespace ReconUi.IdentityDAL
{
    public static class RoleDAL
    {
        public static object GetRoles()
        {
            using (var context = new ReconContext())
            {
                return context.AspNetRoles.Where(x => x.Name.ToLower() != "systemadmin").ToList();
            }
        }
        public static void SaveRole(IdentityRole Role)
        {
            using (var context = new ReconContext())
            {
                context.AspNetRoles.Add(new AspNetRoles() { Id = Role.Id, Name = Role.Name });
                context.SaveChanges();
            }
        }
        public static object GetRoleByRoleId(string roleId)
        {
            using (var context = new ReconContext())
            {
                return context.AspNetRoles.Where(x => x.Id.Equals(roleId)).FirstOrDefault();
            }
        }
        public static void UpdateRole(IdentityRole Role)
        {
            using (var context = new ReconContext())
            {
                context.AspNetRoles.AddOrUpdate(new AspNetRoles() { Id = Role.Id, Name = Role.Name });
                context.SaveChanges();
            }
        }
        public static void DeleteRole(string roleId)
        {
            using (var context = new ReconContext())
            {
                var role = context.AspNetRoles.Where(x => x.Id.Equals(roleId)).FirstOrDefault();
                context.AspNetRoles.Remove(role);
                context.SaveChanges();
            }
        }
    }
}
