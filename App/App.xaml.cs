using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
	    public static BackgroundTaskDeferral AppServiceDeferral = null;
	    public static AppServiceConnection Connection = null;
	    public static event EventHandler AppServiceConnected;

	    public Frame RootFrame
	    {
		    get => Window.Current.Content as Frame;
		    set => Window.Current.Content = value;
	    }

	    /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
            // assurez-vous juste que la fenêtre est active
            if (RootFrame == null)
            {
                // Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
                var frame = new Frame();

                frame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: chargez l'état de l'application précédemment suspendue
                }

                // Placez le frame dans la fenêtre active
	            RootFrame = frame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (RootFrame.Content == null)
                {
                    // Quand la pile de navigation n'est pas restaurée, accédez à la première page,
                    // puis configurez la nouvelle page en transmettant les informations requises en tant que
                    // paramètre
                    RootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Vérifiez que la fenêtre actuelle est active
                Window.Current.Activate();
            }

	        Services.Clock.PollingTick
		        .ObserveOn(Services.Dispatcher)
		        .Subscribe((_) => ToggleTheme());

			// TODO Move it somewhere async. Maybe have a ShellView, and do this in OnNavigatedTo/OnNavigatedFrom 
	        LaunchConsole();
        }

	    public void ToggleTheme()
	    {
		    var isDark = RootFrame.RequestedTheme == ElementTheme.Dark;
		    RootFrame.RequestedTheme = isDark ? ElementTheme.Light : ElementTheme.Dark;
	    }


        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            deferral.Complete();
        }

	    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
	    {
		    // connection established from the fulltrust process
		    if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails)
		    {
			    AppServiceDeferral = args.TaskInstance.GetDeferral();
			    args.TaskInstance.Canceled += OnTaskCanceled;

			    if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails details)
			    {
				    Connection = details.AppServiceConnection;
				    AppServiceConnected?.Invoke(this, null);
			    }
		    }
	    }

	    private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
	    {
		    AppServiceDeferral?.Complete();
	    }

	    private async Task LaunchConsole()
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




    }
}
