using System;
using System.Collections.Generic;

using AntMe.Deutsch;

// F�ge hier hinter AntMe.Spieler einen Punkt und deinen Namen ohne Leerzeichen
// ein! Zum Beispiel "AntMe.Spieler.WolfgangGallo".
namespace AntMe.Spieler
{

	// Das Spieler-Attribut erlaubt das Festlegen des Volk-Names und von Vor-
	// und Nachname des Spielers. Der Volk-Name mu� zugewiesen werden, sonst wird
	// das Volk nicht gefunden.
	[Spieler(
		Volkname = "mAggressorAnts",
		Vorname = "Martin",
		Nachname = "Disch"
	)]

	// Das Typ-Attribut erlaubt das �ndern der Ameisen-Eigenschaften. Um den Typ
	// zu aktivieren mu� ein Name zugewiesen und dieser Name in der Methode 
	// BestimmeTyp zur�ckgegeben werden. Das Attribut kann kopiert und mit
	// verschiedenen Namen mehrfach verwendet werden.
	// Eine genauere Beschreibung gibts in Lektion 6 des Ameisen-Tutorials.
    [Kaste(
        Name = "Soldier",
        GeschwindigkeitModifikator = -1,
        DrehgeschwindigkeitModifikator = -1,
        LastModifikator = -1,
        ReichweiteModifikator = -1,
        SichtweiteModifikator = 0,
        EnergieModifikator = 2,
        AngriffModifikator = 2
    )]
    [Kaste(
        Name = "Scout",
        GeschwindigkeitModifikator = 2,
        DrehgeschwindigkeitModifikator = -1,
        LastModifikator = -1,
        ReichweiteModifikator = 0,
        SichtweiteModifikator = 2,
        EnergieModifikator = -1,
        AngriffModifikator = -1
    )]

	public class mAggressorAnt : Basisameise
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
            if (anzahl["Scout"] > 0)
            {
                if (anzahl["Soldier"] / anzahl["Scout"] < 3)
                {
                    return "Soldier";
                }
            }
            return "Scout";
		}

		#endregion

		#region Fortbewegung

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn der die Ameise nicht weiss wo sie
		/// hingehen soll.
		/// </summary>
		public override void Wartet()
		{
            DreheUmWinkel(Zufall.Zahl(-45, 45));
            GeheGeradeaus(500);
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise ein Drittel ihrer maximalen
		/// Reichweite �berschritten hat.
		/// </summary>
		public override void WirdM�de()
		{
            GeheZuBau();
		}

		#endregion

		#region Nahrung

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens einen
		/// Zuckerhaufen sieht.
		/// </summary>
		/// <param name="zucker">Der n�chstgelegene Zuckerhaufen.</param>
		public override void Sieht(Zucker zucker)
		{
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindstens ein
		/// Obstst�ck sieht.
		/// </summary>
		/// <param name="obst">Das n�chstgelegene Obstst�ck.</param>
		public override void Sieht(Obst obst)
		{
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn di e Ameise einen Zuckerhaufen als Ziel
		/// hat und bei diesem ankommt.
		/// </summary>
		/// <param name="zucker">Der Zuckerhaufen.</param>
		public override void ZielErreicht(Zucker zucker)
		{
		}

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise ein Obstst�ck als Ziel hat und
		/// bei diesem ankommt.
		/// </summary>
		/// <param name="obst">Das Obst�ck.</param>
		public override void ZielErreicht(Obst obst)
		{
		}

		#endregion

		#region Kommunikation

		/// <summary>
		/// Wird einmal aufgerufen, wenn die Ameise eine Markierung des selben
		/// Volkes riecht. Einmal gerochene Markierungen werden nicht erneut
		/// gerochen.
		/// </summary>
		/// <param name="markierung">Die n�chste neue Markierung.</param>
		public override void RiechtFreund(Markierung markierung)
		{
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindstens eine Ameise des
		/// selben Volkes sieht.
		/// </summary>
		/// <param name="ameise">Die n�chstgelegene befreundete Ameise.</param>
		public override void SiehtFreund(Ameise ameise)
		{
		}

		/// <summary>
		/// Wird aufgerufen, wenn die Ameise eine befreundete Ameise eines anderen Teams trifft.
		/// </summary>
		/// <param name="ameise"></param>
		public override void SiehtVerb�ndeten(Ameise ameise)
		{
		}

		#endregion

		#region Kampf

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens eine Wanze
		/// sieht.
		/// </summary>
		/// <param name="wanze">Die n�chstgelegene Wanze.</param>
		public override void SiehtFeind(Wanze wanze)
		{
		}

		/// <summary>
		/// Wird wiederholt aufgerufen, wenn die Ameise mindestens eine Ameise eines
		/// anderen Volkes sieht.
		/// </summary>
		/// <param name="ameise">Die n�chstgelegen feindliche Ameise.</param>
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
		/// Wird unabh�ngig von �u�eren Umst�nden in jeder Runde aufgerufen.
		/// </summary>
		public override void Tick()
		{
            if (AktuelleEnergie < MaximaleEnergie / 2)
            {
                GeheZuBau();
            }
		}

		#endregion
		 
	}
}