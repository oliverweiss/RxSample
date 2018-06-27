using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
	public sealed partial class WordArt : UserControl
	{
		private Random _rand;

		public WordArt()
		{
			this.InitializeComponent();
			_rand = new Random();
		}

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(WordArt), new PropertyMetadata("", TextChanged));




		public double Angle
		{
			get { return (double)GetValue(AngleProperty); }
			set { SetValue(AngleProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AngleProperty =
			DependencyProperty.Register("Angle", typeof(double), typeof(WordArt), new PropertyMetadata(0.0));

		private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((WordArt)d).OnTextChanged();
		}

		private void OnTextChanged()
		{
			Angle = _rand.NextDouble(-30.0, 30.0);

			jumpIn.Begin();
		}
	}

	public static class RandomExtensions
	{
		public static double NextDouble(this Random rnd, double to)
		{
			return rnd.NextDouble() * to;
		}

		public static double NextDouble(this Random rnd, double from, double to)
		{
			return from + rnd.NextDouble(to - from);
		}

	}
}
