using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Db.IdentityLibrary.DataModel
{
    using System;
    public class AspNetUserRoles : IdentityUserRole<string>
    {
    }
}
