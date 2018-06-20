using System;
using CoreLocation;
using Foundation;
using hockeyappint.DependencyService;
using hockeyappint.iOS.DependencyService;
using hockeyappint.Models;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInformation))]
namespace hockeyappint.iOS.DependencyService
{
	public class DeviceInformation : IDeviceInformation
	{
		CLLocationManager locationManager;

		public void ExecuteLocationManager(bool IncludeHeading = false)
		{
			locationManager = new CLLocationManager();
			locationManager.AuthorizationChanged += (NSURLAuthenticationChallengeSender, args) =>
			{

			};
			locationManager.DesiredAccuracy = 1000;

			locationManager.ShouldDisplayHeadingCalibration += (CLLocationManager manager) => { return IncludeHeading && !CLLocationManager.HeadingAvailable; };

			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				locationManager.RequestWhenInUseAuthorization();
			}

			if (CLLocationManager.LocationServicesEnabled)
			{
				locationManager.StartUpdatingLocation();
			}
			if (IncludeHeading && CLLocationManager.HeadingAvailable)
			{
				locationManager.StartUpdatingHeading();
			}
		}

		public string GetApplicationKey()
		{
			string deviceIDValue = "";

			//TODO: return sha1 code

			return deviceIDValue;
		}

		public string GetApplicationName()
		{
			var appName = NSBundle.MainBundle.InfoDictionary["CFBundleName"]?.ToString();
			return appName;
		}

		public string GetApplicationVersionNumber()
		{
			string version = NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();
			return version;
		}

		public string GetDeviceId()
		{
			return UIDevice.CurrentDevice.GetNativeHash().ToString();
		}

		public string GetDisplay()
		{
			return UIScreen.MainScreen.Bounds.Width + "|" + UIScreen.MainScreen.Bounds.Height;
		}

		public GeoLocationModel GetGeoLocation(bool IncludeHeading = false)
		{
			GeoLocationModel geoLocation = new GeoLocationModel();
			locationManager = new CLLocationManager();

			// you can also set the desired accuracy:
			locationManager.DesiredAccuracy = 1000; // 1000 meters/1 kilometer
													// you can also use presets, which simply evalute to a double value:
													// locationManager.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters;


			// handle the updated location method and update the UI
			if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
			{
				locationManager.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
				{
					CLLocation newLocation = e.Locations[e.Locations.Length - 1];
					geoLocation.Latitude = newLocation.Coordinate.Latitude;
					geoLocation.Longitude = newLocation.Coordinate.Longitude;
				};
			}
			else
			{
#pragma warning disable 618
				// this won't be called on iOS 6 (deprecated)
				locationManager.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) =>
				{
					CLLocation newLocation = e.NewLocation;
					geoLocation.Latitude = newLocation.Coordinate.Latitude;
					geoLocation.Longitude = newLocation.Coordinate.Longitude;
				};
#pragma warning restore 618
			}

			//iOS 8 requires you to manually request authorization now - Note the Info.plist file has a new key called requestWhenInUseAuthorization added to.
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				locationManager.RequestWhenInUseAuthorization();
			}

			// start updating our location, et. al.
			if (CLLocationManager.LocationServicesEnabled)
				locationManager.StartUpdatingLocation();
			if (IncludeHeading && CLLocationManager.HeadingAvailable)
				locationManager.StartUpdatingHeading();

			return geoLocation;
		}

		public string GetIPAddress()
		{
			//Public IP
			string publicIP = new System.Net.WebClient().DownloadString("https://icanhazip.com/").Trim();
			return publicIP;
		}

		public bool GetIsCompromised()
		{
			if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("cydia://package/com.exapmle.package")))
			{
				return true;
			}

			return false;
		}

		public bool GetIsEmulator()
		{
			if (Runtime.Arch == Arch.SIMULATOR)
				return true;
			return false;
		}

		public string GetLanguage()
		{
			return NSLocale.CurrentLocale.LocaleIdentifier;
		}

		public string GetModel()
		{
			return UIDevice.CurrentDevice.Model;
		}

		public string GetOSVersion()
		{
			return UIDevice.CurrentDevice.SystemVersion;
		}

		public string GetPhoneNumber()
		{
			return "";
		}

		public string GetSimId()
		{
			return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
		}
	}
}
