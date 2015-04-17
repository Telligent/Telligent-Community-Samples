SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS(select * from sys.columns where Name = N'HideResultsUntilVotingComplete' and Object_ID = Object_ID(N'dbo.polling_Polls'))
BEGIN
	ALTER TABLE dbo.polling_Polls ADD
		HideResultsUntilVotingComplete bit NOT NULL CONSTRAINT DF_polling_Polls_ShowResults DEFAULT 0,
		VotingEndDateUtc datetime NULL	
END

GO