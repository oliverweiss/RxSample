using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Shared
{
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
