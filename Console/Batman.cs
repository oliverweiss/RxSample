using System;

namespace Console
{
	internal class Batman
	{
		private string[] Words = new[]
		{
			"AIEEE", "AIIEEE", "ARRGH", "AWK", "AWKKKKKK", "BAM", "BANG", "BIFF", "BLOOP", "BLURP", "BOFF", "BONK", "CLANK",
			"CLASH", "CLUNK", "CRRAACK", "CRASH", "CRRAACK", "CRUNCH", "FLRBBBBB", "GLIPP", "GLURPP", "KAPOW", "KAYO", "KERPLOP",
			"KLONK", "KLUNK", "KRUNCH", "OOOFF", "OOOOFF", "OUCH", "OWWW", "PAM", "PLOP", "POW", "POWIE", "QUNCKKK", "RAKKK",
			"RIP", "SLOSH", "SOCK", "SPLATS", "SPLATT", "SPLOOSH", "SWAAP", "SWISH", "SWOOSH", "THUNK", "THWACK", "THWACKE",
			"THWAPE", "THWAPP", "UGGH", "URKKK", "VRONK", "WHACK", "WHAMM", "WHAMMM", "WHAP", "ZAM", "ZAMM", "ZAMMM", "ZAP",
			"ZGRUPPP", "ZLONK", "ZLOPP", "ZLOTT", "ZOK", "ZOWIE", "ZWAPP", "ZZWAP", "ZZZZWAP", "ZZZZZWAP"
		};

		public Batman()
		{
		}

		public IObservable<string> Fight()
		{
			throw new NotImplementedException();
		}
	}
}