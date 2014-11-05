using System;
using System.Collections.Generic;

using AntMe.Deutsch;

namespace AntMe.Spieler
{
	[Spieler(
		Volkname = "mSugarAnts",
		Vorname = "Martin",
		Nachname = "Disch"
	)]

	[Kaste(
		Name = "Standard",
		GeschwindigkeitModifikator = 1,
		DrehgeschwindigkeitModifikator = -1,
		LastModifikator = 2,
		ReichweiteModifikator = -1,
		SichtweiteModifikator = 1,
		EnergieModifikator = -1,
		AngriffModifikator = -1
	)]

	public class mSugarAnt : Basisameise
	{

		#region Kaste

		public override string BestimmeKaste(Dictionary<string, int> anzahl)
		{
			return "Standard";
		}

		#endregion

		#region Fortbewegung

		public override void Wartet()
		{
            if (Ziel == null)
            {
                DreheUmWinkel(Zufall.Zahl(-10, 10));
                GeheGeradeaus(20);
            }
		}

		public override void WirdMüde()
		{
            GeheZuBau();
		}

		#endregion

		#region Nahrung

		public override void Sieht(Zucker zucker)
		{
            if (AktuelleLast == 0)
            {
                GeheZuZiel(zucker);
            }
		}

		public override void Sieht(Obst obst)
		{
            if (BrauchtNochTräger(obst))
            {
                GeheZuZiel(obst);
            }
		}

		public override void ZielErreicht(Zucker zucker)
		{
            // Spray a very large marking, which will be gone very quickly
            // but picked up by a large number of ants.
            SprüheMarkierung(1, 2000);
            Nimm(zucker);
            GeheZuBau();
		}

		public override void ZielErreicht(Obst obst)
		{
            if (BrauchtNochTräger(obst))
            {
                SprüheMarkierung(2, 2000);
                Nimm(obst);
                GeheZuBau();
            }
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

		public override void SiehtFreund(Ameise ameise)
		{
		}

		public override void SiehtVerbündeten(Ameise ameise)
		{
		}

		#endregion

		#region Kampf

		public override void SiehtFeind(Wanze wanze)
		{
            // Check if we collide with the bug
            int relativeRichtung = Koordinate.BestimmeRichtung(this, wanze) - Richtung;
            if (relativeRichtung > -15 && relativeRichtung < 15)
            {
                // If yes, drop the food to walk faster and evade
                LasseNahrungFallen();
                if (relativeRichtung < 0)
                {
                    DreheUmWinkel(20 + relativeRichtung);
                }
                else
                {
                    DreheUmWinkel(-20 - relativeRichtung);
                }
                GeheGeradeaus(100);
            }
		}

		public override void SiehtFeind(Ameise ameise)
		{
		}

		public override void WirdAngegriffen(Wanze wanze)
		{
            // Leave the scene quickly
            LasseNahrungFallen();
            GeheWegVon(wanze, 100);
		}

		public override void WirdAngegriffen(Ameise ameise)
		{
            // Leave the scene quickly
            LasseNahrungFallen();
            GeheWegVon(ameise, 100);
		}

		#endregion

		#region Sonstiges

		public override void IstGestorben(Todesart todesart)
		{
		}

		public override void Tick()
		{
		}

		#endregion
		 
	}
}