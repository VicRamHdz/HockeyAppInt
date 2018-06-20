using System.Net;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Telephony;
using hockeyappint.DependencyService;
using hockeyappint.Droid.DependencyService;
using hockeyappint.Models;
using Java.IO;
using Java.Lang;
using Java.Security;
using Java.Util;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInformation))]
namespace hockeyappint.Droid.DependencyService
{
	public class DeviceInformation : Object, IDeviceInformation
	{
		private TelephonyManager _telephonyManager;
		private LocationManager _locationManager;

		public DeviceInformation()
		{
			_telephonyManager = (TelephonyManager)Android.App.Application.Context.GetSystemService(Context.TelephonyService);
			_locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
		}

		public void ExecuteLocationManager(bool IncludeHeading = false)
		{
			//Do nothing as this Dependency is only for iOS
		}

		public string GetApplicationKey()
		{
			string deviceIDValue = "";

			byte[] deviceIDbytes = new byte[16];
			if (!getSecureRandomBytes(deviceIDbytes))
			{
				//Log.e("ApplicationKey", "unexpected error in getStoredApplicationKey, can't generate key");
				return "INVALID";
			}
			deviceIDValue = byteArrayToHexString(deviceIDbytes);

			return deviceIDValue;
		}

		public string GetApplicationName()
		{
			var context = Forms.Context;
			return context.PackageManager?.GetApplicationInfo(context.PackageName, 0)?.LoadLabel(context.PackageManager);
		}

		public string GetApplicationVersionNumber()
		{
			//Version appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			//string version = string.Format("{0}.{1}.{2}.{3}", appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);
			Context context = Forms.Context;
			var version = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
			return version;
		}

		public string GetDeviceId()
		{
			return _telephonyManager.DeviceId;
		}

		public string GetDisplay()
		{
			return "";//Resources.DisplayMetrics.WidthPixels + "|" + Resources.DisplayMetric.HeightPixels;
		}

		public GeoLocationModel GetGeoLocation(bool IncludeHeading = false)
		{
			GeoLocationModel geoLocation = new GeoLocationModel();
			Android.Locations.Location currentLocation;
			Criteria locationCriteria = new Criteria();

			locationCriteria.Accuracy = Accuracy.Coarse;
			locationCriteria.PowerRequirement = Power.Medium;
			locationCriteria.BearingRequired = IncludeHeading;

			string locationProvider = _locationManager.GetBestProvider(locationCriteria, true);

			if (locationProvider != null)
			{
				currentLocation = _locationManager.GetLastKnownLocation(locationProvider);

				if (currentLocation != null)
				{
					geoLocation.Latitude = currentLocation.Latitude;
					geoLocation.Longitude = currentLocation.Longitude;
				}
			}
			return geoLocation;
		}

		public string GetIPAddress()
		{
			string ipAddress = string.Empty;
			IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
			if (addresses != null && addresses[0] != null)
			{
				ipAddress = addresses[0].ToString();
			}
			return ipAddress;
		}

		public bool GetIsCompromised()
		{
			return CheckBuildTagsHaveTestKeys() || CheckSUFileExist() || CheckPresenceOfSuspiciousAPKs();
		}

		public bool GetIsEmulator()
		{
			string fingerPrint = Build.Fingerprint;
			bool isEmulator = false;
			if (fingerPrint != null)
			{
				isEmulator = fingerPrint.Contains("vbox") || fingerPrint.Contains("generic");
			}

			return isEmulator;
		}

		public string GetLanguage()
		{
			return Locale.Default.GetDisplayLanguage(Locale.Default);
		}

		public string GetModel()
		{
			return Build.Model;
		}

		public string GetOSVersion()
		{
			return Build.VERSION.Sdk;
		}

		public string GetPhoneNumber()
		{
			return _telephonyManager.Line1Number;
		}

		public string GetSimId()
		{
			return _telephonyManager.SubscriberId;
		}

		#region private methods

		public static bool getSecureRandomBytes(byte[] secureBytesArray)
		{
			if ((null != secureBytesArray) && (0 != secureBytesArray.Length))
			{
				try
				{
					SecureRandom ranGen = SecureRandom.GetInstance("SHA1PRNG");
					ranGen.NextBytes(secureBytesArray);
					return true;
				}
				catch (NoSuchAlgorithmException) { }
			}
			return false;
		}

		public static string byteArrayToHexString(byte[] byteArray)
		{
			int byteArrayLen = byteArray.Length;
			StringBuffer strBuffer = new StringBuffer(byteArrayLen * 2);
			for (int i = 0; i < byteArrayLen; i++)
			{
				int value = byteArray[i] & 0xFF;
				if (value < 16)
				{
					strBuffer.Append('0');
				}
				strBuffer.Append(Integer.ToHexString(value));
			}
			return strBuffer.ToString().ToUpper();
		}

		private bool CheckBuildTagsHaveTestKeys()
		{
			string buildTags = Build.Tags;
			return buildTags != null && buildTags.Contains("test-keys");
		}

		private bool CheckSUFileExist()
		{
			string[] paths = { "/system/app/Superuser.apk", "/sbin/su", "/system/bin/su", "/system/xbin/su", "/data/local/xbin/su", "/data/local/bin/su", "/system/sd/xbin/su",
				"/system/bin/failsafe/su", "/data/local/su" };
			foreach (string path in paths)
			{
				if (new File(path).Exists()) return true;
			}
			return false;
		}

		private bool CheckPresenceOfSuspiciousAPKs()
		{
			Java.Lang.Process process = null;
			try
			{
				process = Runtime.GetRuntime().Exec(new string[] { "/system/xbin/which", "su" });
				BufferedReader br = new BufferedReader(new InputStreamReader(process.OutputStream));
				if (br.ReadLine() != null) return true;
				return false;
			}
			catch (System.Exception)
			{
				return false;
			}
			finally
			{
				if (process != null) process.Dispose();
			}
		}

		#endregion
	}
}
