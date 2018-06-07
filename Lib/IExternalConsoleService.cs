using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Lib
{
	public interface IExternalConsoleService
	{
		bool IsReady { get; }
		Task<StatusEnum> GetStatusAsync();
	}
}
