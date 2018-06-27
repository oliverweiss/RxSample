using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
		private static readonly IScheduler _backgroundScheduler = Scheduler.Default;
		private static readonly IScheduler _connectionScheduler = Scheduler.Default;

		static void Main(string[] args)
		{
			_appServiceExit = new AutoResetEvent(false);
			InitializeAppServiceConnection();

			StartAFight();

			_appServiceExit.WaitOne();
			System.Console.ReadLine();
		}

		private static void StartAFight()
		{
			var keyStrokes = Observable
				.Defer(() => Observable
					.Start(() => System.Console.ReadKey(true), _backgroundScheduler))
				.Repeat()
				.Publish().RefCount();

			var words = keyStrokes
				.Zip(new BatmanFight(), (_, word) => word)
				.DistinctUntilChanged(TimeSpan.FromSeconds(1), _backgroundScheduler);
			
			words
				.Do(word => ColorConsole.Write(ConsoleColor.White, ConsoleColor.DarkGreen, $" *{word}*"))
				.Select(word => Observable
					.FromAsync(() => _connection
						.SendMessageAsync(new ValueSet {{"BATMAN", word}}).AsTask(), _connectionScheduler)) 
				.Switch()
				.Subscribe();
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
			var status = CalculateStatus();
			await args.Request.SendResponseAsync(new ValueSet { { param, status.ToString() } });
		}

		private static StatusEnum CalculateStatus()
		{
			var status = new Random().NextDouble() < 0.5 ? StatusEnum.Fail : StatusEnum.Ok;
			var backgroundColor = status == StatusEnum.Ok ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
			ColorConsole.WriteLine(ConsoleColor.Gray, backgroundColor, status.ToString());
			return status;
		}
	}

	public static class ObservableExtensions
	{
		public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source, TimeSpan timeout,
			IScheduler scheduler)
		{
			var published = source.Publish().RefCount();

			return published
				.Window(() => published.DistinctUntilChanged().Skip(1).Merge(published.Throttle(timeout, scheduler)))
				.Select(w => w.Take(1))
				.Switch();
		}
	}
}
