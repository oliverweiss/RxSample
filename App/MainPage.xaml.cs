using System;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Lib;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App
{
	/// <summary>
	/// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			ViewModel = new MainPageViewModel(Services.Clock, Services.Status, Services.Dispatcher);
			App.AppServiceConnected += MainPage_AppServiceConnected;

		}

		public MainPageViewModel ViewModel { get; set; }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			ViewModel?.OnNavigatedTo();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			ViewModel?.OnNavigatedFrom();

			base.OnNavigatedFrom(e);
		}

		private async void Do()
		{
			// launch the fulltrust process and for it to connect to the app service            
			if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
			{
				await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
			}
			else
			{
				var dialog = new MessageDialog("This feature is only available on Windows 10 Desktop SKU");
				await dialog.ShowAsync();
			}

		}

		private async void MainPage_AppServiceConnected(object sender, EventArgs e)
		{
			var message = new ValueSet {{"MESSAGE", "Hello, World!"}, {"COLOR", "Black"}, {"BACKGROUND", "Red"}};
			// send the ValueSet to the fulltrust process
			AppServiceResponse response = await App.Connection.SendMessageAsync(message);

			// check the result
			if (response.Message.TryGetValue("RESPONSE", out var result) && result.ToString() == "Success")
			{
				await new MessageDialog(result as string).ShowAsync();
			}
			// no longer need the AppService connection
			App.AppServiceDeferral.Complete();
		}

	}
}
