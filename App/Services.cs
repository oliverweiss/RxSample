using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Lib;

namespace App
{
	public class Services
	{
		public static Services Instance { get; } = new Services();

		private Services()
		{
			Dispatcher = new SynchronizationContextScheduler(SynchronizationContext.Current);
			ThreadPool = Scheduler.Default;
			Clock = new ClockService(ThreadPool);
			ExternalConsole = new ExternalConsoleService();
			ViewModel = new MainPageViewModel(Clock, Dispatcher, ThreadPool, ExternalConsole);
		}

		public IExternalConsoleService ExternalConsole { get; }
		public IScheduler Dispatcher { get; }
		public IScheduler ThreadPool { get; }
		public ClockService Clock { get; }
		public MainPageViewModel ViewModel { get; }
	}
}
