using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Lib;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ClockServiceTests
    {
	    private TestScheduler threadPool;
	    private TestScheduler dispatcher;
	    private IDisposable subscription;

		[TestInitialize]
	    public void Init()
	    {
		    threadPool = new TestScheduler();
		    dispatcher = new TestScheduler();
	    }

	    [TestMethod]
	    public void NextSecondReturnsTheNextFullSecond()
	    {
		    var exactSecond = new DateTimeOffset(2018, 05, 29, 7, 39, 15, new TimeSpan(0,2,0,0));

		    void Verify(long ticks)
		    {
			    var next = ClockService.NextSecond(exactSecond.AddTicks(ticks));
			    Assert.AreEqual(0, next.Millisecond);
			    Assert.AreEqual(0, next.Ticks % TimeSpan.TicksPerSecond);
			    Assert.AreEqual(exactSecond.AddSeconds(1), next);
		    }

		    Verify(1L);
		    Verify(TimeSpan.TicksPerSecond - 1);
		    Verify(3 * TimeSpan.TicksPerSecond / 5);
	    }

        [TestMethod]
        public void TickEverySecond()
		{
			var count = 0;
			var clock = new ClockService(threadPool);
			subscription = clock.Tick.ObserveOn(dispatcher).Subscribe(_ => count += 1);

			// Initial state
			Assert.AreEqual(0, count);

			// First value is produced immediately
			AdvanceByAndVerify(1, 1, ref count, true); 

			// Second value is produced at the next full second
			AdvanceToAndVerify(ClockService.NextSecond(threadPool.Now).Ticks, 2, ref count, true);
			Assert.AreEqual(0, threadPool.Now.Millisecond);

			// All remaining values are produced every second
			for (var i = 3; i < 3600; i++)
			{
				AdvanceByAndVerify(TimeSpan.TicksPerSecond, i, ref count, true);
			}
		}

	    [TestMethod]
	    public void LongTickEveryNSecond()
	    {
		    var count = 0;
		    var clock = new ClockService(threadPool);

		    subscription = clock.PollingTick.ObserveOn(dispatcher).Subscribe(_ => count+=1);

		    Assert.AreEqual(0, count); // Initial state

		    // First value is produced immediately
			AdvanceByAndVerify(1, 1, ref count, true);

		    // A new value is produced in every time interval
		    for (var i = 1; i < 1000; i++)
		    {
			    AdvanceByAndVerify(TimeSpan.TicksPerSecond*ClockService.PollingInterval, i+1, ref count);
		    }
	    }

	    [TestCleanup]
	    public void Cleanup()
	    {
			subscription?.Dispose();
	    }

	    private void AdvanceByAndVerify(long threadPoolTicks, int expectedValueAfter, ref int count, bool exact = false)
	    {

		    if (exact)
		    {
			    threadPool.AdvanceBy(threadPoolTicks-1);
			    Assert.AreEqual(expectedValueAfter-1, count);
			    dispatcher.AdvanceBy(1);
			    Assert.AreEqual(expectedValueAfter-1, count);
		    }

		    threadPool.AdvanceBy(exact ? 1 : threadPoolTicks);
		    Assert.AreEqual(expectedValueAfter-1, count);
		    dispatcher.AdvanceBy(1);
		    Assert.AreEqual(expectedValueAfter, count);

	    }

	    void AdvanceToAndVerify(long threadPoolTicks, int expectedValueAfter, ref int count, bool exact = false)
	    {
		    if (exact)
		    {
			    threadPool.AdvanceTo(threadPoolTicks-1);
			    Assert.AreEqual(expectedValueAfter - 1, count);
			    dispatcher.AdvanceBy(1);
			    Assert.AreEqual(expectedValueAfter - 1, count);

			    threadPool.AdvanceBy(1);
		    }
		    else
		    {
			    threadPool.AdvanceTo(threadPoolTicks);
		    }
		    Assert.AreEqual(expectedValueAfter-1, count);
		    dispatcher.AdvanceBy(1);
		    Assert.AreEqual(expectedValueAfter, count);
	    }
    }
}
