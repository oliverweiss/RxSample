using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace App
{
	public class EqualityTrigger : StateTriggerBase
	{
		public object Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
		public object Target { get => GetValue(TargetProperty); set => SetValue(TargetProperty, value); }

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(EqualityTrigger), new PropertyMetadata(default(object), OnChange));

		// Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TargetProperty =
			DependencyProperty.Register("Target", typeof(object), typeof(EqualityTrigger), new PropertyMetadata(default(object)));


		private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is EqualityTrigger trigger)
			{
				var areEqual = AreEqual(trigger.Value, trigger.Target);
				trigger.SetActive(areEqual);
			}
		}

		private static bool AreEqual(object left, object right)
		{
			if (left == right) return true;
			if (left == null || right == null) { return false; }

			bool AreEqualEnum(object x, object y) => x.GetType().IsEnum && y is int i && (int) x == i;

			return AreEqualEnum(left, right) || AreEqualEnum(right, left);
		}
	}
}
