using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.Models
{
    public class PlageHoraireRetrait
    {
        internal TimeSpan Pas { get; private set; }
        public List<DateTime> Creneaux { get; private set; }

        public PlageHoraireRetrait(DateTime premierCreneau, DateTime dernierCreneau, TimeSpan pas)
        {
            Pas = pas;
            DateTime creneauCourant = premierCreneau;
            Creneaux = new List<DateTime>();
            while (creneauCourant <= dernierCreneau)
            {
                Creneaux.Add(creneauCourant);
                creneauCourant = ObtenirCreneauSuivant(creneauCourant);
            }
        }

        public bool Contient(DateTime date)
        {
            if (Creneaux.First() <= date && date <= Creneaux.Last())
                return true;
            else
                return false;
        }

        private DateTime ObtenirCreneauCourant(DateTime date)
        {
            int minute = date.Minute;
            int minuteCreneauCourant = minute / (int)Pas.TotalMinutes * (int)Pas.TotalMinutes;
            return date.AddMinutes(minuteCreneauCourant - minute);
        }
        private DateTime ObtenirCreneauSuivant(DateTime date)
        {
            return ObtenirCreneauCourant(date) + Pas;
        }

        internal void Rogner(DateTime date)
        {
            if (Contient(date))
            {
                int indexMin = -1;
                int compteur = 0;
                foreach (DateTime creneau in Creneaux)
                {
                    if (creneau < date)
                    {
                        compteur++;
                        if (indexMin == -1)
                            indexMin = Creneaux.IndexOf(creneau);
                    }
                }
                if (compteur > 0)
                    Creneaux.RemoveRange(indexMin, compteur);
            }
        }
    }
}