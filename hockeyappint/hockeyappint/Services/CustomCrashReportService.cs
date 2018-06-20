using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using hockeyappint.DependencyService;
using Xamarin.Forms;

namespace hockeyappint.Services
{
	public class CustomCrashReportService
	{
		private readonly HttpClient _client;
		private string _endpointUrl;

		public CustomCrashReportService()
		{
			_client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(200),

			};

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					_endpointUrl = Constants.HockeyIOSAppUrl;
					break;
				case Device.Android:
					_endpointUrl = Constants.HockeyAndroidAppUrl;
					break;
			}
		}

		/// <summary>
		/// Send report 
		/// Workarround problems: http://developers.de/blogs/damir_dobric/archive/2013/09/10/problems-with-webapi-multipart-content-upload-and-boundary-quot-quotes.aspx
		/// </summary>
		public async Task<string> PostReport(Exception ex, bool isApiError = false, string title = "", string message = "")
		{
			var device = Xamarin.Forms.DependencyService.Get<IDeviceInformation>();
			var version = device.GetApplicationVersionNumber();

			// Send only production crashes
			//if (version.Contains("x")) return null;

			var report = CreateReportFile(ex, isApiError, device, title, message);
			var buffer = Encoding.UTF8.GetBytes(report);

			HttpContent fileBytesArray = new ByteArrayContent(buffer);

			// Hockeyapp will throw a 500 error if you DON'T have quotes around the filename!
			fileBytesArray.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "\"log\"",
				FileName = "\"logfile.log\"",
			};
			fileBytesArray.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

			var form = new MultipartFormDataContent();

			// You have to take the quotes out of the boundary header or Hockeyapp claims you haven't posted a file.
			// and you will get 400 errors
			var param = form.Headers.ContentType.Parameters;
			var headerValue = param.FirstOrDefault(p => p.Name == "boundary");
			headerValue.Value = headerValue.Value.Replace("\"", "");

			// Add the file report to the multipart form 
			form.Add(fileBytesArray);

			var result = await _client.PostAsync(UriBuilder("crashes/upload"), form);
			return result.StatusCode.ToString();
		}

		private string CreateReportFile(Exception ex, bool isApiError, IDeviceInformation device, string title, string message)
		{
			// Create the custom report
			var guid = Guid.NewGuid();
			const string space = "\r\n";
			var baseEx = ex.GetBaseException();
			// Feel free to add any (key: value)
			// Metadata that you need 
			// also remove colons on value to avoid break the report
			var report =
				"Format: Xamarin" + space +
				"Package: " + string.Format("com.{0}.casino", device.GetApplicationName().ToLower()) + space +
				"Version: " + device.GetApplicationVersionNumber() + space +
				"IsApiError: " + isApiError + space +
				//"User: " + App.UserProfileInfo?.FirstName + space + App.UserProfileInfo?.LastName +
				"Title: " + title.Replace(":", "=") + space +
				"Message: " + "Feedback Handled Exception - " + message.Replace(":", "=") + space +
				"CrashReporter Key: " + guid +
				space + space +
				baseEx.Message.Replace(":", "=") + space +
				baseEx.StackTrace;

			return report;
		}

		private Uri UriBuilder(string endpoint) => new Uri(string.Concat(_endpointUrl, endpoint));
	}
}
