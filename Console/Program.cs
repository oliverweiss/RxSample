﻿using System;
using System.Threading;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

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
				AppServiceName = "NetConsole",
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
            var message = args.Request.Message["MESSAGE"] as string;
	        var foreground = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), args.Request.Message["COLOR"] as string ?? ConsoleColor.White.ToString());
	        var background = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), args.Request.Message["BACKGROUND"] as string ?? ConsoleColor.Black.ToString());

			ColorConsole.WriteLine(foreground, background, message);

	        await args.Request.SendResponseAsync(new ValueSet {{"RESPONSE", "Success"}});
        }


	}
}
