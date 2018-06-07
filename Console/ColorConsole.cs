using System;

namespace Console
{
	internal class ColorConsole : IDisposable
	{
		private readonly ConsoleColor _initialForegroundColor;
		private readonly ConsoleColor? _initialBackgroundColor;

		public ColorConsole(ConsoleColor foregroundColor, ConsoleColor? backgroundColor = null)
		{
			_initialForegroundColor = System.Console.ForegroundColor;
			_initialBackgroundColor = System.Console.BackgroundColor;
			System.Console.ForegroundColor = foregroundColor;
			if (backgroundColor != null) System.Console.BackgroundColor = backgroundColor.Value;
		}

		public static void WriteLine(ConsoleColor foregroundColor, string text)
		{
			using (new ColorConsole(foregroundColor)) { System.Console.WriteLine(text); }
		}

		public static void WriteLine(ConsoleColor foregroundColor,ConsoleColor backgroundColor, string text)
		{
			using (new ColorConsole(foregroundColor, backgroundColor)) { System.Console.WriteLine(text); }
		}
		
		public void Dispose()
		{
			System.Console.ForegroundColor = _initialForegroundColor;
			if (_initialBackgroundColor != null) System.Console.BackgroundColor = _initialBackgroundColor.Value;
		}
	}
}