using System.Collections.Generic;
using AntMe.Deutsch;

namespace AntMe.Spieler
{

	[Spieler(
		Volkname = "mKillerAnts",
		Vorname = "Martin",
		Nachname = "Disch"
	)]

	[Kaste(
		Name = "Killer",
		GeschwindigkeitModifikator = -1,
		DrehgeschwindigkeitModifikator = -1,
		LastModifikator = -1,
		ReichweiteModifikator = -1,
		SichtweiteModifikator = 0,
		EnergieModifikator = 2,
		AngriffModifikator = 2
	)]

	public class mKillerAnt : Basisameise
	{
		public override string BestimmeKaste(Dictionary<string, int> anzahl)
		{
			return "Killer";
		}

		#region Fortbewegung

		public override void Wartet()
		{
			GeheGeradeaus(40);
			DreheUmWinkel(Zufall.Zahl(-10, 10));
		}

		#endregion
		#region Kommunikation

		public override void RiechtFreund(Markierung markierung)
		{
			if (Ziel == null)
			{
				GeheZuZiel(markierung);
			}
		}

		#endregion
		#region Kampf

		public override void SiehtFeind(Wanze wanze)
		{
			SprüheMarkierung(0, 150);
			GreifeAn(wanze);
		}

		public override void WirdAngegriffen(Wanze wanze)
		{
			GreifeAn(wanze);
		}

		#endregion
		#region Sonstiges

		public override void Tick()
		{

			// Return home if ant has walked a lot already
			if (Reichweite - ZurückgelegteStrecke - 100 < EntfernungZuBau)
			{
				GeheZuBau();
			}

			// Go home if too much energy has been lost during a fight
			if (AktuelleEnergie < MaximaleEnergie * 2 / 3)
			{
				GeheZuBau();
			}

		}

		#endregion

	}

}