using System;
using System.Linq;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.Content.Version1;
using Telligent.Evolution.Extensibility.Email.Version1;
using Telligent.Evolution.Extensibility.Templating.Version1;
using Telligent.Evolution.Extensibility.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.BigSocial.Polling.InternalApi;

namespace Telligent.BigSocial.Polling.Plugins
{
    public class AuthorPollVoteNotification : INotificationType, ITranslatablePlugin, ICategorizedPlugin, ITemplatablePlugin, IEmailTemplatePreviewPlugin, IEmailNotificationType
    {
        private static readonly Guid _notificationTypeId = new Guid("0183e454-3ee7-4973-9167-caaa3d8fbd5b");
        private ITranslatablePluginController _translatablePluginController;
        private INotificationController _notificationController;
        private ITemplatablePluginController _templatablePluginController;

        #region IPlugin Members

        public string Name
        {
            get { return "Big Social Polling - Author Vote Notifications"; }
        }

        public string Description
        {
            get { return "Raises notifications to a poll author when a user votes on their poll"; }
        }

        public void Initialize()
        {
            PublicApi.PollVotes.Events.AfterCreate += new PublicApi.PollVoteAfterCreateEventHandler(PollVoteEvents_AfterCreate);
            PublicApi.PollVotes.Events.AfterDelete += new PublicApi.PollVoteAfterDeleteEventHandler(PollVoteEvents_AfterDelete);
        }

        void PollVoteEvents_AfterCreate(PublicApi.PollVoteAfterCreateEventArgs e)
        {
            var poll = PublicApi.Polls.Get(e.PollId);

            if (poll == null || !poll.Author.Id.HasValue || poll.Author.Id.Value == e.UserId)
                return;

            AddNotification(e.PollId, PublicApi.Polls.ContentTypeId, poll.Author.Id.Value, e.UserId);
        }

        void PollVoteEvents_AfterDelete(PublicApi.PollVoteAfterDeleteEventArgs e)
        {
            var poll = PublicApi.Polls.Get(e.PollId);

            if (poll == null || !poll.Author.Id.HasValue || poll.Author.Id.Value == e.UserId)
                return;

            DeleteNotification(e.PollId, PublicApi.Polls.ContentTypeId, poll.Author.Id.Value, e.UserId);
        }

        private void AddNotification(Guid contentId, Guid contentTypeId, int userId, int actorId)
        {
            var contentType = TEApi.ContentTypes.Get(contentTypeId);
            if (contentType == null)
                return;

            _notificationController.CreateUpdate(new NotificationCreateUpdateOptions
            {
                ContentId = contentId,
                ContentTypeId = contentTypeId,
                LastUpdate = DateTime.UtcNow,
                UserId = userId,
                MaxActorsPerNotification = 10,
                ActorIdToAdd = actorId
            });
        }

        private void DeleteNotification(Guid contentId, Guid contentTypeId, int userId, int actorId)
        {
            _notificationController.CreateUpdate(new NotificationCreateUpdateOptions
            {
                ContentId = contentId,
                ContentTypeId = contentTypeId,
                LastUpdate = DateTime.UtcNow,
                UserId = userId,
                ActorIdToRemove = actorId
            });
        }

        #endregion

        #region INotificationType Members

        public string NotificationTypeName
        {
            get { return _translatablePluginController.GetLanguageResourceValue("name"); }
        }

        public string NotificationTypeDescription
        {
            get { return _translatablePluginController.GetLanguageResourceValue("description"); }
        }

        public Guid NotificationTypeId
        {
            get { return _notificationTypeId; }
        }

        public string NotificationTypeCategory
        {
            get { return _translatablePluginController.GetLanguageResourceValue("category"); }
        }

        public void SetController(INotificationController controller)
        {
            _notificationController = controller;
        }

        public bool CanDeleteNotification(Guid notificationId, int userId)
        {
            var notification = TEApi.Notifications.Get(notificationId);
            return notification != null && notification.UserId == userId;
        }

        public string GetMessage(Guid notificationId, string target)
        {
            return GetContentMessage(notificationId, target, _translatablePluginController);
        }

        public bool IsCacheable
        {
            get { return true; }
        }

        public bool VaryCacheByUser
        {
            get { return true; }
        }

        public string GetTargetUrl(Guid notificationId)
        {
            return GetContentUrl(notificationId);
        }

        public string GetContentMessage(Guid notificationId, string target, ITranslatablePluginController _translatablePluginController)
        {
            var notification = TEApi.Notifications.Get(notificationId);
            if (notification == null)
                return null;

            var content = TEApi.Content.Get(notification.ContentId.GetValueOrDefault(), notification.ContentTypeId.GetValueOrDefault());
            if (content == null)
                return null;

            if (target == "ShortText")
            {
                if (notification.Actors.Count() == 1)
                {
                    var latestUser = TEApi.Users.Get(new UsersGetOptions { Id = notification.Actors.First().UserId });

                    return String.Format(_translatablePluginController.GetLanguageResourceValue("short_singular"),
                        latestUser.DisplayName,
                        TEApi.Language.Truncate(content.HtmlName("Web"), 30, "…"));
                }
                return String.Format(_translatablePluginController.GetLanguageResourceValue("short_multiple"),
                    notification.Actors.Count().ToString(),
                    TEApi.Language.Truncate(content.HtmlName("Web"), 30, "…"));
            }
            if (notification.Actors == null || notification.Actors.Any())
                return null;

            var actors = notification.Actors.OrderByDescending(a => a.Date);

            var firstTwoActorUsers = actors
                .Take(2)
                .Select(a => TEApi.Users.Get(new UsersGetOptions { Id = a.UserId }))
                .ToList();

            if (actors.Count() == 1)
                return String.Format(_translatablePluginController.GetLanguageResourceValue("html_one"),
                    GetUserLink(firstTwoActorUsers[0], _translatablePluginController),
                    System.Web.HttpUtility.HtmlAttributeEncode(content.Url),
                    TEApi.Language.Truncate(content.HtmlName("Web"), 50, "…"));
            if (actors.Count() == 2)
                return String.Format(_translatablePluginController.GetLanguageResourceValue("html_two"),
                    GetUserLink(firstTwoActorUsers[0], _translatablePluginController),
                    GetUserLink(firstTwoActorUsers[1], _translatablePluginController),
                    System.Web.HttpUtility.HtmlAttributeEncode(content.Url),
                    TEApi.Language.Truncate(content.HtmlName("Web"), 50, "…"));
            if (actors.Count() == 3)
                return String.Format(_translatablePluginController.GetLanguageResourceValue("html_more_singular"),
                    GetUserLink(firstTwoActorUsers[0], _translatablePluginController),
                    GetUserLink(firstTwoActorUsers[1], _translatablePluginController),
                    System.Web.HttpUtility.HtmlAttributeEncode(content.Url),
                    TEApi.Language.Truncate(content.HtmlName("Web"), 50, "…"));
            if (actors.Count() > 3)
                return String.Format(_translatablePluginController.GetLanguageResourceValue("html_more_plural"),
                    GetUserLink(firstTwoActorUsers[0], _translatablePluginController),
                    GetUserLink(firstTwoActorUsers[1], _translatablePluginController),
                    (actors.Count() - 2).ToString(),
                    System.Web.HttpUtility.HtmlAttributeEncode(content.Url),
                    TEApi.Language.Truncate(content.HtmlName("Web"), 50, "…"));

            return null;
        }

        public string GetContentUrl(Guid notificationId)
        {
            var notification = TEApi.Notifications.Get(notificationId);
            if (notification == null)
                return null;

            var content = TEApi.Content.Get(notification.ContentId.GetValueOrDefault(),
                notification.ContentTypeId.GetValueOrDefault());
            if (content == null)
                return null;

            return content.Url;
        }

        public string GetUserLink(User user, ITranslatablePluginController translatablePluginController)
        {
            if (user == null)
                return null;

            // if no access to profile url, just use the display name
            if (String.IsNullOrWhiteSpace(user.ProfileUrl))
                return user.DisplayName;

            // otherwise, template the user as a user link
            return String.Format(translatablePluginController.GetLanguageResourceValue("html_user_link"),
                System.Web.HttpUtility.HtmlAttributeEncode(user.ProfileUrl),
                user.DisplayName);
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
                    var enUs = new Translation("en-us");

                    enUs.Set("category", "Activity related to content you write");
                    enUs.Set("name", "Poll Author Votes");
                    enUs.Set("description", "Votes on polls I created");

                    enUs.Set("short_singular", "{0} voted on {1}.");
                    enUs.Set("short_multiple", "{0} people voted on {1}.");

                    enUs.Set("html_one", @"{0} voted on <a href=""{1}"" class=""view-post"">{2}</a>.");
                    enUs.Set("html_two", @"{0} and {1} voted on <a href=""{2}"" class=""view-post"">{3}</a>.");
                    enUs.Set("html_more_singular", @"{0}, {1} and one other voted <a href=""{2}"" class=""view-post"">{3}</a>.");
                    enUs.Set("html_more_plural", @"{0}, {1} and {2} others voted on <a href=""{3}"" class=""view-post"">{4}</a>.");

                    enUs.Set("html_user_link", @"<a href=""{0}"" class=""internal-link view-user-profile"">{1}</a>");

                    defaultTranslations = new[] { enUs };
                }
                return defaultTranslations;
            }
        }

        public void SetController(ITranslatablePluginController controller)
        {
            _translatablePluginController = controller;
        }

        #endregion

        #region ICategorizedPlugin Members
        string[] ICategorizedPlugin.Categories
        {
            get { return new string[] { "Polling", "Notifications", "Translatable", "Templatable", "Email" }; ; }
        }
        #endregion

        TokenizedTemplate[] ITemplatablePlugin.DefaultTemplates
        {
            get { throw new NotImplementedException(); }
        }

        public void SetController(ITemplatablePluginController controller)
        {
            _templatablePluginController = controller;
        }

        string IEmailNotificationType.Get(EmailTarget target, Notification notification)
        {
            if (notification == null)
                return string.Empty;

            switch (target)
            {
                case EmailTarget.Subject:
                    return RenderTemplate("email_subject", notification);
                case EmailTarget.Header:
                    return RenderTemplate("email_header", notification);
                case EmailTarget.Footer:
                    return RenderTemplate("email_footer", notification);
                case EmailTarget.Body:
                    return RenderTemplate("email_subject", notification);
            }

            return string.Empty;
        }

        private string RenderTemplate(string templateName, Notification notification)
        {
            var content = TEApi.Content.Get(notification.ContentId.GetValueOrDefault(), notification.ContentTypeId.GetValueOrDefault());
            if (content == null)
                return null;

            var tokenContext = new TemplateContext();
            tokenContext.AddItem(Telligent.Evolution.Api.Content.ContentTypes.GenericContent, notification.Content);
            //tokenContext.AddItem(PublicApi.Comments.ContentTypeId, comment);
            return _templatablePluginController.RenderTokenString(templateName, tokenContext);
        }

        #region IEmailTemplatePreviewPlugin Members
        string IEmailTemplatePreviewPlugin.GetTemplateName(EmailTarget target)
        {
            switch (target)
            {
                case EmailTarget.Subject:
                    return "email_subject";
                case EmailTarget.Header:
                    return "email_header";
                case EmailTarget.Footer:
                    return "email_footer";
                case EmailTarget.Body:
                    return "email_subject";
            }

            return string.Empty;
        }
        #endregion

    }
}
