using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.InternalApi
{
	static internal class Formatting
	{
		static internal DateTime FromUtcToUserTime(DateTime utcDate)
		{
			return System.TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.FindSystemTimeZoneById(TEApi.Users.AccessingUser.TimeZoneId));
		}

		static internal DateTime FromUserTimeToUtc(DateTime userDate)
		{
			return TimeZoneInfo.ConvertTimeToUtc(userDate, TimeZoneInfo.FindSystemTimeZoneById(TEApi.Users.AccessingUser.TimeZoneId));
		}
	}
}
