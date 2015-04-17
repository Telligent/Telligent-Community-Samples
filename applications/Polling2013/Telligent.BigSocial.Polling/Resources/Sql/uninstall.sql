/****** Object:  StoredProcedure [dbo].[polling_PollVote_AddUpdate]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollVote_AddUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollVote_AddUpdate]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollVote_Delete]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollVote_Delete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollVote_Delete]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollVote_Get]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollVote_Get]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollVote_Get]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_AddUpdate]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_AddUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_AddUpdate]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_Delete]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_Delete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_Delete]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_Get]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_Get]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_Get]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_GetSet]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_GetSet]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_GetSet]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_GetToReindex]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_GetToReindex]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_GetToReindex]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_SetIndexed]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_SetIndexed]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_SetIndexed]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollAnswer_AddUpdate]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollAnswer_AddUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollAnswer_AddUpdate]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollAnswer_Delete]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollAnswer_Delete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollAnswer_Delete]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollAnswer_Get]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollAnswer_Get]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollAnswer_Get]
GO
/****** Object:  StoredProcedure [dbo].[polling_PollAnswer_UpdateVoteCounts]    Script Date: 08/27/2012 13:27:22 ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_PollAnswer_UpdateVoteCounts]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_PollAnswer_UpdateVoteCounts]
GO
/****** Object:  StoredProcedure [dbo].[polling_Poll_DeleteByGroup]     ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_DeleteByGroup]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].polling_Poll_DeleteByGroup
GO
/****** Object:  StoredProcedure [dbo].[polling_ReassignUser]     ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_ReassignUser]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].polling_ReassignUser
GO

/****** Object:  StoredProcedure [dbo].[polling_Poll_ReindexAll]     ******/
IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'dbo.[polling_Poll_ReindexAll]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
	DROP PROCEDURE [dbo].[polling_Poll_ReindexAll]
GO
