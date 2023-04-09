using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace ReconUi.Models
{
    public class IdentityRelatedModel
    {
        //        [UIHint("_IdentityLst")]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public AllroleWithAllUser AllroleWithAllUser { get; set; }
        public List<IdentityRelatedModel> IdentityRelatedModels { get; set; }

        public IEnumerable<Microsoft.AspNet.Identity.EntityFramework.IdentityUser> lstIdentityUser { get; set; }
        public IEnumerable<Microsoft.AspNet.Identity.EntityFramework.IdentityRole> lstIdentityRole { get; set; }
        public IEnumerable<AllroleWithAllUser> AllDetailsUserlist { get; set; }

        public List<SelectListItem> UserRolesList
        {
            get;
            set;
        }
        public string UserRoleId
        {
            get;
            set;
        }
        public string UserRoleName
        {
            get;
            set;
        }
    }

    public class FormData
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRoleId { get; set; }

        public List<FormData> FormDatas { get; set; }
    }

}