namespace Db.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLogs",
                c => new
                    {
                        ActivityLogId = c.Int(nullable: false, identity: true),
                        TableName = c.String(),
                        LogData = c.String(),
                        LogDescription = c.String(),
                        ActivityUser = c.String(),
                        ActivityUserId = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityLogId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId });
            
            CreateTable(
                "dbo.BankCardBinNoes",
                c => new
                    {
                        BankCardBinNoId = c.Int(nullable: false, identity: true),
                        BinNo = c.String(),
                        NetworkType = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Bank_BankId = c.Int(),
                    })
                .PrimaryKey(t => t.BankCardBinNoId)
                .ForeignKey("dbo.Banks", t => t.Bank_BankId)
                .Index(t => t.Bank_BankId);
            
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        BankId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BankCode = c.String(),
                        Address = c.String(),
                        Contact = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BankId);
            
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        BranchId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BranchCode = c.String(),
                        Address = c.String(),
                        Contact = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Bank_BankId = c.Int(),
                    })
                .PrimaryKey(t => t.BranchId)
                .ForeignKey("dbo.Banks", t => t.Bank_BankId)
                .Index(t => t.Bank_BankId);
            
            CreateTable(
                "dbo.Terminals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TerminalId = c.String(),
                        Name = c.String(),
                        Address = c.String(),
                        TerminalType = c.Int(nullable: false),
                        TerminalBrand = c.String(),
                        TerminalIP = c.String(),
                        TerminalUserName = c.String(),
                        TerminalPassword = c.String(),
                        BranchId = c.Int(nullable: false),
                        Cbs_terminal_ac = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Branches", t => t.BranchId, cascadeDelete: true)
                .Index(t => t.BranchId);
            
            CreateTable(
                "dbo.CashLeaves",
                c => new
                    {
                        CashLeafId = c.Int(nullable: false, identity: true),
                        PhysicalCassettePosition = c.Int(nullable: false),
                        TotalNoteCount = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Cash_CashId = c.Int(),
                        Transaction_TransactionId = c.Int(),
                    })
                .PrimaryKey(t => t.CashLeafId)
                .ForeignKey("dbo.Cashes", t => t.Cash_CashId)
                .ForeignKey("dbo.Transactions", t => t.Transaction_TransactionId)
                .Index(t => t.Cash_CashId)
                .Index(t => t.Transaction_TransactionId);
            
            CreateTable(
                "dbo.Cashes",
                c => new
                    {
                        CashId = c.Int(nullable: false, identity: true),
                        Note = c.String(),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CashId);
            
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
                "dbo.Sources",
                c => new
                    {
                        SourceId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SourceId);
            
            CreateTable(
                "dbo.EsewaTransactions",
                c => new
                    {
                        EsewaTransactionId = c.Int(nullable: false, identity: true),
                        EsewaId = c.String(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "dbo.SubSources",
                c => new
                    {
                        SubSourceId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        Source_SourceId = c.Int(),
                    })
                .PrimaryKey(t => t.SubSourceId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId)
                .Index(t => t.Source_SourceId);
            
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
                "dbo.UploadedFiles",
                c => new
                    {
                        UploadedFileId = c.Int(nullable: false, identity: true),
                        ActualFileName = c.String(),
                        ShowFileName = c.String(),
                        Catagory = c.String(),
                        SourceId = c.Int(nullable: false),
                        SubSourceId = c.Int(),
                        SubChildSourceId = c.Int(),
                        MinTransactionId = c.Int(nullable: false),
                        MaxTransactionId = c.Int(nullable: false),
                        TerminalId = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UploadedFileId);
            
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
                        TransactionDate = c.DateTime(nullable: false),
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
                "dbo.Transactions",
                c => new
                    {
                        TransactionId = c.Int(nullable: false, identity: true),
                        CardNo = c.String(),
                        AuthCode = c.String(),
                        ResponseCode = c.String(),
                        ResponseCodeDescription = c.String(),
                        TransactionNo = c.String(),
                        TraceNo = c.String(),
                        ReferenceNo = c.String(),
                        TerminalId = c.String(),
                        UtrNo = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        GmtToLocalTransactionDate = c.DateTime(),
                        TransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SourceTransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DestinationTransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionTime = c.Time(nullable: false, precision: 7),
                        ProcessingCode = c.String(),
                        AdviseDate = c.DateTime(),
                        AccountNo = c.String(),
                        Currency = c.String(),
                        CBSValueDate = c.DateTime(),
                        CBSRefValue = c.String(),
                        TransactionType = c.Int(nullable: false),
                        CardType = c.Int(nullable: false),
                        TerminalOwner = c.Int(nullable: false),
                        TransactionStatus = c.Int(nullable: false),
                        TerminalType = c.Int(nullable: false),
                        NetworkType = c.Int(nullable: false),
                        ApplicationGenerated = c.Boolean(nullable: false),
                        Recon_status = c.String(),
                        MainCode = c.String(),
                        Source_SourceId = c.Int(nullable: false),
                        SubSource_SubSourceId = c.Int(nullable: false),
                        UploadedFile_UploadedFileId = c.Int(nullable: false),
                        Issuing_Bank = c.Int(nullable: false),
                        Invalid = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        SubChildSource_SubChildSourceId = c.Int(),
                    })
                .PrimaryKey(t => t.TransactionId)
                .ForeignKey("dbo.Sources", t => t.Source_SourceId, cascadeDelete: true)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId, cascadeDelete: true)
                .ForeignKey("dbo.UploadedFiles", t => t.UploadedFile_UploadedFileId, cascadeDelete: true)
                .ForeignKey("dbo.SubChildSources", t => t.SubChildSource_SubChildSourceId)
                .Index(t => t.TransactionId, name: "TXN_INDEX_TransactionId")
                .Index(t => t.TransactionDate, name: "TXN_INDEX_TransactionDate")
                .Index(t => t.TransactionStatus, name: "TXN_INDEX_TransactionStatus")
                .Index(t => t.Source_SourceId)
                .Index(t => t.SubSource_SubSourceId)
                .Index(t => t.UploadedFile_UploadedFileId)
                .Index(t => t.SubChildSource_SubChildSourceId);
            
            CreateTable(
                "dbo.SubChildSources",
                c => new
                    {
                        SubChildSourceId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        SourceChildDescription = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        SubSource_SubSourceId = c.Int(),
                    })
                .PrimaryKey(t => t.SubChildSourceId)
                .ForeignKey("dbo.SubSources", t => t.SubSource_SubSourceId)
                .Index(t => t.SubSource_SubSourceId);
            
            CreateTable(
                "dbo.ExcelReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sql = c.String(),
                        ReportName = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberBankCardBinNoes",
                c => new
                    {
                        MemberBankCardBinNoId = c.Int(nullable: false, identity: true),
                        BinNo = c.String(),
                        NetworkType = c.Int(nullable: false),
                        MemberBankId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MemberBankCardBinNoId)
                .ForeignKey("dbo.MemberBanks", t => t.MemberBankId, cascadeDelete: true)
                .Index(t => t.MemberBankId);
            
            CreateTable(
                "dbo.MemberBanks",
                c => new
                    {
                        MemberBankId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        BankCode = c.String(),
                        Address = c.String(),
                        Contact = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MemberBankId);
            
            CreateTable(
                "dbo.NPNSettlements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BinNo = c.String(),
                        BankName = c.String(),
                        TranDate = c.String(),
                        TerminalId = c.String(),
                        CardNo = c.String(),
                        TraceNo = c.String(),
                        RRN = c.String(),
                        TranAmt = c.Double(nullable: false),
                        AuthCode = c.String(),
                        Loro = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reasons",
                c => new
                    {
                        ReasonId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsDisplay = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReasonId);
            
            CreateTable(
                "dbo.ReconProcessStatus",
                c => new
                    {
                        ReconStatusId = c.Int(nullable: false, identity: true),
                        IsReconStarted = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReconStatusId);
            
            CreateTable(
                "dbo.ReconTypes",
                c => new
                    {
                        ReconTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        MapReconMethod = c.String(),
                        IsDisplay = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReconTypeId);
            
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
            
            CreateTable(
                "dbo.Settlements",
                c => new
                    {
                        SettlementId = c.Int(nullable: false, identity: true),
                        TransactionDate = c.DateTime(nullable: false),
                        TerminalId = c.String(),
                        CardNo = c.String(),
                        TraceNo = c.String(),
                        AuthCode = c.String(),
                        VisaTransactionId = c.String(),
                        NpnTransactionId = c.String(),
                        CBSTransactionId = c.String(),
                        VisaTransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NpnTransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CbsTransactionAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VisaResponseCode = c.String(),
                        NpnResponseCode = c.String(),
                        CbsResponseCode = c.String(),
                        TerminalType = c.String(),
                        FT_Branch = c.String(),
                        IsOwnUsPayableReceivable = c.String(),
                        Status = c.String(),
                        Reason = c.String(),
                        IsSettled = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SettlementId);
            
            CreateTable(
                "dbo.AspNetUsersAspNetRoles",
                c => new
                    {
                        AspNetUsers_Id = c.String(nullable: false, maxLength: 128),
                        AspNetRoles_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.AspNetUsers_Id, t.AspNetRoles_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUsers_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.AspNetRoles_Id, cascadeDelete: true)
                .Index(t => t.AspNetUsers_Id)
                .Index(t => t.AspNetRoles_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportParameters", "ReportMasterId", "dbo.ReportMasters");
            DropForeignKey("dbo.ReportMasters", "ReportTypeId", "dbo.ReportTypes");
            DropForeignKey("dbo.ReportMasters", "CategorieId", "dbo.Categories");
            DropForeignKey("dbo.MemberBankCardBinNoes", "MemberBankId", "dbo.MemberBanks");
            DropForeignKey("dbo.Transactions", "SubChildSource_SubChildSourceId", "dbo.SubChildSources");
            DropForeignKey("dbo.SubChildSources", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.CbsTransactions", "SubChildSource_SubChildSourceId", "dbo.SubChildSources");
            DropForeignKey("dbo.SubSources", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.Transactions", "UploadedFile_UploadedFileId", "dbo.UploadedFiles");
            DropForeignKey("dbo.Transactions", "SubSource_SubSourceId", "dbo.SubSources");
            DropForeignKey("dbo.Transactions", "Source_SourceId", "dbo.Sources");
            DropForeignKey("dbo.CashLeaves", "Transaction_TransactionId", "dbo.Transactions");
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
            DropForeignKey("dbo.CashLeaves", "Cash_CashId", "dbo.Cashes");
            DropForeignKey("dbo.Branches", "Bank_BankId", "dbo.Banks");
            DropForeignKey("dbo.Terminals", "BranchId", "dbo.Branches");
            DropForeignKey("dbo.BankCardBinNoes", "Bank_BankId", "dbo.Banks");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsersAspNetRoles", "AspNetRoles_Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUsersAspNetRoles", "AspNetUsers_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsersAspNetRoles", new[] { "AspNetRoles_Id" });
            DropIndex("dbo.AspNetUsersAspNetRoles", new[] { "AspNetUsers_Id" });
            DropIndex("dbo.ReportParameters", new[] { "ReportMasterId" });
            DropIndex("dbo.ReportMasters", new[] { "ReportTypeId" });
            DropIndex("dbo.ReportMasters", new[] { "CategorieId" });
            DropIndex("dbo.MemberBankCardBinNoes", new[] { "MemberBankId" });
            DropIndex("dbo.SubChildSources", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.Transactions", new[] { "SubChildSource_SubChildSourceId" });
            DropIndex("dbo.Transactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.Transactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.Transactions", new[] { "Source_SourceId" });
            DropIndex("dbo.Transactions", "TXN_INDEX_TransactionStatus");
            DropIndex("dbo.Transactions", "TXN_INDEX_TransactionDate");
            DropIndex("dbo.Transactions", "TXN_INDEX_TransactionId");
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
            DropIndex("dbo.SubSources", new[] { "Source_SourceId" });
            DropIndex("dbo.EsewaTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.EsewaTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.EsewaTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "UploadedFile_UploadedFileId" });
            DropIndex("dbo.CbsTransactions", new[] { "SubChildSource_SubChildSourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "SubSource_SubSourceId" });
            DropIndex("dbo.CbsTransactions", new[] { "Source_SourceId" });
            DropIndex("dbo.CashLeaves", new[] { "Transaction_TransactionId" });
            DropIndex("dbo.CashLeaves", new[] { "Cash_CashId" });
            DropIndex("dbo.Terminals", new[] { "BranchId" });
            DropIndex("dbo.Branches", new[] { "Bank_BankId" });
            DropIndex("dbo.BankCardBinNoes", new[] { "Bank_BankId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropTable("dbo.AspNetUsersAspNetRoles");
            DropTable("dbo.Settlements");
            DropTable("dbo.ReportParameters");
            DropTable("dbo.ReportTypes");
            DropTable("dbo.ReportMasters");
            DropTable("dbo.ReconTypes");
            DropTable("dbo.ReconProcessStatus");
            DropTable("dbo.Reasons");
            DropTable("dbo.NPNSettlements");
            DropTable("dbo.MemberBanks");
            DropTable("dbo.MemberBankCardBinNoes");
            DropTable("dbo.ExcelReports");
            DropTable("dbo.SubChildSources");
            DropTable("dbo.Transactions");
            DropTable("dbo.NostroTransactions");
            DropTable("dbo.MobileTopupTransactions");
            DropTable("dbo.MobileIbftTransactions");
            DropTable("dbo.MirrorTransactions");
            DropTable("dbo.InternetTopupTransactions");
            DropTable("dbo.UploadedFiles");
            DropTable("dbo.InternetIbftTransactions");
            DropTable("dbo.SubSources");
            DropTable("dbo.EsewaTransactions");
            DropTable("dbo.Sources");
            DropTable("dbo.CbsTransactions");
            DropTable("dbo.Categories");
            DropTable("dbo.Cashes");
            DropTable("dbo.CashLeaves");
            DropTable("dbo.Terminals");
            DropTable("dbo.Branches");
            DropTable("dbo.Banks");
            DropTable("dbo.BankCardBinNoes");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ActivityLogs");
        }
    }
}
