using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace FoodTruck.Models
{
    public class PlageHoraireRetrait
    {
        private TimeSpan Pas = new TimeSpan(0, int.Parse(ConfigurationManager.AppSettings["IntervalleCreneaux"]), 0);
        public List<DateTime> Creneaux { get; private set; }

        public PlageHoraireRetrait(DateTime premierCreneau, DateTime dernierCreneau)
        {
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
        public bool Apres(TimeSpan heuresMinutes)
        {
            TimeSpan creneau = new TimeSpan(Creneaux.First().Hour, Creneaux.First().Minute, 0);
            if (creneau > heuresMinutes)
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