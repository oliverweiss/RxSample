using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Contracts;
using System.Threading.Tasks;

namespace Console
{
	class Program
	{
		static AppServiceConnection _connection = null;
		static AutoResetEvent _appServiceExit;

		static void Main(string[] args)
		{
			_appServiceExit = new AutoResetEvent(false);
			InitializeAppServiceConnection();
			_appServiceExit.WaitOne();

			System.Console.ReadLine();
		}

		static async void InitializeAppServiceConnection()
		{
			_connection = new AppServiceConnection
			{
				AppServiceName = "ExternalConsole",
				PackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName
			};
			_connection.RequestReceived += Connection_RequestReceived;
			_connection.ServiceClosed += Connection_ServiceClosed;

			AppServiceConnectionStatus status = await _connection.OpenAsync();
			if (status != AppServiceConnectionStatus.Success)
			{
				// TODO: error handling
			}
		}

		private static void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
		{
			// signal the event so the process can shut down
			_appServiceExit.Set();
		}

        private static async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var param = args.Request.Message["GET"] as string;
	        var status = await CalculateStatus(); 
	        await args.Request.SendResponseAsync(new ValueSet {{param, status.ToString()}});
        }

		private static  Task<StatusEnum> CalculateStatus()
		{
			var status = new Random().NextDouble() < 0.5 ? StatusEnum.Fail : StatusEnum.Ok;
			if (status == StatusEnum.Ok) ColorConsole.WriteLine(ConsoleColor.Gray, ConsoleColor.DarkGreen, status.ToString());
			else ColorConsole.WriteLine(ConsoleColor.Gray, ConsoleColor.DarkRed, status.ToString());
			return Task.FromResult(status);
		}


	}
}
