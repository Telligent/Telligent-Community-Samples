using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using Telligent.DynamicConfiguration.Components;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Urls.Routing;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
    public class PollUserNavigation : IPlugin, ITranslatablePlugin, IUserCustomNavigationPlugin, IUserDefaultCustomNavigationPlugin
    {
        private readonly Guid _defaultId = new Guid("4e622c53-afd7-4e53-8773-f8b1d63a63fd");

        ITranslatablePluginController _translation;

        #region IPlugin
        public string Name
        {
            get { return "Poll User Navigation"; }
        }

        public string Description
        {
            get { return "Adds poll custom navigation support within user profile."; }
        }

        public void Initialize()
        {

        }
        #endregion

        #region IUserDefaultCustomNavigationPlugin Members
        public int DefaultOrderNumber
        {
            get { return 200; }
        }

        public ICustomNavigationItem GetDefaultNavigationItem(ICustomNavigationItemConfiguration configuration)
        {
            Telligent.Evolution.Extensibility.Api.Entities.Version1.User user = null;
            if (configuration.GetStringValue("user", "current") == "accessing")
                user = TEApi.Users.AccessingUser;
            else
            {
                var userItem = TEApi.Url.CurrentContext.ContextItems.GetItemByContentType(TEApi.Users.ContentTypeId);
                if (userItem != null)
                    user = TEApi.Users.Get(new UsersGetOptions() { ContentId = userItem.ContentId });
            }

            return new UserPollsNavigationItem(this, configuration, _defaultId, user.Id.Value, () => _translation.GetLanguageResourceValue("configuration_defaultLabel"));
        }
        #endregion

        #region IUserCustomNavigationPlugin Members
        public ICustomNavigationItem GetNavigationItem(Guid id, ICustomNavigationItemConfiguration configuration)
        {
            Telligent.Evolution.Extensibility.Api.Entities.Version1.User user = null;
            if (configuration.GetStringValue("user", "current") == "accessing")
                user = TEApi.Users.AccessingUser;
            else
            {
                var userItem = TEApi.Url.CurrentContext.ContextItems.GetItemByContentType(TEApi.Users.ContentTypeId);
                if (userItem != null)
                    user = TEApi.Users.Get(new UsersGetOptions() { ContentId = userItem.ContentId });
            }

            return new UserPollsNavigationItem(this, configuration, id, user.Id.Value, () => _translation.GetLanguageResourceValue("configuration_defaultLabel"));
        }

        public string NavigationTypeName
        {
            get { return _translation.GetLanguageResourceValue("navigation_type_name"); }
        }

        public PropertyGroup[] GetConfigurationProperties()
        {
            PropertyGroup group = new PropertyGroup("config", "", 1);

            Property p = new Property("user", _translation.GetLanguageResourceValue("link_to_user"), PropertyType.String, 1, "current");
            p.SelectableValues.Add(new PropertyValue("current", _translation.GetLanguageResourceValue("link_to_user_current"), 0));
            p.SelectableValues.Add(new PropertyValue("accessing", _translation.GetLanguageResourceValue("link_to_user_accessing"), 1));
            group.Properties.Add(p);

            p = new Property("label", _translation.GetLanguageResourceValue("label"), PropertyType.String, 2, "Polls");
            p.ControlType = typeof(Telligent.Evolution.Controls.ContentFragmentTokenStringControl);
            group.Properties.Add(p);

            return new PropertyGroup[] { group };
        }
        #endregion

        #region ITranslatablePlugin Members
        public Translation[] DefaultTranslations
        {
            get
            {
                var translation = new Translation("en-US");
                translation.Set("configuration_options", "Options");
                translation.Set("configuration_label", "Label");
                translation.Set("configuration_label_description", "Enter an optional label for this link.");
                translation.Set("configuration_defaultLabel", "Polls");
                translation.Set("navigationitem_name", "User Polls");

                return new Translation[] { translation };
            }
        }

        public void SetController(ITranslatablePluginController controller)
        {
            _translation = controller;
        }
        #endregion

        public class UserPollsNavigationItem : ICustomNavigationItem
        {
            Func<string> _getLabel;
            int _userId;

            internal UserPollsNavigationItem(ICustomNavigationPlugin plugin, ICustomNavigationItemConfiguration configuration, Guid id, int userId, Func<string> getLabel)
            {
                Plugin = plugin;
                Configuration = configuration;
                UniqueID = id;
                _userId = userId;
                _getLabel = getLabel;
            }
            #region ICustomNavigationItem Members

            public ICustomNavigationItem[] Children { get; set; }
            public ICustomNavigationItemConfiguration Configuration { get; private set; }
            public string CssClass { get { return "list-polls"; } }
            public string Label { get { return _getLabel(); } }
            public ICustomNavigationPlugin Plugin { get; private set; }
            public Guid UniqueID { get; private set; }

            public bool IsSelected(string currentFullUrl)
            {
                return TEApi.Url.CurrentContext.UrlName == "poll_userpolls";
            }

            public bool IsVisible(int userId)
            {
                return !string.IsNullOrEmpty(Url);
            }

            public string Url
            {
                get
                {
                    return InternalApi.PollingUrlService.UserPolls(_userId);
                }
            }

            #endregion
        }
    }
}
