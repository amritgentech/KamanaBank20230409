using Db.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Db.IdentityLibrary.DataModel;
using Db.Migrations;

namespace Db
{
    public class ReconContext : DbContext
    {
        public ReconContext() : base("name=ReconContextConnectionString")
        {
            if (Database.Exists())
                Database.SetInitializer<ReconContext>(new MigrateDatabaseToLatestVersion<ReconContext, Configuration>());
            else
                Database.SetInitializer<ReconContext>(new ReconDbInitializer());
            //            Configuration.LazyLoadingEnabled = false;
            //            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Source> Sources { get; set; }
        public DbSet<SubSource> SubSources { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CashLeaf> CashLeafs { get; set; }
        public DbSet<Cash> Cashs { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankCardBinNo> BankCardBinNos { get; set; }
        public DbSet<Branch> Branchs { get; set; }
        public DbSet<MemberBank> MemberBanks { get; set; }
        public DbSet<MemberBankCardBinNo> MemberBankCardBinNos { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<ReconType> ReconTypes { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<SubChildSource> SubChildSources { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<ReconProcessStatus> ReconStatuses { get; set; }
        public DbSet<ExcelReport> ExcelReport { get; set; }
        public DbSet<NPNSettlement> NPNSettlement { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        //Digital Banking
        public DbSet<CbsTransaction> CbsTransactions { get; set; }
        public DbSet<EsewaTransaction> EsewaTransactions { get; set; }
        public DbSet<MirrorTransaction> MirrorTransactions { get; set; }
        public DbSet<NostroTransaction> NostroTransactions { get; set; }
        public virtual DbSet<MobileTopupTransaction> MobileTopupTransactions { get; set; }
        public virtual DbSet<MobileIbftTransaction> MobileIbftTransactions { get; set; }
        public virtual DbSet<InternetTopupTransaction> InternetTopupTransactions { get; set; }
        public virtual DbSet<InternetIbftTransaction> InternetIbftTransactions { get; set; }
        public DbSet<ReportMaster> ReportMasters { get; set; }
        public DbSet<ReportParameter> ReportParameters { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //            modelBuilder.Entity<AspNetRoles>()
            //                .HasMany(e => e.AspNetUsers)
            //                .WithMany(e => e.AspNetRoles)
            //                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUserRoles>().HasKey(a => new { a.RoleId, a.UserId });

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

        }
    }
}
