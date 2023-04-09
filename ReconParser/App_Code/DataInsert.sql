SET IDENTITY_INSERT [dbo].[CashNotes] ON
INSERT INTO [CashNotes]([CashNoteId],[Note],[Description],[CreatedAt],[UpdatedAt])VALUES(1,'100','HUNDRED',GetDate(),GetDate());
INSERT INTO [CashNotes]([CashNoteId],[Note],[Description],[CreatedAt],[UpdatedAt])VALUES(2,'500','FIVE HUNDRED',GetDate(),GetDate());
INSERT INTO [CashNotes]([CashNoteId],[Note],[Description],[CreatedAt],[UpdatedAt])VALUES(3,'1000','ONE THOUSAND',GetDate(),GetDate());
SET IDENTITY_INSERT [dbo].[CashNotes] OFF

SET IDENTITY_INSERT [dbo].[Sources] ON
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(1,'EJOURNAL',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(2,'SCT',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(3,'CBS',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(4,'SWITCH',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(5,'VISA',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(6,'HBL',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(7,'NPN',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(8,'NEPS',GetDate(),GetDate());
INSERT INTO [Sources]([SourceId],[Description],[CreatedAt],[UpdatedAt])VALUES(9,'MASTER CARD',GetDate(),GetDate());
SET IDENTITY_INSERT [dbo].[Sources] OFF

SET IDENTITY_INSERT [dbo].[SubSources] ON
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(1,'NCR',GetDate(),GetDate(),1);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(2,'WINCOR',GetDate(),GetDate(),1);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(3,'DIEBOLD',GetDate(),GetDate(),1);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(4,'OTHERS_ATM_REPORT(EP707)',GetDate(),GetDate(),5);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(5,'OWN_ATM_REPORT(EP745)',GetDate(),GetDate(),5);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(6,'OTHERS_POS_REPORT(705)',GetDate(),GetDate(),5);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(7,'ATM_REVERSAL_REPORT(EP727)',GetDate(),GetDate(),5);
INSERT INTO [SubSources]([SubSourceId],[Description],[CreatedAt],[UpdatedAt],[Source_SourceId]) VALUES(8,'POS_REVERSAL_REPORT(EP725)',GetDate(),GetDate(),5);
SET IDENTITY_INSERT [dbo].[SubSources] OFF