﻿using System;
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
			ViewModel = new MainPageViewModel(Services.Instance.Clock, Services.Instance.Dispatcher, Services.Instance.ThreadPool, Services.Instance.ExternalConsole);
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
	}
}
