using System;
using System.Reactive.Linq;

namespace Lib
{
	public static class ObservableExtensions
	{
		public static IObservable<T> StartImmediately<T>(this IObservable<T> source) => source.Publish().RefCount();
	}
}