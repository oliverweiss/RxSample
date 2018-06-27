using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI;
using Contracts;
using Lib;
using System.Reactive.Linq;
using Windows.Foundation;

namespace App
{
	public class ExternalConsoleService : IExternalConsoleService
	{
		private readonly ClockService _clock;

		private class ExternalConsoleServiceImpl : IExternalConsoleService
		{
			private readonly AppServiceConnection _connection;
			private IObservable<ValueSet> _consoleEvents
				;

			public ExternalConsoleServiceImpl(AppServiceConnection connection)
			{
				_connection = connection;

				_consoleEvents = Observable.FromEventPattern<TypedEventHandler<AppServiceConnection, AppServiceRequestReceivedEventArgs>, AppServiceRequestReceivedEventArgs>(
					h => h.Invoke,
					h => _connection.RequestReceived += h,
					h => _connection.RequestReceived -= h)
					.Select(args => args.EventArgs.Request.Message)
					.Publish().RefCount();

				FightingWords = _consoleEvents
					.Where(vs => vs.ContainsKey("BATMAN"))
					.Select(vs => vs["BATMAN"].ToString())
					.Select(Capitalize);
			}

			private static string Capitalize(string arg)
			{
				if (string.IsNullOrEmpty(arg)) return arg;
				if (arg.Length < 1) return arg;
				return arg.Substring(0, 1).ToUpperInvariant() + arg.Substring(1).ToLowerInvariant() + " !";
			}

			public bool IsReady => true;

			public async Task<StatusEnum> GetStatusAsync()
			{
				var request = new ValueSet { { "GET", "status" } };

				var response = await _connection.SendMessageAsync(request);

				return response.Message.TryGetValue("status", out var status) &&
					   Enum.TryParse(status as string, out StatusEnum result)
					? result
					: StatusEnum.Unknown;
			}

			public IObservable<string> FightingWords { get; }
		};

		private ExternalConsoleServiceImpl _implementation;
		private ExternalConsoleServiceImpl Implementation => _implementation ?? throw new ArgumentNullException($"{nameof(ExternalConsoleService)} is not initialized yet. Call {nameof(ExternalConsoleService)}.{nameof(Init)} first.");

		public ExternalConsoleService(ClockService clock)
		{
			_clock = clock;
		}

		public void Init(AppServiceConnection connection) => _implementation = new ExternalConsoleServiceImpl(connection);

		public bool IsReady => _implementation != null;
		public Task<StatusEnum> GetStatusAsync() => Implementation.GetStatusAsync();
		public IObservable<string> FightingWords => _clock.DeferUntil(() => IsReady, () => Implementation.FightingWords);
	}
}
