using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Lib;

namespace App
{
	public class LocalizationService : ILocalizationService
	{
		public IEnumerable<string> SupportedLanguages => ApplicationLanguages.ManifestLanguages;

		//public IEnumerable<string> SupportedLanguages => ResourceContext
		//	.GetForCurrentView()
		//	.QualifierValues["Language"]
		//	.Split(";")
		//	.OrderBy(l => l);

		public string CurrentLanguage => ResourceContext.GetForCurrentView().Languages[0];

		public event EventHandler<string> OnLanguageChanged;

		public async Task SetLanguageAsync(string tag)
		{
			if (tag == CurrentLanguage) return;

			ApplicationLanguages.PrimaryLanguageOverride = tag;

			ResourceContext.GetForCurrentView().Reset();
			ResourceContext.GetForViewIndependentUse().Reset();

			// Workaround, otherwise the first language change does not update the resources.
			await Task.Delay(TimeSpan.FromMilliseconds(200));

			OnLanguageChanged?.Invoke(this, tag);
		}
	}
}