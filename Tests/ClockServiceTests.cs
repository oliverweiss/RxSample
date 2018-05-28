using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Lib;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ClockServiceTests
    {
        [TestMethod]
        public void TickEvery1s()
        {
	        var count = 0;
	        var threadPool = new TestScheduler();
	        var dispatcher = new TestScheduler();
			var clock = new ClockService(threadPool);
	        clock.Tick.ObserveOn(dispatcher).Subscribe(_ => count+=1);

	        Assert.AreEqual(0, count); // Initial state

			// First value is produced immediately
			threadPool.AdvanceBy(1); // Produce the first value
	        Assert.AreEqual(0, count);
			dispatcher.AdvanceBy(1); // Consume the first value
	        Assert.AreEqual(1, count);

	        // Second value is produced at the next full second
			threadPool.AdvanceTo(ClockService.NextSecond(threadPool.Now).Ticks); // Produce the second value
	        Assert.AreEqual(1, count);
			dispatcher.AdvanceBy(1); // Consume the second value
	        Assert.AreEqual(2, count);

			// All remaining values are produced every second
	        for (var i = 2; i < 3600; i++)
	        {
		        threadPool.AdvanceBy(TimeSpan.TicksPerSecond);
		        Assert.AreEqual(i, count);
				dispatcher.AdvanceBy(1);
		        Assert.AreEqual(i+1, count);
	        }
        }
    }
}
