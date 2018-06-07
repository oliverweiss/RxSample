using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Contracts;
using Lib;

namespace App
{
	public static class Converters
	{
		public static Visibility VisibleWhenOk(StatusEnum arg) => VisibleWhen(StatusEnum.Ok, arg);
		public static Visibility VisibleWhenFail(StatusEnum arg) => VisibleWhen(StatusEnum.Fail, arg);
		public static Visibility VisibleWhenUnknown(StatusEnum arg) => VisibleWhen(StatusEnum.Unknown, arg);

		private static Visibility VisibleWhen(StatusEnum value, StatusEnum arg) => arg == value ? Visibility.Visible : Visibility.Collapsed;
	}
}
