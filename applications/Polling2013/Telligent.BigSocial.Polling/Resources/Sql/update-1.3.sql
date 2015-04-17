SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(select * from sys.columns where Name = N'NewPollEmailSent' and Object_ID = Object_ID(N'dbo.polling_Polls'))
BEGIN
	ALTER TABLE dbo.[polling_Polls] ADD
		NewPollEmailSent bit NOT NULL CONSTRAINT DF_polling_Polls_NewPollEmailSent DEFAULT 0
END
GO

UPDATE dbo.polling_Polls SET NewPollEmailSent = 1
GO

IF NOT EXISTS(select * from sys.columns where Name = N'IsSuspectedAbusive' and Object_ID = Object_ID(N'dbo.polling_Polls'))
BEGIN
	ALTER TABLE dbo.[polling_Polls] ADD
		IsSuspectedAbusive bit NOT NULL CONSTRAINT DF_polling_Polls_IsSuspectedAbusive DEFAULT 0
END

IF NOT EXISTS(SELECT * FROM  sys.columns 
            WHERE Name = N'ApplicationId' and Object_ID = Object_ID(N'polling_Polls'))    
BEGIN
	ALTER TABLE [dbo].[polling_Polls]
		ADD [ApplicationId] UNIQUEIDENTIFIER NULL
END
GO

UPDATE p SET ApplicationId = g.NodeId FROM dbo.cs_Groups g JOIN dbo.[polling_Polls] p ON g.GroupID = p.GroupId WHERE ApplicationId IS NULL
GO

ALTER TABLE [dbo].[polling_Polls]
	ALTER COLUMN [ApplicationId] UNIQUEIDENTIFIER NOT NULL
GO

ALTER TABLE [dbo].[polling_Polls]
	ALTER COLUMN [GroupId] INT NULL
GO