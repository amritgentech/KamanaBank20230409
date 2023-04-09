using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReconUi.Models
{
    public class AllroleWithAllUser
    {
        public string userId { get; set; }
        public string UserRoleName { get; set; }
        public List<string> UserRoleNamelst { get; set; }
        public string UserName { get; set; }
        public IEnumerable<AllroleWithAllUser> AllDetailsUserlist { get; set; }
    }
}