using System;
using System.Threading.Tasks;

namespace Lib
{
	public class StatusService
	{
		private readonly Random _random = new Random();

		public async Task<StatusEnum> GetStatusAsync()
		{
			await Task.Delay(TimeSpan.FromSeconds(_random.NextDouble()));
			return GetStatus();
		}

		private StatusEnum GetStatus()
		{
			var val = _random.Next(0, 100);
			if (val < 30) return StatusEnum.Unknow;
			if (val < 50) return StatusEnum.Fail;
			return StatusEnum.Ok;
		}
	}

	public enum StatusEnum	
	{
		Unknow, Fail, Ok
	}
}