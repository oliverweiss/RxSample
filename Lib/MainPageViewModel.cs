using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Contracts;

namespace Lib
{
	public class MainPageViewModel : INotifyPropertyChanged
    {
	    private readonly ClockService _clock;
	    private readonly IScheduler _dispatcher;
	    private readonly IScheduler _threadPool;
	    private readonly IExternalConsoleService _console;
	    
	    private readonly Stack<IDisposable> _subscriptions = new Stack<IDisposable>();

	    private string _date;
	    private string _time;
	    private string _error;
	    private StatusEnum _status;
	    public string Date  {get => _date; set => Set(ref _date, value); }
	    public string Time  {get => _time; set => Set(ref _time, value); }
	    public string Error {get => _error; set => Set(ref _error, value); }
	    public StatusEnum Status {get => _status; set => Set(ref _status, value); }
		
		public MainPageViewModel(ClockService clock, IScheduler dispatcher, IScheduler threadPool, IExternalConsoleService console)
		{
			_clock = clock;
			_dispatcher = dispatcher;
			_threadPool = threadPool;
			_console = console;
		}

		public void Update(TimeStamp timestamp)
	    {
		    Date = timestamp.Date;
		    Time = timestamp.Time;
		    Error = timestamp.Error;
	    }

	    public void OnNavigatedTo()
	    {
		    _subscriptions.Push(_clock.Tick
			    .ObserveOn(_dispatcher)
			    .Subscribe(Update));

		    _subscriptions.Push(_clock.PollingTick
			    .Where(_ => _console.IsReady)
			    .Select(_ => Observable.FromAsync(_console.GetStatusAsync, _threadPool))
			    .Switch()
			    .DistinctUntilChanged()
			    .ObserveOn(_dispatcher)
			    .Subscribe(status => Status = status));
	    }

	    public void OnNavigatedFrom()
	    {
		    foreach (var subscription in _subscriptions)
		    {
			    subscription?.Dispose();
		    }
	    }
		
	    public event PropertyChangedEventHandler PropertyChanged;

	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }

	    protected void Set<T>(ref T field, T value = default(T), [CallerMemberName] string propertyName = null)
	    {
		    if (EqualityComparer<T>.Default.Equals(field, value)) return;
		    field = value;
		    OnPropertyChanged(propertyName);
	    }

    }
}
