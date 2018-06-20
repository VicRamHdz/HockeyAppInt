using System;
using hockeyappint.Models;

namespace hockeyappint.DependencyService
{
	public interface IDeviceInformation
	{
		string GetPhoneNumber();
		string GetModel();
		string GetLanguage();
		string GetDisplay();
		string GetSimId();
		string GetOSVersion();
		string GetIPAddress();
		GeoLocationModel GetGeoLocation(bool IncludeHeading = false);
		string GetDeviceId();
		bool GetIsEmulator();
		bool GetIsCompromised();
		string GetApplicationKey();
		string GetApplicationVersionNumber();
		void ExecuteLocationManager(bool IncludeHeading = false);
		string GetApplicationName();
	}
}
