using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Lib;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App
{
	/// <summary>
	/// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private object _navigationParameter;

		public MainPage()
		{
			this.InitializeComponent();

			ApplicationView.PreferredLaunchViewSize = new Size(640, 480);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;


			ViewModel = new MainPageViewModel(Services.Instance.Clock, Services.Instance.Localization, Services.Instance.Dispatcher, Services.Instance.ThreadPool, Services.Instance.ExternalConsole);
		}

		public MainPageViewModel ViewModel { get; set; }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			_navigationParameter = e.Parameter;
			Services.Instance.Localization.OnLanguageChanged += LocalizationOnOnLanguageChanged;
			ViewModel?.OnNavigatedTo();
		}

		private void LocalizationOnOnLanguageChanged(object sender, string e)
		{
			var cacheSize = Frame.CacheSize;
			Frame.CacheSize = 0;
			Frame.Navigate(this.GetType(), _navigationParameter);
			Frame.CacheSize = cacheSize;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel?.OnNavigatedFrom();
			Services.Instance.Localization.OnLanguageChanged -= LocalizationOnOnLanguageChanged;
			base.OnNavigatedFrom(e);
		}
	}
}
