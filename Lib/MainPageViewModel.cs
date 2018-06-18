using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Contracts;

namespace Lib
{
	public class MainPageViewModel : INotifyPropertyChanged
    {
	    private readonly ClockService _clock;
	    private readonly ILocalizationService _localization;
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
	    public ObservableCollection<SupportedLanguage> SupportedLanguages { get; } = new ObservableCollection<SupportedLanguage>();

	    public MainPageViewModel(ClockService clock, ILocalizationService localization, IScheduler dispatcher, IScheduler threadPool, IExternalConsoleService console)
		{
			_clock = clock;
			_localization = localization;
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
		    Register(_clock.Tick, Update);

		    Register(_clock.PollingTick
			    .Where(_ => _console.IsReady)
			    .Select(_ => Observable.FromAsync(_console.GetStatusAsync, _threadPool))
			    .Switch()
			    .DistinctUntilChanged(),
			    status => Status = status);

		    var languages = _localization.SupportedLanguages.Select(l =>
			    new SupportedLanguage(l,
				    l == _localization.CurrentLanguage,
				    async () => await _localization.SetLanguageAsync(l)));

		    foreach (var l in languages)
		    {
				SupportedLanguages.Add(l);
		    }
	    }

	    public void OnNavigatedFrom()
	    {
		    SupportedLanguages.Clear();

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

	    private void Register<T>(IObservable<T> sequence, Action<T> onNext) => _subscriptions.Push(sequence.ObserveOn(_dispatcher).Subscribe(onNext));
    }

	public class SupportedLanguage
	{
		/// <summary>
		/// BCP-47 language tag
		/// </summary>
		public string Tag { get; }

		public bool IsCurrent { get; }
		public Action OnSetLanguage { get; }

		public SupportedLanguage(string tag, bool isCurrent, Action onSetLanguage)
		{
			Tag = tag;
			IsCurrent = isCurrent;
			OnSetLanguage = onSetLanguage;
		}

		public void SetAsCurrent() => OnSetLanguage();
	}

	public class DataPoint
	{
		public string Value { get; set; }
	}
}
