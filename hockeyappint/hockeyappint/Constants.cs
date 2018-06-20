using System;
namespace hockeyappint
{
	public class Constants
	{
		public static string HockeyAndroidAPPKey { get; set; } = "PUT_YOUR_ANDROID_HOCKEYAPP_KEY";
		public static string HockeyiOSAPPKey { get; set; } = "PUT_YOUR_IOS_HOCKEYAPP_KEY";
		public static string HockeyAndroidAppUrl = string.Format("https://rink.hockeyapp.net/api/2/apps/{0}/", HockeyAndroidAPPKey);
		public static string HockeyIOSAppUrl = string.Format("https://rink.hockeyapp.net/api/2/apps/{0}/", HockeyiOSAPPKey);
	}
}
