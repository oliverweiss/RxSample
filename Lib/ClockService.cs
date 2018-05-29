using System;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Lib
{
	public class ClockService
	{
		public IObservable<TimeStamp> Tick { get; }
		public IObservable<long> PollingTick { get; set; }

		public static readonly int PollingInterval = 10;
		
		public ClockService(IScheduler threadPool)
		{
			// TODO Find a way to keep the ticks in sync (expensive solution: polling every 10ms and using DistinctUntilChanged(DateTime.Now.Seconds))
			var ticks = Observable.Timer(
					NextSecond(threadPool.Now),
					TimeSpan.FromSeconds(1),
					threadPool)
				.Publish().RefCount(); // Turns it into a Hot observable (starts producing values immediately)

			var now = Observable.Return(0L, threadPool);

			Tick = now
				.Concat(ticks)
				.Select(_ => new TimeStamp(threadPool.Now));

			PollingTick = now
				.Concat(ticks.Buffer(PollingInterval).Select(_ => 0L));
		}

		/// <summary>
		/// Returns the next full second
		/// </summary>
		/// <param name="now"></param>
		/// <returns></returns>
		public static DateTimeOffset NextSecond(DateTimeOffset now)
		{
			return now.AddTicks(TimeSpan.TicksPerSecond - now.Ticks % TimeSpan.TicksPerSecond);
		}
	}

	public class TimeStamp
	{
		private readonly DateTime _instant;

		public TimeStamp(DateTimeOffset instant)
		{
			_instant = instant.LocalDateTime;
		}

		public string Time => _instant.ToString("HH:mm:ss");
		public string Date => _instant.ToString("dd.MM.yyyy");
		public string Error => _instant.ToString("fff");

		public static TimeStamp Now(IScheduler scheduler) => new TimeStamp(scheduler.Now);
	}
}
