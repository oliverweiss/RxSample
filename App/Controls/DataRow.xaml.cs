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

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace App.Controls
{
    public sealed partial class DataRow : UserControl
    {
        public DataRow()
        {
            this.InitializeComponent();
        }
		
		public string Key
		{
			get { return (string)GetValue(KeyProperty); }
			set { SetValue(KeyProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty KeyProperty =
			DependencyProperty.Register(nameof(Key), typeof(string), typeof(DataRow), new PropertyMetadata(""));
		
		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register(nameof(Value), typeof(string), typeof(DataRow), new PropertyMetadata(""));
	}
}
