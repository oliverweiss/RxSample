//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************

using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;

namespace App.Controls
{
	/// <summary>
	/// The <see cref="CompositionShadow"/> control allows the creation of a DropShadow for any Xaml FrameworkElement in markup
	/// making it easier to add shadows to Xaml without having to directly drop down to Windows.UI.Composition APIs.
	/// </summary>
	[ContentProperty(Name = nameof(CastingElement))]
	public sealed partial class CompositionShadow : UserControl
	{
		public static readonly DependencyProperty BlurRadiusProperty =
			DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(CompositionShadow), new PropertyMetadata(9.0, OnBlurRadiusChanged));

		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register(nameof(Color), typeof(Color), typeof(CompositionShadow), new PropertyMetadata(Colors.Black, OnColorChanged));

		public static readonly DependencyProperty OffsetXProperty =
			DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetXChanged));

		public static readonly DependencyProperty OffsetYProperty =
			DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetYChanged));

		public static readonly DependencyProperty OffsetZProperty =
			DependencyProperty.Register(nameof(OffsetZ), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetZChanged));

		public static readonly DependencyProperty ShadowOpacityProperty =
			DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(CompositionShadow), new PropertyMetadata(1.0, OnShadowOpacityChanged));

		public CompositionShadow()
		{
			InitializeComponent();
			DefaultStyleKey = typeof(CompositionShadow);
			SizeChanged += CompositionShadow_SizeChanged;
			Loaded += (sender, e) => ConfigureShadowVisualForCastingElement();

			var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
			Visual = compositor.CreateSpriteVisual();
			DropShadow = compositor.CreateDropShadow();
			Visual.Shadow = DropShadow;

			// SetElementChildVisual on the control itself ("this") would result in the shadow
			// rendering on top of the content. To avoid this, CompositionShadow contains a Border
			// (to host the shadow) and a ContentPresenter (to hose the actual content, "CastingElement").
			ElementCompositionPreview.SetElementChildVisual(ShadowElement, Visual);
		}

		/// <summary>
		/// The blur radius of the drop shadow.
		/// </summary>
		public double BlurRadius
		{
			get => (double)GetValue(BlurRadiusProperty);
			set => SetValue(BlurRadiusProperty, value);
		}

		/// <summary>
		/// The FrameworkElement that this <see cref="CompositionShadow"/> uses to create the mask for the
		/// underlying <see cref="Windows.UI.Composition.DropShadow"/>.
		/// </summary>
		public FrameworkElement CastingElement
		{
			get => _castingElement;

			set
			{
				if (_castingElement != null)
				{
					_castingElement.SizeChanged -= CompositionShadow_SizeChanged;
				}

				_castingElement = value;
				_castingElement.SizeChanged += CompositionShadow_SizeChanged;

				ConfigureShadowVisualForCastingElement();
			}
		}

		/// <summary>
		/// The color of the drop shadow.
		/// </summary>
		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		/// <summary>
		/// Exposes the underlying composition object to allow custom Windows.UI.Composition animations.
		/// </summary>
		public DropShadow DropShadow { get; }

		/// <summary>
		/// Exposes the underlying SpriteVisual to allow custom animations and transforms.
		/// </summary>
		public SpriteVisual Visual { get; }

		/// <summary>
		/// The mask of the underlying <see cref="Windows.UI.Composition.DropShadow"/>.
		/// Allows for a custom <see cref="Windows.UI.Composition.CompositionBrush"/> to be set.
		/// </summary>
		public CompositionBrush Mask
		{
			get => DropShadow.Mask;
			set => DropShadow.Mask = value;
		}

		/// <summary>
		/// The x offset of the drop shadow.
		/// </summary>
		public double OffsetX
		{
			get => (double)GetValue(OffsetXProperty);
			set => SetValue(OffsetXProperty, value);
		}

		/// <summary>
		/// The y offset of the drop shadow.
		/// </summary>
		public double OffsetY
		{
			get => (double)GetValue(OffsetYProperty);
			set => SetValue(OffsetYProperty, value);
		}

		/// <summary>
		/// The z offset of the drop shadow.
		/// </summary>
		public double OffsetZ
		{
			get => (double)GetValue(OffsetZProperty);
			set => SetValue(OffsetZProperty, value);
		}

		/// <summary>
		/// The opacity of the drop shadow.
		/// </summary>
		public double ShadowOpacity
		{
			get => (double)GetValue(ShadowOpacityProperty);
			set => SetValue(ShadowOpacityProperty, value);
		}

		private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnBlurRadiusChanged((double)e.NewValue);
		private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnColorChanged((Color)e.NewValue);
		private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnOffsetXChanged((double)e.NewValue);
		private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnOffsetYChanged((double)e.NewValue);
		private static void OnOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnOffsetZChanged((double)e.NewValue);
		private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((CompositionShadow)d).OnShadowOpacityChanged((double)e.NewValue);
		private void CompositionShadow_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateShadowSize();

		private void ConfigureShadowVisualForCastingElement()
		{
			UpdateShadowMask();
			UpdateShadowSize();
		}

		private void OnBlurRadiusChanged(double newValue) => DropShadow.BlurRadius = (float)newValue;
		private void OnColorChanged(Color newValue) => DropShadow.Color = newValue;
		private void OnOffsetXChanged(double newValue) => UpdateShadowOffset((float)newValue, DropShadow.Offset.Y, DropShadow.Offset.Z);
		private void OnOffsetYChanged(double newValue) => UpdateShadowOffset(DropShadow.Offset.X, (float)newValue, DropShadow.Offset.Z);
		private void OnOffsetZChanged(double newValue) => UpdateShadowOffset(DropShadow.Offset.X, DropShadow.Offset.Y, (float)newValue);
		private void OnShadowOpacityChanged(double newValue) => DropShadow.Opacity = (float)newValue;

		private void UpdateShadowMask()
		{
			CompositionBrush GetAlphaMask(FrameworkElement e)
			{
				if (e == null) return null;
				switch (e)
				{
					case Image image: return image.GetAlphaMask();
					case Shape shape: return shape.GetAlphaMask();
					case TextBlock block: return block.GetAlphaMask();
					default: return null;
				}
			}

			DropShadow.Mask = GetAlphaMask(_castingElement);
		}

		private void UpdateShadowOffset(float x, float y, float z) => DropShadow.Offset = new Vector3(x, y, z);

		private void UpdateShadowSize()
		{
			var newSize = new Vector2((float)ActualWidth, (float)ActualHeight);
			if (_castingElement != null)
			{
				newSize = new Vector2((float)_castingElement.ActualWidth, (float)_castingElement.ActualHeight);
			}

			Visual.Size = newSize;
		}

		private FrameworkElement _castingElement;
	}
}