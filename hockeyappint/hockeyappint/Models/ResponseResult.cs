using System;
namespace hockeyappint.Models
{
	public class ResponseResult<T>
	{
		/// <summary>
		/// Status code for the HTTP call.
		/// </summary>
		public int StatusCode { get; set; }
		/// <summary>
		/// Status of the request.
		/// </summary>
		public string Status { get; set; }
		/// <summary>
		/// Additional details for the status of the request.
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// Data that comes as a result of the request.
		/// </summary>
		public T Data { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is success.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is succes; otherwise, <c>false</c>.
		/// </value>
		public bool IsSuccess
		{
			get
			{
				if (string.IsNullOrEmpty(Status))
				{
					return false;
				}
				return Status.ToLower().Equals("success");
			}
		}
	}
}
