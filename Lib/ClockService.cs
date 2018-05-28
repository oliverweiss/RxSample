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
				.Select(_ => TimeStamp.Now);

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
			return now
				.AddSeconds(1)
				.AddTicks(now.Ticks % TimeSpan.TicksPerSecond);
		}
	}

	public class TimeStamp
	{
		private readonly DateTime _instant;

		public TimeStamp(DateTime instant)
		{
			_instant = instant;
		}

		public string Time => _instant.ToString("HH:mm:ss");
		public string Date => _instant.ToString("dd.MM.yyyy");
		public string Error => _instant.ToString("fff");

		public static TimeStamp Now => new TimeStamp(DateTime.Now);
	}
}
