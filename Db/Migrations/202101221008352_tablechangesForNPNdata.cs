namespace Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tablechangesForNPNdata : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CbsTransactions",
                c => new
                    {
                        CbsTransactionId = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        Particulars = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DebitOrCredit = c.String(),
                        ReferenceNumber = c.String(),
                        IbftCreditUniqueId = c.String(),
                        Balance = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        SubChildSource_SubChildSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CbsTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .ForeignKey("dbo.SubChildSources", t => t.SubChildSource_SubChildSourceId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.SubChildSource_SubChildSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.EsewaTransactions",
                c => new
                    {
                        EsewaTransactionId = c.Int(nullable: false, identity: true),
                        EsewaId = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        UniqueId = c.String(),
                        Settlement = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        NoofAttempt = c.String(),
                        Remark = c.String(),
                        InitiatingAc = c.String(),
                        CreatedBy = c.String(),
                        VerifiedBy = c.String(),
                        LastModifiedDate = c.DateTime(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EsewaTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.InternetIbftTransactions",
                c => new
                    {
                        InternetIbftTransactionId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        MobileNumber = c.String(),
                        UserName = c.String(),
                        FromAccount = c.String(),
                        ToAccount = c.String(),
                        BankCode = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargeAmount = c.Decimal(precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionUri = c.String(),
                        OrginatingUniqueId = c.String(),
                        TransactionStatus = c.String(),
                        Description = c.String(),
                        TraceId = c.String(),
                        ServiceInfoId = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.InternetIbftTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.InternetTopupTransactions",
                c => new
                    {
                        InternetTopupTransactionId = c.Int(nullable: false, identity: true),
                        TransactionTraceId = c.String(),
                        CustomerName = c.String(),
                        UserName = c.String(),
                        Profile = c.String(),
                        TopupTraceId = c.String(),
                        FromAccount = c.String(),
                        ToAccount = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionDate = c.DateTime(nullable: false),
                        TopupNumber = c.String(),
                        TransactionStatus = c.String(),
                        Remarks = c.String(),
                        TopupStatus = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.InternetTopupTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.MirrorTransactions",
                c => new
                    {
                        MirrorTransactionId = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        Particulars = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DebitOrCredit = c.String(),
                        ReferenceNumber = c.String(),
                        TraceNo = c.String(),
                        Balance = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MirrorTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.MobileIbftTransactions",
                c => new
                    {
                        MobileIbftTransactionId = c.Int(nullable: false, identity: true),
                        Id = c.String(),
                        MobileNumber = c.String(),
                        FromAccount = c.String(),
                        ToAccount = c.String(),
                        ChargeAmount = c.Decimal(precision: 18, scale: 2),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RequestedDate = c.DateTime(nullable: false),
                        AccessibleChannel = c.String(),
                        TransactionStatus = c.String(),
                        TransactionCode = c.String(),
                        TransactionDescription = c.String(),
                        DestinationAccountDescription = c.String(),
                        FonepayTraceId = c.String(),
                        FonepayTransactionStatus = c.String(),
                        TopupResponseDescription = c.String(),
                        TransactionTraceId = c.String(),
                        StanID = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MobileIbftTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.MobileTopupTransactions",
                c => new
                    {
                        MobileTopupTransactionId = c.Int(nullable: false, identity: true),
                        TraceId = c.String(),
                        ServiceName = c.String(),
                        CustomerName = c.String(),
                        UserName = c.String(),
                        FromAccount = c.String(),
                        ToAccount = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceAttribute = c.String(),
                        RecordedDate = c.DateTime(nullable: false),
                        TransactionStatus = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionTraceId = c.String(),
                        TransactionResponseCode = c.String(),
                        TransactionResponseDescription = c.String(),
                        TopupStatus = c.String(),
                        TopupTraceId = c.String(),
                        TopupResponseCode = c.String(),
                        TopupResponseDescription = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MobileTopupTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.NostroTransactions",
                c => new
                    {
                        NostroTransactionId = c.Int(nullable: false, identity: true),
                        TransactionId = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        ValueDate = c.DateTime(nullable: false),
                        ChequeNo = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BalanceAmount = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        UniqueId = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NostroTransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId);
            
            CreateTable(
                "dbo.ReportMasters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Query = c.String(),
                        DatabaseName = c.String(),
                        CategorieId = c.Int(nullable: false),
                        ReportTypeId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategorieId, cascadeDelete: true)
                .ForeignKey("dbo.ReportTypes", t => t.ReportTypeId, cascadeDelete: true)
                .Index(t => t.CategorieId)
                .Index(t => t.ReportTypeId);
            
            CreateTable(
                "dbo.ReportTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        DiagramType = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReportParameters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Label = c.String(),
                        DisplayName = c.String(),
                        Query = c.String(),
                        ParameterDataType = c.Int(nullable: false),
                        ReportMasterId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReportMasters", t => t.ReportMasterId, cascadeDelete: true)
                .Index(t => t.ReportMasterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportParameters", "ReportMasterId", "dbo.ReportMasters");
            DropForeignKey("dbo.ReportMasters", "ReportTypeId", "dbo.ReportTypes");
            DropForeignKey("dbo.ReportMasters", "CategorieId", "dbo.Categories");
            DropForeignKey("dbo.CbsTransactions", "SubChildSource_SubChildSourceId", "dbo.SubChildSources");
            DropForeignKey("dbo.NostroTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.NostroTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.NostroTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.MobileTopupTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.MobileTopupTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.MobileTopupTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.MobileIbftTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.MobileIbftTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.MobileIbftTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.MirrorTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.MirrorTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.MirrorTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.InternetTopupTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.InternetTopupTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.InternetTopupTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.InternetIbftTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.EsewaTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.CbsTransactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.InternetIbftTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.InternetIbftTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.EsewaTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.CbsTransactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.EsewaTransactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.CbsTransactions", "Source_SourceId", "dbo.Sources");
            DropIndex("dbo.ReportParameters", new[] { "ReportMasterId" });
            DropIndex("dbo.ReportMasters", new[] { "ReportTypeId" });
            DropIndex("dbo.ReportMasters", new[] { "CategorieId" });
            DropIndex("dbo.NostroTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.NostroTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.NostroTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.MobileTopupTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.MobileTopupTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.MobileTopupTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.MobileIbftTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.MobileIbftTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.MobileIbftTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.MirrorTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.MirrorTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.MirrorTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.InternetTopupTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.InternetTopupTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.InternetTopupTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.InternetIbftTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.InternetIbftTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.InternetIbftTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.EsewaTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.EsewaTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.EsewaTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.CbsTransactions", new[] { "SubChildSource_SubChildSourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "Source_SourceId" });
            DropTable("dbo.ReportParameters");
            DropTable("dbo.ReportTypes");
            DropTable("dbo.ReportMasters");
            DropTable("dbo.NostroTransactions");
            DropTable("dbo.MobileTopupTransactions");
            DropTable("dbo.MobileIbftTransactions");
            DropTable("dbo.MirrorTransactions");
            DropTable("dbo.InternetTopupTransactions");
            DropTable("dbo.InternetIbftTransactions");
            DropTable("dbo.EsewaTransactions");
            DropTable("dbo.CbsTransactions");
            DropTable("dbo.Categories");
        }
    }
}
