using System;
using System.Diagnostics;
using System.Threading.Tasks;
using hockeyappint.Models;
using hockeyappint.Services;

namespace hockeyappint.BaseClasses
{
	public class BaseViewModel
	{
		#region Private Properties
		private string _busyMessage = "";

		private bool _isBusy;
		internal bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				_isBusy = value;
				try
				{
					if (IsBusy)
					{
						//UserDialogs.Instance.ShowLoading(BusyMessage, MaskType.Black);
					}
					else
					{
						//UserDialogs.Instance.HideLoading();
						_busyMessage = "";
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine($"Can not display or hide Loading dialog, Error {ex.Message} Stack: {ex.StackTrace}");
				}
				//RaisePropertyChanged();
			}
		}
		internal string BusyMessage
		{
			get
			{
				return _busyMessage;
			}
			set
			{
				_busyMessage = value;
				if (IsBusy)
				{
					//UserDialogs.Instance.ShowLoading(BusyMessage, MaskType.Black);
				}
			}
		}

		//internal INavigationService _navigation { get; set; }
		//internal IPageDialogService _dialogService { get; set; }

		public CustomCrashReportService ReportService;

		#endregion Private Properties

		public BaseViewModel()
		{
			ReportService = new CustomCrashReportService();
		}

		public async Task DisplayMessage(string title, string message)
		{
			if (IsBusy)
				IsBusy = false;
			//await _dialogService?.DisplayAlertAsync(title, message, "OK");
		}

		public async Task<bool> DisplayYesNoMessage(string title, string message)
		{
			if (IsBusy)
				IsBusy = false;

			return true;
			//return await _dialogService?.DisplayAlertAsync(title, message, "YES", "NO");
		}

		public async Task DisplayError(Exception ex)
		{
			if (IsBusy)
				IsBusy = false;

			//Sending stack trace to hockeyapp
			await ReportService.PostReport(ex, false, "App Error", $"App Error: {ex.Message}");

			//Display general error
			//await _dialogService?.DisplayAlertAsync("Error", "Something wrong occured", "OK");

			//Showing error to console
			//await _dialogService?.DisplayAlertAsync("Error", ex.Message, "OK");
			Console.WriteLine(ex.ToString());
		}

		public async Task DisplayApiMessage<T>(ResponseResult<T> response)
		{
			if (IsBusy)
				IsBusy = false;

			if (response.StatusCode == 400)
			{
				//Display business error message
				//await _dialogService?.DisplayAlertAsync("Error", response.Message, "OK");
			}
			else if (response.StatusCode == 500)
			{
				//Sending api reason error to hockeyapp
				await ReportService.PostReport(new Exception(response.Message), false, "Api Error", $"API Error: {response.Message}");

				//Display general error
				//await _dialogService?.DisplayAlertAsync("Error", "Something wrong occured", "OK");

				//Showing error to console
				Console.WriteLine(response.Message);
			}
		}
	}
}
