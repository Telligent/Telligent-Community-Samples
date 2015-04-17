using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Telligent.Evolution.Components;
using Telligent.BigSocial.Polling.InternalApi;
using Telligent.DynamicConfiguration.Components;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.Security.Version1;
using Telligent.Evolution.Extensibility.Urls.Version1;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Urls.Routing;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using UIApi = Telligent.Evolution.Extensibility.UI.Version1;
using Permission = Telligent.Evolution.Extensibility.Security.Version1.Permission;

namespace Telligent.BigSocial.Polling.Plugins
{
    public class PollingApplication : IPlugin, IConfigurablePlugin, IRequiredConfigurationPlugin, IInstallablePlugin, IPluginGroup, IApplicationNavigable, INavigable, ITranslatablePlugin, IPermissionRegistrar, ICategorizedPlugin
	{
		PollFactoryDefaultWidgetProvider _widgetProvider;
        private ITranslatablePluginController _translatablePluginController;

		#region IPlugin Members

		public string Name
		{
            get { return "Big Social Polling - Polling Application"; }
		}

		public string Description
		{
			get { return "Adds support for defining polls within groups."; }
		}

		public void Initialize()
		{
			_widgetProvider = Telligent.Evolution.Extensibility.Version1.PluginManager.Get<PollFactoryDefaultWidgetProvider>().FirstOrDefault();
		}

		#endregion

		#region IConfigurablePlugin Members

		public PropertyGroup[] ConfigurationOptions
		{
			get 
			{
				PropertyGroup group = new PropertyGroup("setup", "Setup", 1);
				group.Properties.Add(new Property("connectionString", "Database Connection String", PropertyType.String, 1, "") { DescriptionText = "The connection string used to access a SQL 2008 or newer database. The user identified should have db_owner permissions to the database." });
				return new PropertyGroup[] { group };
			}
		}

		public void Update(IPluginConfiguration configuration)
		{
			InternalApi.PollingDataService.ConnectionString = configuration.GetString("connectionString");
		}

		#endregion

		#region IRequiredConfigurationPlugin Members

		public bool IsConfigured
		{
			get 
			{
				return InternalApi.PollingDataService.IsConnectionStringValid();
			}
		}

		#endregion

		#region IInstallablePlugin Members

		public void Install(Version lastInstalledVersion)
        {
            #region Install SQL
            if (lastInstalledVersion == null || lastInstalledVersion.Major == 0)
				InternalApi.PollingDataService.Install();

			if (lastInstalledVersion == null || lastInstalledVersion <= new Version(1, 1))
				InternalApi.PollingDataService.Install("update-1.1.sql");

            if (lastInstalledVersion == null || lastInstalledVersion <= new Version(1, 3))
                InternalApi.PollingDataService.Install("update-1.3.sql");

			InternalApi.PollingDataService.Install("storedprocedures.sql");
            #endregion

            #region Install Widgets

            _widgetProvider = Telligent.Evolution.Extensibility.Version1.PluginManager.Get<PollFactoryDefaultWidgetProvider>().FirstOrDefault();
			UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.DeleteAllFiles(_widgetProvider);

			var definitionFiles = new string[] { 
				"PollingBreadcrumbs-Widget.xml",
				"PollingCreateEditPoll-Widget.xml",
				"PollingLinks-Widget.xml",
				"PollingPoll-Widget.xml",
				"PollingPollList-Widget.xml",
				"PollingTitle-Widget.xml",
			};

			foreach (var definitionFile in definitionFiles)
			{
				using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Widgets." + definitionFile))
				{
					UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.AddUpdateDefinitionFile(_widgetProvider, definitionFile, stream);
				}
			}

			var supplementaryFiles = new Dictionary<Guid,string[]>();
			supplementaryFiles[new Guid("86f521cd27fb43919261b3383c2ccb15")] = new string[] {
				"PollingCreateEditPoll/ui.js"
			};
			supplementaryFiles[new Guid("5f93753d405f4eca8faef2f9ed07b946")] = new string[] {
				"PollingPollList/pagedContent.vm"
			};
			
			foreach (var instanceId in supplementaryFiles.Keys)
			{
				foreach (var relativePath in supplementaryFiles[instanceId])
				{
					using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Widgets." + relativePath.Replace("/", ".")))
					{
						UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.AddUpdateSupplementaryFile(_widgetProvider, instanceId, relativePath.Substring(relativePath.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1), stream);
					}
				}
			}

            foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Group))
            {
                //if fiji theme is found install fiji version widgets
                if (theme.Name == "7e987e474b714b01ba29b4336720c446" || theme.Name == "424eb7d9138d417b994b64bff44bf274")
                {
                    if (theme.Name == "7e987e474b714b01ba29b4336720c446")
                    {
                        definitionFiles = new string[] { 
				            "PollingPoll-Fiji-Widget.xml",
				            "PollingCreateEditPoll-Fiji-Widget.xml",
               				"PollingPollList-Fiji-Widget.xml"
			            };
                    }
                    else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
                    {
                        definitionFiles = new string[] { 
				            "PollingPoll-Enterprise-Widget.xml",
				            "PollingCreateEditPoll-Enterprise-Widget.xml",
               				"PollingPollList-Enterprise-Widget.xml"
			            };
                    }

                    foreach (var definitionFile in definitionFiles)
                    {
                        using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Widgets." + definitionFile))
                        {
                            UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.AddUpdateDefinitionFile(_widgetProvider, definitionFile, stream);
                        }
                    }

                    //Enterprise and Fiji supplementary files are identical, installing the Fiji versions for both themes.
                    supplementaryFiles = new Dictionary<Guid, string[]>();
                    supplementaryFiles[new Guid("86f521cd27fb43919261b3383c2ccb15")] = new string[] {
				        "PollingCreateEditPollFiji/ui.js"
                    };

                    supplementaryFiles[new Guid("86f521cd27fb43919261b3383c2ccb15")] = new string[] {
				        "PollingPollListFiji/pagedContent.vm"
                    };

                    foreach (var instanceId in supplementaryFiles.Keys)
                    {
                        foreach (var relativePath in supplementaryFiles[instanceId])
                        {
                            using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Widgets." + relativePath.Replace("/", ".")))
                            {
                                UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.AddUpdateSupplementaryFile(_widgetProvider, instanceId, theme.Name, relativePath.Substring(relativePath.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1), stream);
                            }
                        }
                    }
                }
            }

			#endregion

			#region Install latest version of the poll page into all group themes (and revert any configured defaults or contextul versions of these pages)
			
			XmlDocument xml;
			foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Group))
			{
                var themeName = String.Empty;
                if (theme.Name == "7e987e474b714b01ba29b4336720c446")
                    themeName = "Fiji";
                else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
                    themeName = "Enterprise";

                if (!String.IsNullOrEmpty(themeName))
                {
				    if (theme.IsConfigurationBased)
				    {
					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.createeditpoll-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateFactoryDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.poll-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateFactoryDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.polls-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateFactoryDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

					    UIApi.ThemePages.DeleteDefault(theme, "createeditpoll", true);
					    UIApi.ThemePages.DeleteDefault(theme, "poll", true);
					    UIApi.ThemePages.DeleteDefault(theme, "polls", true);
				    }
				    else
				    {
					    // non-configured-based themes don't support editing factory default pages.

					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.createeditpoll-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.poll-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

					    xml = new XmlDocument();
					    xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.polls-" + themeName + "-Groups-Page.xml"));
					    UIApi.ThemePages.AddUpdateDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));
				    }				

				    UIApi.ThemePages.Delete(theme, "createeditpoll", true);
				    UIApi.ThemePages.Delete(theme, "poll", true);
				    UIApi.ThemePages.Delete(theme, "polls", true);
			    }
            }

            xml = null;
            foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Site))
            {
                var themeName = String.Empty;
                if (theme.Name == "7e987e474b714b01ba29b4336720c446")
                    themeName = "Fiji";
                else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
                    themeName = "Enterprise";

                if (!String.IsNullOrEmpty(themeName))
                {
                    if (theme.IsConfigurationBased)
                    {
                        xml = new XmlDocument();
                        xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.user-polllist-" + themeName + "-Page.xml"));
                        UIApi.ThemePages.AddUpdateFactoryDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));

                        UIApi.ThemePages.DeleteDefault(theme, "user-polllist", true);
                    }
                    else
                    {
                        // non-configured-based themes don't support editing factory default pages.

                        xml = new XmlDocument();
                        xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages.user-polllist-" + themeName + "-Page.xml"));
                        UIApi.ThemePages.AddUpdateDefault(theme, xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage"));
                    }

                    UIApi.ThemePages.Delete(theme, "user-polllist", true);
               }
           }

			#endregion

			#region Install CSS Files

			foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Site))
			{
			    if (theme.IsConfigurationBased)
			    {
			        var themeName = "Social";
			        if (theme.Name == "7e987e474b714b01ba29b4336720c446")
			            themeName = "Fiji";
			        else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
			            themeName = "Enterprise";

			        if (themeName != "Social")
			        {
			            using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Css.polling-" + themeName + ".css"))
			            {
			                if (stream != null)
			                {
			                    UIApi.ThemeFiles.AddUpdateFactoryDefault(theme, UIApi.ThemeProperties.CssFiles, "polling.css", stream, (int) stream.Length);
			                    stream.Seek(0, System.IO.SeekOrigin.Begin);
			                    UIApi.ThemeFiles.AddUpdate(theme, UIApi.ThemeableApplicationIds.Site, UIApi.ThemeProperties.CssFiles, "polling.css", stream, (int) stream.Length);
			                }
			            }
			        }

			        else
			        {
                        using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Css.polling-" + themeName + ".css"))
                        {
			                if (stream != null)
			                {
                                UIApi.ThemeFiles.AddUpdateFactoryDefault(theme, UIApi.ThemeProperties.CssFiles, "polling.css", stream, (int)stream.Length);
                                stream.Seek(0, System.IO.SeekOrigin.Begin);
                                UIApi.ThemeFiles.AddUpdate(theme, UIApi.ThemeableApplicationIds.Site, UIApi.ThemeProperties.CssFiles, "polling.css", stream, (int)stream.Length);
			                }
			            }

                        //using (var stream = InternalApi.EmbeddedResources.GetStream("Telligent.BigSocial.Polling.Resources.Css.polling-handheld-" + themeName + ".css"))
                        //{
                        //    if (stream != null)
                        //    {
                        //        UIApi.ThemeFiles.AddUpdateFactoryDefault(theme, UIApi.ThemeProperties.CssFiles, "polling-handheld.css", stream, (int) stream.Length,
                        //            new UIApi.CssThemeFileOptions() { MediaQuery = "(max-width: 570px)", ApplyToModals = true, ApplyToNonModals = true });
                        //        stream.Seek(0, System.IO.SeekOrigin.Begin);
                        //        UIApi.ThemeFiles.AddUpdate(theme, UIApi.ThemeableApplicationIds.Site, UIApi.ThemeProperties.CssFiles, "polling-handheld.css", stream, (int) stream.Length,
                        //            new UIApi.CssThemeFileOptions() { MediaQuery = "(max-width: 570px)", ApplyToModals = true, ApplyToNonModals = true });
                        //    }
                        //}
			        }
			    }
			}

			#endregion
        }

		public void Uninstall()
		{
			InternalApi.PollingDataService.UnInstall();

            #region Remove Content from search
            try
            {
                //Remove Ideas and Challenges from search, mark all for reindexing when plugin is reenabled.
                TEApi.SearchIndexing.Delete(new SearchIndexDeleteOptions { Category = "polls" });
                InternalApi.PollingDataService.ReIndexAllPolls();
            }
            catch (Exception ex)
            {
                var csex = new CSException(CSExceptionType.PluginInitializationError, "There was an error removing polling content from search", ex);
                csex.Log();
            }
            #endregion

            #region Delete custom pages used to support polls (from factory defaults, configured defaults, and contextual pages)

		    foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Group))
		    {
		        var themeName = String.Empty;
		        if (theme.Name == "7e987e474b714b01ba29b4336720c446")
		            themeName = "Fiji";
		        else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
		            themeName = "Enterprise";

		        if (!String.IsNullOrEmpty(themeName))
		        {
		            if (theme.IsConfigurationBased)
		            {
		                UIApi.ThemePages.DeleteFactoryDefault(theme, "createeditpoll", true);
		                UIApi.ThemePages.DeleteFactoryDefault(theme, "poll", true);
		                UIApi.ThemePages.DeleteFactoryDefault(theme, "polls", true);
		            }

		            UIApi.ThemePages.DeleteDefault(theme, "createeditpoll", true);
		            UIApi.ThemePages.DeleteDefault(theme, "poll", true);
		            UIApi.ThemePages.DeleteDefault(theme, "polls", true);

		            UIApi.ThemePages.Delete(theme, "createeditpoll", true);
		            UIApi.ThemePages.Delete(theme, "poll", true);
		            UIApi.ThemePages.Delete(theme, "polls", true);
		        }
		    }

		    foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Site))
            {
                var themeName = String.Empty;
                if (theme.Name == "7e987e474b714b01ba29b4336720c446")
                    themeName = "Fiji";
                else if (theme.Name == "424eb7d9138d417b994b64bff44bf274")
                    themeName = "Enterprise";

                if (!String.IsNullOrEmpty(themeName))
                {
                    if (theme.IsConfigurationBased)
                    {
                        UIApi.ThemePages.DeleteFactoryDefault(theme, "user-pollist", true);
                    }

                    UIApi.ThemePages.DeleteDefault(theme, "user-pollist", true);

                    UIApi.ThemePages.Delete(theme, "user-pollist", true);
                }
            }

			#endregion

			#region Remove Widget Files

			UIApi.FactoryDefaultScriptedContentFragmentProviderFiles.DeleteAllFiles(_widgetProvider);

			#endregion

			#region Uninstall CSS Files

			foreach (var theme in UIApi.Themes.List(UIApi.ThemeTypes.Site))
			{
				if (theme.IsConfigurationBased)
				{
					UIApi.ThemeFiles.Remove(theme, UIApi.ThemeableApplicationIds.Site, UIApi.ThemeProperties.CssFiles, "polling.css");
					UIApi.ThemeFiles.RemoveFactoryDefault(theme, UIApi.ThemeProperties.CssFiles, "polling.css");

                    //UIApi.ThemeFiles.Remove(theme, UIApi.ThemeableApplicationIds.Site, UIApi.ThemeProperties.CssFiles, "polling-handheld.css");
                    //UIApi.ThemeFiles.RemoveFactoryDefault(theme, UIApi.ThemeProperties.CssFiles, "polling-handheld.css");
                }
			}

			#endregion
		}

		public Version Version
		{
			get { return GetType().Assembly.GetName().Version; }
		}

		#endregion

		#region IPluginGroup Members

		public IEnumerable<Type> Plugins
		{
			get 
			{
				return new Type[] { 
					typeof(PollContentType),
					typeof(PollSearchCategories),
					typeof(PollRestEndpoints),
					typeof(PollWidgetExtension),
					typeof(PollAnswerWidgetExtension),
					typeof(PollVoteWidgetExtension),
					typeof(PollUrlsWidgetExtension),
					typeof(PollWidgetContextProvider),
					typeof(PollFactoryDefaultWidgetProvider),
					typeof(PollGroupNavigation),
					typeof(PollViewer),
					typeof(PollHeaderExtension),
					typeof(PollNewPostLink),
					typeof(PollVotesMetric),
					typeof(TopPollsScore),
                    typeof(PollUserNavigation)		
                };
			}
		}

		#endregion

        #region IApplicationNavigable Members

        Guid IApplicationNavigable.ApplicationTypeId
        {
            get { return TEApi.Groups.ApplicationTypeId; }
        }

        public void RegisterUrls(IUrlController controller)
        {
            const string guidConstraintRegex = @"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}";
            object pollIdConstraints = new { PollId = guidConstraintRegex };
            object answerIdConstraints = new { PollId = guidConstraintRegex, PollAnswerId = guidConstraintRegex };

            controller.AddPage("polls_list", "polls", null, null, "polls", new PageDefinitionOptions() { DefaultPageXml = LoadPageXml("polls-Social-Groups-Page") });
            controller.AddPage("polls_create", "polls/create", null, null, "createeditpoll", new PageDefinitionOptions() { DefaultPageXml = LoadPageXml("createeditpoll-Social-Groups-Page") });

            controller.AddPage("polls_view", "polls/{PollId}", null, pollIdConstraints, "poll", new PageDefinitionOptions() { ParseContext = ParsePollContext, DefaultPageXml = LoadPageXml("poll-Social-Groups-Page") });
            controller.AddPage("polls_edit", "polls/edit/{PollId}", null, pollIdConstraints, "createeditpoll", new PageDefinitionOptions() { ParseContext = ParsePollContext, DefaultPageXml = LoadPageXml("createeditpoll-Social-Groups-Page") });

            controller.AddRaw("polls_vote", "polls/{PollId}/vote/{PollAnswerId}", null, answerIdConstraints,
                (a, p) =>
                {
                    HandlePollVote(a.ApplicationInstance.Context);
                }, new RawDefinitionOptions() { ParseContext = ParsePollContext });
        }

        private void ParsePollContext(PageContext context)
        {
            var pollId = context.GetTokenValue("PollId");
            
            ContextItem contextItem = null;
            if (pollId != null)
            {
                var id = Guid.Parse(pollId.ToString());
                var poll = PublicApi.Polls.Get(id);
                if (poll != null && !poll.HasErrors())
                    contextItem = BuildPollContextItem(poll);
            }

            if (contextItem != null)
                context.ContextItems.Put(contextItem);
        }

        private ContextItem BuildPollContextItem(PublicApi.Poll poll)
        {
            var item = new ContextItem()
            {
                TypeName = "Poll",
                ApplicationId = poll.Group.ApplicationId,
                ApplicationTypeId = Telligent.Evolution.Components.ContentTypes.Group,
                ContainerId = poll.Group.ContainerId,
                ContainerTypeId = Telligent.Evolution.Components.ContentTypes.Group,
                ContentId = poll.ContentId,
                ContentTypeId = PublicApi.Polls.ContentTypeId,
                Id = poll.ContentId.ToString()
            };
            return item;
        }

        public void HandlePollVote(System.Web.HttpContext context)
        {
            var user = TEApi.Users.AccessingUser;

            if (user.IsSystemAccount.HasValue && user.IsSystemAccount.Value)
            {
                context.Response.RedirectLocation = TEApi.CoreUrls.LogIn(new CoreUrlLoginOptions() { ReturnToCurrentUrl = true });
                context.Response.StatusCode = 302;
                context.Response.End();
            }

            var pollContextItem = TEApi.Url.CurrentContext.ContextItems.GetItemByContentType(PublicApi.Polls.ContentTypeId);
            Poll poll = null;
            if (pollContextItem != null && pollContextItem.ContentId.HasValue)
                poll = InternalApi.PollingService.GetPoll(pollContextItem.ContentId.Value);

            if (poll == null)
                throw new PollException(_translatablePluginController.GetLanguageResourceValue("poll_NotFound"));

            var pollAnswerIdObj = TEApi.Url.CurrentContext.GetTokenValue("PollAnswerId");
            Guid pollAnswerId;
            if (pollAnswerIdObj != null && Guid.TryParse(pollAnswerIdObj.ToString(), out pollAnswerId))
            {
                var answer = InternalApi.PollingService.GetPollAnswer(pollAnswerId);
                if (answer != null)
                {
                    try
                    {
                        InternalApi.PollingService.AddUpdatePollVote(new Telligent.BigSocial.Polling.InternalApi.PollVote() { PollId = poll.Id, UserId = user.Id.Value, PollAnswerId = pollAnswerId });

                        var url = TEApi.Url.Adjust(InternalApi.PollingUrlService.PollUrl(poll.Id), "voted=" + pollAnswerId.ToString());
                        context.Response.RedirectLocation = url;
                        context.Response.StatusCode = 302;
                        context.Response.End();
                    }
                    catch (Exception ex)
                    {
                        throw new PollException(_translatablePluginController.GetLanguageResourceValue("vote_UnknownError"), ex);
                    }
                }
                else
                {
                    var url = TEApi.Url.Adjust(InternalApi.PollingUrlService.PollUrl(poll.Id), "votefailed=true");
                    context.Response.RedirectLocation = url;
                    context.Response.StatusCode = 302;
                    context.Response.End();
                }
            }
        }

        private static string LoadPageXml(string pageName)
        {
            var xml = new XmlDocument();
            xml.LoadXml(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Pages." + pageName + ".xml"));
            var xmlNode = xml.SelectSingleNode("theme/contentFragmentPages/contentFragmentPage");
            return xmlNode != null ? xmlNode.OuterXml : String.Empty;
        }
        #endregion

        #region INavigable Members
        void INavigable.RegisterUrls(IUrlController controller)
        {
            object userNameConstraint = new { UserName = @"^[a-zA-Z0-9\-\._]+$" };

            controller.AddPage("poll_userpolls", "members/{UserName}/polls", null, userNameConstraint, "user-polllist", new PageDefinitionOptions() { ParseContext = ParseUserContext, DefaultPageXml = LoadPageXml("user-polllist-Social-Page") });
        }

        private void ParseUserContext(PageContext context)
        {
            var userName = context.GetTokenValue("UserName");
            ContextItem contextItem = null;

            if (userName != null)
            {
                var user = TEApi.Users.Get(new UsersGetOptions() { Username = userName.ToString() });
                if (user != null)
                    contextItem = BuildUserContextItem(user);
                else
                    Globals.RedirectToLoginOrThrowException(Telligent.Evolution.Components.CSExceptionType.UserNotFound);
            }

            if (contextItem != null)
                context.ContextItems.Put(contextItem);
        }

        private ContextItem BuildUserContextItem(Telligent.Evolution.Extensibility.Api.Entities.Version1.User user)
        {
            var item = new ContextItem()
            {
                TypeName = "User",
                ApplicationId = user.ContentId,
                ApplicationTypeId = TEApi.Users.ContentTypeId,
                ContainerId = user.ContentId,
                ContainerTypeId = TEApi.Users.ContentTypeId,
                ContentId = user.ContentId,
                ContentTypeId = TEApi.Users.ContentTypeId,
                Id = user.Id.ToString()
            };
            return item;
        }
        #endregion

        #region ITranslatablePlugin Members

        Translation[] defaultTranslations;

        public Translation[] DefaultTranslations
        {
            get
            {
                if (defaultTranslations == null)
                {
                    var t = new Translation("en-us");

                    t.Set("poll_NotFound", "Poll Not Found.  The poll may have been deleted or you may no longer have permission to view the poll.");
                    t.Set("vote_UnknownError", "An error occurred recoding your vote.  Please visit the site and vote on the poll directly.");
                    t.Set("Permission_Polling_ReadPolls", "Polling - Read Polls");
                    t.Set("Permission_Polling_ReadPolls_Description", "This permission determines who can view polls.");
                    t.Set("Permission_Polling_VoteOnPolls", "Polling - Vote on Polls");
                    t.Set("Permission_Polling_VoteOnPolls_Description", "This permission determines who can vote on polls.");
                    t.Set("Permission_Polling_CreatePolls", "Polling - Create Polls");
                    t.Set("Permission_Polling_CreatePolls_Description", "This permission determines who can create polls.");
                    t.Set("Permission_Polling_ModeratePolls", "Polling - Moderate Polls");
                    t.Set("Permission_Polling_ModeratePolls_Description", "This permission determines who can moderate polls.");

                    defaultTranslations = new[] { t };
                }
                return defaultTranslations;
            }
        }

        public void SetController(ITranslatablePluginController controller)
        {
            _translatablePluginController = controller;
        }

        #endregion

        #region IPermissionRegistrar
        void IPermissionRegistrar.RegisterPermissions(IPermissionRegistrarController permissionController)
        {
            permissionController.Register(
                new Permission(
                    PollPermissions.ReadPolls,
                    "Permission_Polling_ReadPolls",
                    "Permission_Polling_ReadPolls_Description",
                    _translatablePluginController,
                    TEApi.Groups.ApplicationTypeId,
                    new PermissionConfiguration()
                    {
                        Joinless = new JoinlessGroupPermissionConfiguration { Administrators = true, Moderators = true, Owners = true, RegisteredUsers = true, Everyone = true },
                        PublicOpen = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true, RegisteredUsers = true, Everyone = true },
                        PublicClosed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true, RegisteredUsers = true, Everyone = true },
                        PrivateListed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PrivateUnlisted = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true }
                    }));
            permissionController.Register(
                new Permission(
                    PollPermissions.CreatePolls,
                    "Permission_Polling_CreatePolls",
                    "Permission_Polling_CreatePolls_Description",
                    _translatablePluginController,
                    TEApi.Groups.ApplicationTypeId,
                    new PermissionConfiguration()
                    {
                        Joinless = new JoinlessGroupPermissionConfiguration { Administrators = true, Moderators = true, Owners = true, RegisteredUsers = true },
                        PublicOpen = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PublicClosed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PrivateListed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PrivateUnlisted = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true }
                    }));

            permissionController.Register(
                new Permission(
                    PollPermissions.VoteOnPolls,
                    "Permission_Polling_VoteOnPolls",
                    "Permission_Polling_VoteOnPolls_Description",
                    _translatablePluginController,
                    TEApi.Groups.ApplicationTypeId,
                    new PermissionConfiguration()
                    {
                        Joinless = new JoinlessGroupPermissionConfiguration { Administrators = true, Moderators = true, Owners = true, RegisteredUsers = true },
                        PublicOpen = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PublicClosed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PrivateListed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true },
                        PrivateUnlisted = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true, Members = true }
                    }));
            permissionController.Register(
                new Permission(
                    PollPermissions.ModeratePolls,
                    "Permission_Polling_ModeratePolls",
                    "Permission_Polling_ModeratePolls_Description",
                    _translatablePluginController,
                    TEApi.Groups.ApplicationTypeId,
                    new PermissionConfiguration()
                    {
                        Joinless = new JoinlessGroupPermissionConfiguration { Administrators = true, Moderators = true, Owners = true },
                        PublicOpen = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true },
                        PublicClosed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true },
                        PrivateListed = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true },
                        PrivateUnlisted = new MembershipGroupPermissionConfiguration { Owners = true, Managers = true }
                    }));
        }
        #endregion

        #region ICategorizedPlugin
        string[] ICategorizedPlugin.Categories
        {
            get { return new[] { "Polling", "Translatable" }; }
        }
        #endregion
    }
}