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

namespace App
{
	public class ExternalConsoleService : IExternalConsoleService
	{
		private class ExternalConsoleServiceImpl : IExternalConsoleService
		{
			private readonly AppServiceConnection _connection;

			public ExternalConsoleServiceImpl(AppServiceConnection connection)
			{
				_connection = connection;
			}

			public bool IsReady => true;

			public async Task<StatusEnum> GetStatusAsync()
			{
				var request = new ValueSet {{"GET", "status"}};

				var response = await _connection.SendMessageAsync(request);

				if (response.Message.TryGetValue("status", out var status))
					if (Enum.TryParse(status as string, out StatusEnum result))
						return result;
				return StatusEnum.Unknown;
			}

		}

		private ExternalConsoleServiceImpl _implementation;
		private ExternalConsoleServiceImpl Implementation => _implementation ?? throw new ArgumentNullException($"{nameof(ExternalConsoleService)} is not initialized yet. Call {nameof(ExternalConsoleService)}.{nameof(Init)} first.");

		public void Init(AppServiceConnection connection) => _implementation = new ExternalConsoleServiceImpl(connection);

		public bool IsReady => _implementation != null;
		public Task<StatusEnum> GetStatusAsync() => Implementation.GetStatusAsync();



	}
}
