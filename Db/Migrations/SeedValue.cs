using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.Enum;
using Db.IdentityLibrary.DataModel;
using Db.Model;
using Microsoft.AspNet.Identity;

namespace Db.Migrations
{
    public static class SeedValue
    {
        public static void Values(ReconContext context)
        {
            Source _Source1 = new Source { Description = "VISA" };
            Source _Source2 = new Source { Description = "EJOURNAL" };
            Source _Source3 = new Source { Description = "CBS" };
            Source _Source4 = new Source { Description = "NPN" };
            Source _Source5 = new Source { Description = "MasterCard" };
            Source _Source6 = new Source { Description = "DIGITALBANKING" };
           

            context.Sources.AddOrUpdate(s => s.Description,
                _Source1, _Source2, _Source3, _Source4, _Source5,_Source6
            );

            context.SaveChanges();

            SubSource _SubSource1 = new SubSource { Description = "EP705", Source = _Source1 };
            SubSource _SubSource2 = new SubSource { Description = "EP725", Source = _Source1 };
            SubSource _SubSource3 = new SubSource { Description = "EP707", Source = _Source1 };
            SubSource _SubSource4 = new SubSource { Description = "EP727", Source = _Source1 };
            SubSource _SubSource5 = new SubSource { Description = "EP745", Source = _Source1 };

            SubSource _SubSource6 = new SubSource { Description = "NCR", Source = _Source2 };
            SubSource _SubSource7 = new SubSource { Description = "WINCOR", Source = _Source2 };
            SubSource _SubSource8 = new SubSource { Description = "DIEBOLD", Source = _Source2 };
            SubSource _SubSource9 = new SubSource { Description = "Finacle", Source = _Source3 };
            SubSource _SubSource10 = new SubSource { Description = "Flexcube", Source = _Source3 };
            SubSource _SubSource11 = new SubSource { Description = "Pumori", Source = _Source3 };
            SubSource _SubSource12 = new SubSource { Description = "T24", Source = _Source3 };
            SubSource _SubSource13 = new SubSource { Description = "NPN", Source = _Source5 };
            SubSource _SubSource14 = new SubSource { Description = "Esewa", Source = _Source6 };
            SubSource _SubSource15 = new SubSource { Description = "Cbs", Source = _Source6 };
            SubSource _SubSource16 = new SubSource { Description = "Nostro", Source = _Source6 };
            SubSource _SubSource17 = new SubSource { Description = "Mirror", Source = _Source6 };
            SubSource _SubSource18 = new SubSource { Description = "InternetIbft", Source = _Source6 };
            SubSource _SubSource19 = new SubSource { Description = "MobileIbft", Source = _Source6 };
            SubSource _SubSource20 = new SubSource { Description = "InternetTopup", Source = _Source6 };
            SubSource _SubSource21 = new SubSource { Description = "MobileTopup", Source = _Source6 };

            context.SubSources.AddOrUpdate(ss => ss.Description,
                _SubSource1,
                _SubSource2,
                _SubSource3,
                _SubSource4,
                _SubSource5,
                _SubSource6,
                _SubSource7,
                _SubSource8,
                _SubSource9,
                _SubSource10,
                _SubSource11,
                _SubSource12,
                _SubSource13,
                 _SubSource14,
                _SubSource15,
                _SubSource16,
                _SubSource17,
                _SubSource18,
                _SubSource19,
                _SubSource20,
                _SubSource21
            );

            context.SaveChanges();

            context.SubChildSources.AddOrUpdate(
                scs => scs.SourceChildDescription,
                    new SubChildSource { Description = "NPN", SourceChildDescription = "NCRNPN", SubSource = _SubSource6 },
                    new SubChildSource { Description = "NPN", SourceChildDescription = "WINCORNPN", SubSource = _SubSource7 },
                    new SubChildSource { Description = "NPN", SourceChildDescription = "DIEBOLDNPN", SubSource = _SubSource8 },
                    new SubChildSource { Description = "NPN", SourceChildDescription = "VISANPN", SubSource = _SubSource5 },
                     //digital banking cover
                    new SubChildSource { Description = "EsewaParking", SourceChildDescription = "EsewaParking", SubSource = _SubSource15 },
                    new SubChildSource { Description = "FonepayIbftParking", SourceChildDescription = "FonepayIbftParking", SubSource = _SubSource15},
                    new SubChildSource { Description = "TopupParking", SourceChildDescription = "TopupParking", SubSource = _SubSource15 }

                    );
            context.ReconTypes.AddOrUpdate(
                r => r.Name,
                new ReconType { Description = "Npn Vs Cbs", Name = "Npn Vs Cbs", MapReconMethod = "GetVwTransactionDetailsModels", IsDisplay = true },
                new ReconType { Description = "Npn Vs Cbs Vs Ej", Name = "Npn Vs Cbs Vs Ej", MapReconMethod = "GetVwTransactionDetailsModels", IsDisplay = true },
                new ReconType { Description = "Npn Vs Cbs Vs Visa", Name = "Npn Vs Cbs Vs Visa", MapReconMethod = "GetVwTransactionDetailsModels", IsDisplay = true }

            );

            context.Cashs.AddOrUpdate(
                c => c.Note,
                new Cash { Note = "100", Description = "HUNDRED" },
                new Cash { Note = "500", Description = "FIVE HUNDRED" },
                new Cash { Note = "1000", Description = "ONE THOUSAND" }
            );

            context.Reasons.AddOrUpdate(
                r => r.Name,
                new Reason { Name = "Match", Description = "All Match", IsDisplay = true },//1
                new Reason { Name = "DateDiff", Description = "DateDiff", IsDisplay = true },//2
                new Reason { Name = "Missing VISA", Description = "Missing VISA", IsDisplay = false },//3
                new Reason { Name = "Missing EJOURNAL", Description = "Missing EJOURNAL", IsDisplay = false },//4
                new Reason { Name = "Missing SCT", Description = "Missing SCT", IsDisplay = false },//5
                new Reason { Name = "Invalid", Description = "Missing CBS for Neps Success", IsDisplay = true },//6
                new Reason { Name = "Exception", Description = "Missing CBS for Neps UnSuccess", IsDisplay = true },//6
                new Reason { Name = "UnMatch", Description = "Not Reconcile", IsDisplay = true },//6
                new Reason { Name = "Suspected", Description = "Suspected", IsDisplay = true },//6
                new Reason { Name = "Missing SWITCH", Description = "Missing SWITCH", IsDisplay = false },//7
                new Reason { Name = "Missing HBL", Description = "Missing HBL", IsDisplay = false },//8
                new Reason { Name = "Missing NPN", Description = "Missing NPN", IsDisplay = false },//9
                new Reason { Name = "Missing NEPS", Description = "Missing NEPS", IsDisplay = false },//10
                new Reason { Name = "Missing MASTER CARD", Description = "Missing MASTER CARD", IsDisplay = false },//11
                new Reason { Name = "Response Code not Match", Description = "Response Code not Match", IsDisplay = false },//12
                new Reason { Name = "Reversal Transactin Missing", Description = "Reversal Transactin Missing", IsDisplay = false },//13
                new Reason { Name = "Other Reason", Description = "Other Reason", IsDisplay = false }//14
            );

            string userId = Guid.NewGuid().ToString();
            string roleId = Guid.NewGuid().ToString();
            var hasher = new PasswordHasher();
            const string roleName = "SystemAdmin";
            //            string userId = "4dfcf479-8280-4deb-a7d9-9e496fa1b7b4";
            //            string roleId = "42df248e-9a41-4536-9660-5889126748e8";
            var user = new AspNetUsers
            {
                Id = userId,
                UserName = "Super",
                Email = "elite@elite.com.np",
                EmailConfirmed = true,
                PasswordHash = "ALv3jmBtzRttDwbQWqtyfW2V0zb07MkKvYbmMwnfOjTyqb/PLA36tTttfG/oNJBcdA==", // Test@2
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var role = new AspNetRoles()
            {
                Id = roleId,
                Name = roleName,
            };
            context.AspNetUsers.AddOrUpdate(a => a.Email, user);
            context.AspNetRoles.AddOrUpdate(r => r.Name, role);
            context.AspNetUserRoles.AddOrUpdate(new AspNetUserRoles() { RoleId = roleId, UserId = userId });

        }
    }
}
