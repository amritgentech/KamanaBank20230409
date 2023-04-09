namespace Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InternetIbftTransactions", "MobileNumber", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "UserName", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "BankCode", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "ChargeAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InternetIbftTransactions", "TransactionUri", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "OrginatingUniqueId", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "ServiceInfoId", c => c.String());
            DropColumn("dbo.InternetIbftTransactions", "BankName");
            DropColumn("dbo.InternetIbftTransactions", "LoginName");
            DropColumn("dbo.InternetIbftTransactions", "TransactionType");
            DropColumn("dbo.InternetIbftTransactions", "IbftTransactionStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InternetIbftTransactions", "IbftTransactionStatus", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "TransactionType", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "LoginName", c => c.String());
            AddColumn("dbo.InternetIbftTransactions", "BankName", c => c.String());
            DropColumn("dbo.InternetIbftTransactions", "ServiceInfoId");
            DropColumn("dbo.InternetIbftTransactions", "OrginatingUniqueId");
            DropColumn("dbo.InternetIbftTransactions", "TransactionUri");
            DropColumn("dbo.InternetIbftTransactions", "ChargeAmount");
            DropColumn("dbo.InternetIbftTransactions", "BankCode");
            DropColumn("dbo.InternetIbftTransactions", "UserName");
            DropColumn("dbo.InternetIbftTransactions", "MobileNumber");
        }
    }
}
