/****** Object:  Table [dbo].[polling_PollVotes]    Script Date: 08/27/2012 13:27:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[polling_PollVotes]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[polling_PollVotes](
		[PollId] [uniqueidentifier] NOT NULL,
		[UserId] [int] NOT NULL,
		[PollAnswerId] [uniqueidentifier] NOT NULL,
		[CreatedDateUtc] [datetime] NOT NULL CONSTRAINT [DF_polling_PollVotes_CreatedDateUtc]  DEFAULT (getdate()),
		[LastUpdatedDateUtc] [datetime2](7) NOT NULL CONSTRAINT [DF_polling_PollVotes_LastUpdatedDateUtc]  DEFAULT (getdate()),
	 CONSTRAINT [PK_polling_PollVotes] PRIMARY KEY CLUSTERED 
	(
		[PollId] ASC,
		[UserId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[polling_Polls]    Script Date: 08/27/2012 13:27:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[polling_Polls]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[polling_Polls](
		[Id] [uniqueidentifier] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[Description] [nvarchar](max) NULL,
		[AuthorUserId] [int] NOT NULL,
		[GroupId] [int] NOT NULL,
		[IsEnabled] [bit] NOT NULL CONSTRAINT [DF_polling_Polls_IsEnabled]  DEFAULT ((1)),
		[IsIndexed] [bit] NOT NULL CONSTRAINT [DF_polling_Polls_IsIndexed]  DEFAULT ((0)),
		[CreatedDateUtc] [datetime] NOT NULL CONSTRAINT [DF_polling_Polls_CreatedDateUtc]  DEFAULT (getdate()),
		[LastUpdatedDateUtc] [datetime] NOT NULL CONSTRAINT [DF_polling_Polls_LastUpdatedDateUtc]  DEFAULT (getdate()),
	 CONSTRAINT [PK_polling_Polls] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[polling_PollAnswers]    Script Date: 08/27/2012 13:27:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[polling_PollAnswers]') AND type in (N'U')) 
BEGIN
	CREATE TABLE [dbo].[polling_PollAnswers](
		[Id] [uniqueidentifier] NOT NULL CONSTRAINT [DF_polling_PollAnswers_Id]  DEFAULT (newid()),
		[PollId] [uniqueidentifier] NOT NULL,
		[Name] [nvarchar](255) NOT NULL,
		[VoteCount] [int] NOT NULL CONSTRAINT [DF_polling_PollAnswer_VoteCount]  DEFAULT ((0)),
	 CONSTRAINT [PK_polling_PollAnswers] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
