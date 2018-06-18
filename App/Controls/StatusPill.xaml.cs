using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Contracts;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace App.Controls
{
    public sealed partial class StatusPill : UserControl
    {
        public StatusPill()
        {
            this.InitializeComponent();
        }

		public StatusEnum Status
		{
			get => (StatusEnum)GetValue(StatusProperty);
			set => SetValue(StatusProperty, value);
		}

		// Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register(nameof(Status), typeof(StatusEnum), typeof(StatusPill), new PropertyMetadata(0, OnStatusChanged));

	    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	    {
		    if (d is StatusPill control)
		    {
			    var success = VisualStateManager.GoToState(control, control.Status.ToString(), false);
		    }
	    }
    }
}
