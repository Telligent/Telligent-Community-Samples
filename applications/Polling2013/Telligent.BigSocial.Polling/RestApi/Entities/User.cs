using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.RestApi
{
	public class User
	{
		public User()
		{
		}

		internal User(int userId)
		{
			var user = TEApi.Users.Get(new UsersGetOptions { Id = userId });
			if (user != null && !user.HasErrors())
			{
				DisplayName = user.DisplayName;
				Username = user.Username;
				ProfileUrl = TEApi.Url.Absolute(user.ProfileUrl);
				AvatarUrl = TEApi.Url.Absolute(user.AvatarUrl);
				UserId = user.Id.Value;
			}
		}

		public string DisplayName { get; set; }
		public string Username { get; set; }
		public string ProfileUrl { get; set; }
		public string AvatarUrl { get; set; }
		public int UserId { get; set; }
	}
}
