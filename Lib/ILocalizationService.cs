using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib
{
	public interface ILocalizationService
	{
		IEnumerable<string> SupportedLanguages { get; }
		string CurrentLanguage { get; }
		Task SetLanguageAsync(string tag);
		event EventHandler<string> OnLanguageChanged;
	}
}