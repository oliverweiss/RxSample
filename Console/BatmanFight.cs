using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Console
{
	internal class BatmanFight : IEnumerable<string>
	{
		private static readonly string[] Words = new[]
		{
			"AIEEE", "AIIEEE", "ARRGH", "AWK", "AWKKKKKK", "BAM", "BANG", "BIFF", "BLOOP", "BLURP", "BOFF", "BONK", "CLANK",
			"CLASH", "CLUNK", "CRRAACK", "CRASH", "CRRAACK", "CRUNCH", "FLRBBBBB", "GLIPP", "GLURPP", "KAPOW", "KAYO", "KERPLOP",
			"KLONK", "KLUNK", "KRUNCH", "OOOFF", "OOOOFF", "OUCH", "OWWW", "PAM", "PLOP", "POW", "POWIE", "QUNCKKK", "RAKKK",
			"RIP", "SLOSH", "SOCK", "SPLATS", "SPLATT", "SPLOOSH", "SWAAP", "SWISH", "SWOOSH", "THUNK", "THWACK", "THWACKE",
			"THWAPE", "THWAPP", "UGGH", "URKKK", "VRONK", "WHACK", "WHAMM", "WHAMMM", "WHAP", "ZAM", "ZAMM", "ZAMMM", "ZAP",
			"ZGRUPPP", "ZLONK", "ZLOPP", "ZLOTT", "ZOK", "ZOWIE", "ZWAPP", "ZZWAP", "ZZZZWAP", "ZZZZZWAP"
		};

		public IEnumerator<string> GetEnumerator()
		{
			return new Enumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal class Enumerator : IEnumerator<string>
		{
			private readonly double _mutability;
			private readonly Random _rand;
			private int _current;

			public Enumerator(double mutability = 0.1)
			{
				_mutability = mutability;
				_rand = new Random();
				ChangeValue();
			}

			private void ChangeValue()
			{
				_current = _rand.Next(Words.Length);
				ColorConsole.Write(ConsoleColor.White, ConsoleColor.DarkMagenta, Environment.NewLine+Current);
			}

			public bool MoveNext()
			{
				if (_rand.NextDouble() < _mutability)
				{
					ChangeValue();
				}
				else
				{
					ColorConsole.Write(ConsoleColor.White, ConsoleColor.DarkYellow, " "+Current);
				}

				return true;
			}

			public void Reset() => ChangeValue();

			public string Current => Words[_current];

			object IEnumerator.Current => Current;

			public void Dispose() { }
		}
	}
}