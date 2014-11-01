using System;
using System.Collections.Generic;

using AntMe.Deutsch;

namespace AntMe.Spieler
{

    [Spieler(
        Volkname = "mAppleAnts",
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

    public class mAppleAnt : Basisameise
    {
        public override string BestimmeKaste(Dictionary<string, int> anzahl)
        {
                if (anzahl["Kämpfer"] <= anzahl["Sammler"])
                {
                    return "Kämpfer";
                }
            return "Sammler";
        }

        #region Fortbewegung

        public override void Wartet()
        {

            // If outside of the food spawn radius, return home
            if (EntfernungZuBau > 400 && Kaste != "Kämpfer")
            {
                GeheZuBau();
            }
            else
            {
                    // if not, turn randomly and walk
                    DreheUmWinkel(Zufall.Zahl(-10, 10));
                    GeheGeradeaus(20);
            }

            // Return home if ant has walked a lot already
            if (Reichweite - ZurückgelegteStrecke - 50 < EntfernungZuBau)
            {
                GeheZuBau();
            }

        }

        #endregion
        #region Nahrung

        public override void Sieht(Obst obst)
        {
            // Give a hand if more power is needed to carry
            if (BrauchtNochTräger(obst))
            {
                GeheZuZiel(obst);
            }
        }

        public override void ZielErreicht(Obst obst)
        {
            // Check if more ants carrying are required
            if (BrauchtNochTräger(obst))
            {
                // If that's the case, anticipate the amount of ants needed and create a marking
                SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                if (Kaste == "Sammler")
                {
                    Nimm(obst);
                    GeheZuBau();
                }
            }
        }

        #endregion
        #region Kommunikation

        public override void RiechtFreund(Markierung markierung)
        {

            if (Kaste == "Sammler")
            {
                // If apple needs more carrying, go there
                if (!(Ziel is Obst) &&
                    !(Ziel is Bau) &&
                    AnzahlAmeisenInSichtweite < markierung.Information)
                {
                    GeheZuZiel(markierung);
                    // Spray another mark if the way is too long for others to find
                    if (Koordinate.BestimmeEntfernung(this, markierung) > 50)
                    {
                        SprüheMarkierung(
                            Koordinate.BestimmeRichtung(this, markierung),
                            Koordinate.BestimmeEntfernung(this, markierung));
                    }
                }
                else
                {
                    // In all other cases, stand still to not follow the apple indefinitely
                    BleibStehen();
                }
            }
            else
            {
                if (Ziel is Wanze)
                {
                    GeheZuZiel(markierung);
                    // Spray another mark if the way is too long for others to find
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

        public override void SiehtFeind(Wanze wanze)
        {
            if (Kaste == "Kämpfer")
            {
                SprüheMarkierung(0, 150);
                GreifeAn(wanze);
            }
            else
            {
                // Check if we collide with the bug
                int relativeRichtung =
                    Koordinate.BestimmeRichtung(this, wanze) - Richtung;
                if (relativeRichtung > -15 && relativeRichtung < 15)
                {
                    // If yes, drop the apple to walk faster and evade
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
            if (Kaste == "Kämpfer")
            {
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Ameise ameise)
        {
            if (Kaste == "Kämpfer")
            {
                GreifeAn(ameise);
            }
        }

        public override void WirdAngegriffen(Wanze wanze)
        {
            if (Kaste == "Kämpfer")
            {
                SprüheMarkierung(0, 150);
                GreifeAn(wanze);
            }
        }

        #endregion
        #region Sonstiges

        public override void Tick()
        {

            if (Kaste == "Kämpfer")
            {
                // Go home if too much energy has been lost during a fight
                if (AktuelleEnergie < MaximaleEnergie * 2 / 3)
                {
                    GeheZuBau();
                }
            }

            // If more ants are required to carry the apple, call for help
            if (Ziel != null && GetragenesObst != null)
            {
                if (BrauchtNochTräger(GetragenesObst))
                {
                    SprüheMarkierung(20 - AnzahlAmeisenInSichtweite, 200);
                }
            }

            if (GetragenesObst != null)
            {
                GeheZuBau();
            }

            // If no help is needed, stand still to not follow the apple indefinitely
            if (Ziel is Obst && !BrauchtNochTräger((Obst)Ziel))
            {
                BleibStehen();
            }

        }

        #endregion

    }
}