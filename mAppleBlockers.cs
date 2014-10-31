using System;
using System.Collections.Generic;

using AntMe.Deutsch;

// Füge hier hinter AntMe.Spieler einen Punkt und deinen Namen ohne Leerzeichen
// ein! Zum Beispiel "AntMe.Spieler.WolfgangGallo".
namespace AntMe.Spieler
{

    /// <summary>
    /// Ameisendemo die sich darauf konzentriert effizient Äpfel einzusammeln.
    /// Andere Nahrungsmittel werden ignoriert und den Käfern wird nur versucht
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
        Name = "Kämpfer",
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
                if (anzahl["Kämpfer"] <= anzahl["Sammler"])
                {
                    return "Kämpfer";
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
            // Sollte die Ameise außerhalb des Nahrungsmittelradiuses liegen...
            if (EntfernungZuBau > 400 && Kaste != "Kämpfer")
            {
                // ... soll sie wieder heim gehen.
                GeheZuBau();
            }
            else
            {
                if (!guarding)
                {
                    // ... ansonsten soll sie sich ein bischen drehen (zufälliger Winkel
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

            // Wenn die restliche verfügbare Strecke der Ameise (minus einem Puffer
            // von 50 Schritten) kleiner als die Entfernung zum Bau ist...
            if (Reichweite - ZurückgelegteStrecke - 50 < EntfernungZuBau)
            {
                // ... soll sie nach Hause gehen um nicht zu sterben.
                GeheZuBau();
            }

        }

        #endregion
        #region Nahrung

        /// <summary>
        /// Wird wiederholt aufgerufen, wenn die Ameise mindstens ein
        /// Obststück sieht.
        /// </summary>
        /// <param name="obst">Das nächstgelegene Obststück.</param>
        public override void Sieht(Obst obst)
        {
            // Sofern der Apfel noch Träger braucht soll die Ameise zum Apfel.
            if (BrauchtNochTräger(obst))
            {
                GeheZuZiel(obst);
            }
        }

        /// <summary>
        /// Wird einmal aufgerufen, wenn die Ameise ein Obststück als Ziel hat und
        /// bei diesem ankommt.
        /// </summary>
        /// <param name="obst">Das Obstück.</param>
        public override void ZielErreicht(Obst obst)
        {
            // Die Ameise soll nochmal prüfen ob der Apfel überhaupt noch Träger
            // braucht.
            if (BrauchtNochTräger(obst))
            {
                // Wenn noch Träger gebraucht werden soll die Ameise eine Markierung
                // sprühen die als Information die Menge benötigter Ameisen hat. Da die
                // benötigte Menge nicht genau ermittelt werden kann wird hier nur
                // geschätzt. Es wird erwartet, dass 20 gebraucht werden und dass in
                // "AnzahlInSichtweite" etwa die Zahl tragenden Ameisen steckt.
                SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                if (Kaste == "Sammler")
                {
                    Nimm(obst);
                    GeheZuBau();
                }
            }
        }

        public override void Sieht(Zucker zucker)
        {
            if (Kaste == "Kämpfer" && !guarding)
            {
                GeheZuZiel(zucker);
            }
        }

        public override void ZielErreicht(Zucker zucker)
        {
            if (Kaste == "Kämpfer")
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
        /// <param name="markierung">Die nächste neue Markierung.</param>
        public override void RiechtFreund(Markierung markierung)
        {

            if (Kaste == "Sammler")
            {
                // Sollte die Ameise nicht schon Obst im Auge haben oder auf dem Weg zum
                // Bau sein soll sie, wenn die angeforderte Menge Ameisen die Ameisenmenge
                // der gerade in Sichtweite befindlichen Ameisen übersteigt, zum
                // Markierungsmittelpunkt gehen um dort hoffentlich den Apfel zu sehen.
                if (!(Ziel is Obst) &&
                    !(Ziel is Bau) &&
                    AnzahlAmeisenInSichtweite < markierung.Information)
                {
                    GeheZuZiel(markierung);
                    // Sollte die Entfernung mehr als 50 schritte zum Mittelpunkt betragen,
                    // soll eine Folgemarkierung gesprüht werden um denn Effektradius zu 
                    // erhöhen.
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        SprüheMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
                else
                {
                    // In allen anderen Fällen soll sie kurz stehen bleiben um zu
                    // verhindern, dass die Ameise dem Apfel ewig hinterher läuft.
                    BleibStehen();
                }
            }
            else
            {
                if (Ziel is Wanze)
                {
                    GeheZuZiel(markierung);
                    // Sollte die Entfernung mehr als 50 schritte zum Mittelpunkt betragen,
                    // soll eine Folgemarkierung gesprüht werden um denn Effektradius zu 
                    // erhöhen.
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        SprüheMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
            }
            
        }

        #endregion
        #region Kampf

        /// <summary>
        /// Wird wiederholt aufgerufen, wenn die Ameise mindestens einen Käfer
        /// sieht.
        /// </summary>
        /// <param name="wanze">Der nächstgelegene Wanze.</param>
        public override void SiehtFeind(Wanze wanze)
        {
            guarding = false;
            if (Kaste == "Kämpfer")
            {
                engaging = true;
                SprüheMarkierung(0, 150);
                GreifeAn(wanze);
            }
            else
            {
                // Bei Käfersicht wird ermittelt ob die Ameise evtl. kollidiert, wenn sie
                // geradeaus weitergeht.
                int relativeRichtung =
                    Koordinate.BestimmeRichtung(this, wanze) - Richtung;
                if (relativeRichtung > -15 && relativeRichtung < 15)
                {
                    // Wenn ja, soll sie erstmal die Nahrung fallen lassen um schneller zu
                    // laufen und dann, je nachdem auf welcher Seite der Käfer ist, in einem
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
            if (Kaste == "Kämpfer")
            {
                engaging = true;
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Ameise ameise)
        {
            guarding = false;
            if (Kaste == "Kämpfer")
            {
                engaging = true;
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Wanze wanze)
        {
            guarding = false;
            if (Kaste == "Kämpfer")
            {
                engaging = true;
                SprüheMarkierung(0, 150);
                GreifeAn(wanze);
            }
        }

        #endregion
        #region Sonstiges

        /// <summary>
        /// Wird unabhängig von äußeren Umständen in jeder Runde aufgerufen.
        /// </summary>
        public override void Tick()
        {

            if (Kaste == "Kämpfer")
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
                // ... und noch Helfer für den Apfel gebraucht werden...
                if (BrauchtNochTräger(GetragenesObst))
                {
                    // ... soll sie eine Markierung sprühen die die Information enthält,
                    // wie viele Ameisen noch beim Tragen helfen sollen.
                    SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                }
            }

            // Sollte die Ameise, während sie Obst trägt, das Ziel "Bau" verlieren,
            // wird das Ziel neu gesetzt.
            if (GetragenesObst != null)
            {
                GeheZuBau();
            }

            // Sollte die Ameise einem Stück Obst hinterher laufen das garkeine Träger
            // mehr braucht soll sie stehen bleiben um anschließend durch "wartet"
            // wieder umher geschickt zu werden.
            if (Ziel is Obst && !BrauchtNochTräger((Obst)Ziel))
            {
                BleibStehen();
            }

        }

        #endregion

    }
}