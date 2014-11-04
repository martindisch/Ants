using System;
using System.Collections.Generic;

using AntMe.Deutsch;

// Füge hier hinter AntMe.Spieler einen Punkt und deinen Namen ohne Leerzeichen
// ein! Zum Beispiel "AntMe.Spieler.WolfgangGallo".
namespace AntMe.Spieler
{

	// Das Spieler-Attribut erlaubt das Festlegen des Volk-Names und von Vor-
	// und Nachname des Spielers. Der Volk-Name muß zugewiesen werden, sonst wird
	// das Volk nicht gefunden.
	[Spieler(
		Volkname = "mSugarAnts",
		Vorname = "Martin",
		Nachname = "Disch"
	)]

	// Das Typ-Attribut erlaubt das Ändern der Ameisen-Eigenschaften. Um den Typ
	// zu aktivieren muß ein Name zugewiesen und dieser Name in der Methode 
	// BestimmeTyp zurückgegeben werden. Das Attribut kann kopiert und mit
	// verschiedenen Namen mehrfach verwendet werden.
	// Eine genauere Beschreibung gibts in Lektion 6 des Ameisen-Tutorials.
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

		/// <summary>
		/// Bestimmt die Kaste einer neuen Ameise.
		/// </summary>
		/// <param name="anzahl">Die Anzahl der von jeder Kaste bereits vorhandenen
		/// Ameisen.</param>
		/// <returns>Der Name der Kaste der Ameise.</returns>
		public override string BestimmeKaste(Dictionary<string, int> anzahl)
		{
			return "Standard";
		}

		#endregion

		#region Fortbewegung

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn der die Ameise nicht weiss wo sie
		/// hingehen soll.
		/// </summary>
		public override void Wartet()
		{
            if (Ziel == null)
            {
                DreheUmWinkel(Zufall.Zahl(-10, 10));
                GeheGeradeaus(20);
            }
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise ein Drittel ihrer maximalen
		/// Reichweite überschritten hat.
		/// </summary>
		public override void WirdMüde()
		{
            GeheZuBau();
		}

		#endregion

		#region Nahrung

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens einen
		/// Zuckerhaufen sieht.
		/// </summary>
		/// <param name="zucker">Der nächstgelegene Zuckerhaufen.</param>
		public override void Sieht(Zucker zucker)
		{
            if (AktuelleLast == 0)
            {
                GeheZuZiel(zucker);
            }
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindstens ein
		/// Obststück sieht.
		/// </summary>
		/// <param name="obst">Das nächstgelegene Obststück.</param>
		public override void Sieht(Obst obst)
		{
            if (BrauchtNochTräger(obst))
            {
                GeheZuZiel(obst);
            }
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn di e Ameise einen Zuckerhaufen als Ziel
		/// hat und bei diesem ankommt.
		/// </summary>
		/// <param name="zucker">Der Zuckerhaufen.</param>
		public override void ZielErreicht(Zucker zucker)
		{
            SprüheMarkierung(1, 2000);
            Nimm(zucker);
            GeheZuBau();
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise ein Obststück als Ziel hat und
		/// bei diesem ankommt.
		/// </summary>
		/// <param name="obst">Das Obstück.</param>
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

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise eine Markierung des selben
		/// Volkes riecht. Einmal gerochene Markierungen werden nicht erneut
		/// gerochen.
		/// </summary>
		/// <param name="markierung">Die nächste neue Markierung.</param>
		public override void RiechtFreund(Markierung markierung)
		{
            if (Ziel == null)
            {
                GeheZuZiel(markierung);
            }
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindstens eine Ameise des
		/// selben Volkes sieht.
		/// </summary>
		/// <param name="ameise">Die nächstgelegene befreundete Ameise.</param>
		public override void SiehtFreund(Ameise ameise)
		{
		}

		/// <summary>
		/// Wird aufgerufen, wenn die Ameise eine befreundete Ameise eines anderen Teams trifft.
		/// </summary>
		/// <param name="ameise"></param>
		public override void SiehtVerbündeten(Ameise ameise)
		{
		}

		#endregion

		#region Kampf

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens eine Wanze
		/// sieht.
		/// </summary>
		/// <param name="wanze">Die nächstgelegene Wanze.</param>
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

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens eine Ameise eines
		/// anderen Volkes sieht.
		/// </summary>
		/// <param name="ameise">Die nächstgelegen feindliche Ameise.</param>
		public override void SiehtFeind(Ameise ameise)
		{
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise von einer Wanze angegriffen
		/// wird.
		/// </summary>
		/// <param name="wanze">Die angreifende Wanze.</param>
		public override void WirdAngegriffen(Wanze wanze)
		{
		}

		/// <summary>
		/// Wird wiederholt aufgerufen in der die Ameise von einer Ameise eines
		/// anderen Volkes Ameise angegriffen wird.
		/// </summary>
		/// <param name="ameise">Die angreifende feindliche Ameise.</param>
		public override void WirdAngegriffen(Ameise ameise)
		{
		}

		#endregion

		#region Sonstiges

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise gestorben ist.
		/// </summary>
		/// <param name="todesart">Die Todesart der Ameise</param>
		public override void IstGestorben(Todesart todesart)
		{
		}

		/// <summary>
		/// Wird unabhängig von äußeren Umständen in jeder Runde aufgerufen.
		/// </summary>
		public override void Tick()
		{
		}

		#endregion
		 
	}
}