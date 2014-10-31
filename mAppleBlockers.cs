using System;
using System.Collections.Generic;

using AntMe.Deutsch;

// F�ge hier hinter AntMe.Spieler einen Punkt und deinen Namen ohne Leerzeichen
// ein! Zum Beispiel "AntMe.Spieler.WolfgangGallo".
namespace AntMe.Spieler
{

    /// <summary>
    /// Ameisendemo die sich darauf konzentriert effizient �pfel einzusammeln.
    /// Andere Nahrungsmittel werden ignoriert und den K�fern wird nur versucht
    /// auszuweichen.
    /// </summary>

    [Spieler(
        Volkname = "mAppleBlockers",
        Vorname = "Martin",
        Nachname = "Disch"
        )]

    [Kaste(
        Name = "Sammler",
        GeschwindigkeitModifikator = 0,
        DrehgeschwindigkeitModifikator = -1,
        LastModifikator = 2,
        ReichweiteModifikator = -1,
        SichtweiteModifikator = 2,
        EnergieModifikator = -1,
        AngriffModifikator = -1
        )]

    [Kaste(
        Name = "K�mpfer",
        GeschwindigkeitModifikator = -1,
        DrehgeschwindigkeitModifikator = -1,
        LastModifikator = -1,
        ReichweiteModifikator = -1,
        SichtweiteModifikator = 0,
        EnergieModifikator = 2,
        AngriffModifikator = 2
    )]

    public class mAppleBlocker : Basisameise
    {
        bool guarding = false;
        bool engaging = false;
        /// <summary>
        /// Bestimmt den Typ einer neuen Ameise.
        /// </summary>
        /// <param name="anzahl">Die Anzahl der von jedem Typ bereits
        /// vorhandenen Ameisen.</param>
        /// <returns>Der Name des Typs der Ameise.</returns>
        public override string BestimmeKaste(Dictionary<string, int> anzahl)
        {
                if (anzahl["K�mpfer"] <= anzahl["Sammler"])
                {
                    return "K�mpfer";
                }
            return "Sammler";
        }

        #region Fortbewegung

        /// <summary>
        /// Wird wiederholt aufgerufen, wenn der die Ameise nicht weiss wo sie
        /// hingehen soll.
        /// </summary>
        public override void Wartet()
        {
            // Sollte die Ameise au�erhalb des Nahrungsmittelradiuses liegen...
            if (EntfernungZuBau > 400 && Kaste != "K�mpfer")
            {
                // ... soll sie wieder heim gehen.
                GeheZuBau();
            }
            else
            {
                if (!guarding)
                {
                    // ... ansonsten soll sie sich ein bischen drehen (zuf�lliger Winkel
                    // zwischen -10 und 10 Grad) und wieder ein paar Schritte laufen.
                    DreheUmWinkel(Zufall.Zahl(-10, 10));
                    GeheGeradeaus(40);
                }
                else
                {
                    DreheUmWinkel(90);
                    GeheGeradeaus(20);
                }
            }

            // Wenn die restliche verf�gbare Strecke der Ameise (minus einem Puffer
            // von 50 Schritten) kleiner als die Entfernung zum Bau ist...
            if (Reichweite - Zur�ckgelegteStrecke - 50 < EntfernungZuBau)
            {
                // ... soll sie nach Hause gehen um nicht zu sterben.
                GeheZuBau();
            }

        }

        #endregion
        #region Nahrung

        /// <summary>
        /// Wird wiederholt aufgerufen, wenn die Ameise mindstens ein
        /// Obstst�ck sieht.
        /// </summary>
        /// <param name="obst">Das n�chstgelegene Obstst�ck.</param>
        public override void Sieht(Obst obst)
        {
            // Sofern der Apfel noch Tr�ger braucht soll die Ameise zum Apfel.
            if (BrauchtNochTr�ger(obst))
            {
                GeheZuZiel(obst);
            }
        }

        /// <summary>
        /// Wird einmal aufgerufen, wenn die Ameise ein Obstst�ck als Ziel hat und
        /// bei diesem ankommt.
        /// </summary>
        /// <param name="obst">Das Obst�ck.</param>
        public override void ZielErreicht(Obst obst)
        {
            // Die Ameise soll nochmal pr�fen ob der Apfel �berhaupt noch Tr�ger
            // braucht.
            if (BrauchtNochTr�ger(obst))
            {
                // Wenn noch Tr�ger gebraucht werden soll die Ameise eine Markierung
                // spr�hen die als Information die Menge ben�tigter Ameisen hat. Da die
                // ben�tigte Menge nicht genau ermittelt werden kann wird hier nur
                // gesch�tzt. Es wird erwartet, dass 20 gebraucht werden und dass in
                // "AnzahlInSichtweite" etwa die Zahl tragenden Ameisen steckt.
                Spr�heMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                if (Kaste == "Sammler")
                {
                    Nimm(obst);
                    GeheZuBau();
                }
            }
        }

        public override void Sieht(Zucker zucker)
        {
            if (Kaste == "K�mpfer" && !guarding)
            {
                GeheZuZiel(zucker);
            }
        }

        public override void ZielErreicht(Zucker zucker)
        {
            if (Kaste == "K�mpfer")
            {
                GeheGeradeaus(40);
                guarding = true;
            }
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

            if (Kaste == "Sammler")
            {
                // Sollte die Ameise nicht schon Obst im Auge haben oder auf dem Weg zum
                // Bau sein soll sie, wenn die angeforderte Menge Ameisen die Ameisenmenge
                // der gerade in Sichtweite befindlichen Ameisen �bersteigt, zum
                // Markierungsmittelpunkt gehen um dort hoffentlich den Apfel zu sehen.
                if (!(Ziel is Obst) &&
                    !(Ziel is Bau) &&
                    AnzahlAmeisenInSichtweite < markierung.Information)
                {
                    GeheZuZiel(markierung);
                    // Sollte die Entfernung mehr als 50 schritte zum Mittelpunkt betragen,
                    // soll eine Folgemarkierung gespr�ht werden um denn Effektradius zu 
                    // erh�hen.
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        Spr�heMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
                else
                {
                    // In allen anderen F�llen soll sie kurz stehen bleiben um zu
                    // verhindern, dass die Ameise dem Apfel ewig hinterher l�uft.
                    BleibStehen();
                }
            }
            else
            {
                if (Ziel is Wanze)
                {
                    GeheZuZiel(markierung);
                    // Sollte die Entfernung mehr als 50 schritte zum Mittelpunkt betragen,
                    // soll eine Folgemarkierung gespr�ht werden um denn Effektradius zu 
                    // erh�hen.
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        Spr�heMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
            }
            
        }

        #endregion
        #region Kampf

        /// <summary>
        /// Wird wiederholt aufgerufen, wenn die Ameise mindestens einen K�fer
        /// sieht.
        /// </summary>
        /// <param name="wanze">Der n�chstgelegene Wanze.</param>
        public override void SiehtFeind(Wanze wanze)
        {
            guarding = false;
            if (Kaste == "K�mpfer")
            {
                engaging = true;
                Spr�heMarkierung(0, 150);
                GreifeAn(wanze);
            }
            else
            {
                // Bei K�fersicht wird ermittelt ob die Ameise evtl. kollidiert, wenn sie
                // geradeaus weitergeht.
                int relativeRichtung =
                    Koordinate.BestimmeRichtung(this, wanze) - Richtung;
                if (relativeRichtung > -15 && relativeRichtung < 15)
                {
                    // Wenn ja, soll sie erstmal die Nahrung fallen lassen um schneller zu
                    // laufen und dann, je nachdem auf welcher Seite der K�fer ist, in einem
                    // 20 Grad-Winkel in die andere Richtung weggehen.
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
            
        }

        public override void SiehtFeind(Ameise ameise)
        {
            guarding = false;
            if (Kaste == "K�mpfer")
            {
                engaging = true;
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Ameise ameise)
        {
            guarding = false;
            if (Kaste == "K�mpfer")
            {
                engaging = true;
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Wanze wanze)
        {
            guarding = false;
            if (Kaste == "K�mpfer")
            {
                engaging = true;
                Spr�heMarkierung(0, 150);
                GreifeAn(wanze);
            }
        }

        #endregion
        #region Sonstiges

        /// <summary>
        /// Wird unabh�ngig von �u�eren Umst�nden in jeder Runde aufgerufen.
        /// </summary>
        public override void Tick()
        {

            if (Kaste == "K�mpfer")
            {
                // Sollte eine Ameise durch den Kampf unter die 2/3-Marke ihrer Energie
                // fallen soll sie nach Hause gehen um aufzuladen.
                if (AktuelleEnergie < MaximaleEnergie * 2 / 3)
                {
                    GeheZuBau();
                }
            }

            // Sollte die Ameise gerade mit Nahrung unterwegs sein...
            if (Ziel != null && GetragenesObst != null)
            {
                // ... und noch Helfer f�r den Apfel gebraucht werden...
                if (BrauchtNochTr�ger(GetragenesObst))
                {
                    // ... soll sie eine Markierung spr�hen die die Information enth�lt,
                    // wie viele Ameisen noch beim Tragen helfen sollen.
                    Spr�heMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                }
            }

            // Sollte die Ameise, w�hrend sie Obst tr�gt, das Ziel "Bau" verlieren,
            // wird das Ziel neu gesetzt.
            if (GetragenesObst != null)
            {
                GeheZuBau();
            }

            // Sollte die Ameise einem St�ck Obst hinterher laufen das garkeine Tr�ger
            // mehr braucht soll sie stehen bleiben um anschlie�end durch "wartet"
            // wieder umher geschickt zu werden.
            if (Ziel is Obst && !BrauchtNochTr�ger((Obst)Ziel))
            {
                BleibStehen();
            }

        }

        #endregion

    }
}