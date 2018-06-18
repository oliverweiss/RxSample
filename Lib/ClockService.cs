using System;
using System.Globalization;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Lib
{
	public class ClockService
	{
		public IObservable<TimeStamp> Tick { get; }
		public IObservable<long> PollingTick { get; set; }

		public static readonly int PollingInterval = 10;
		
		public ClockService(IScheduler threadPool)
		{
			var ticks = Observable.Generate(Unit.Default,
				_ => true,
				_ => Unit.Default,
				_ => Unit.Default,
				_ => NextSecond(threadPool.Now),
				threadPool
			).Publish().RefCount();
			
			var now = Observable.Return(Unit.Default, threadPool);

			Tick = now
				.Concat(ticks)
				.Select(_ => new TimeStamp(threadPool.Now));

			PollingTick = now
				.Concat(ticks
					.Buffer(PollingInterval)
					.Select(_ => Unit.Default))
				.Select(_ => 0L);
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

		public string Time => _instant.ToLongTimeString();
		public string Date => _instant.ToShortDateString();
		public string Error => _instant.ToString("fff");
	}
}
