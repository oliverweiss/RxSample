using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lib;

namespace App
{
	public static class Services
	{
		static Services()
		{
			Dispatcher = new SynchronizationContextScheduler(SynchronizationContext.Current);

			ThreadPool = Scheduler.Default;

			Clock = new ClockService(ThreadPool);

			Status = new StatusService();

			ViewModel = new MainPageViewModel(Clock, Status, Dispatcher);
		}

		public static IScheduler Dispatcher { get; }

		public static IScheduler ThreadPool { get; }

		public static ClockService Clock { get; }

		public static StatusService Status { get; }

		public static MainPageViewModel ViewModel { get; }
	}
}
