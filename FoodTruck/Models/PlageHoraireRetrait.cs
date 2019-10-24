using System;
using System.Collections.Generic;
using System.Configuration;

namespace FoodTruck.Models
{
    public class PlageHoraireRetrait
    {
        private static readonly TimeSpan Pas = new TimeSpan(0, int.Parse(ConfigurationManager.AppSettings["IntervalleCreneaux"]), 0);
        public DateTime PremierCreneau { get; private set; }
        public DateTime DernierCreneau { get; private set; }
        public TypeRepas TypeRepas { get; private set; }
        public List<DateTime> Creneaux { get; private set; }

        public PlageHoraireRetrait(DateTime premierCreneau, DateTime dernierCreneau)
        {
            if (premierCreneau < dernierCreneau)
            {
                PremierCreneau = premierCreneau;
                DernierCreneau = dernierCreneau;
            }
            else
            {
                PremierCreneau = dernierCreneau;
                DernierCreneau = premierCreneau;
            }
            ConstruireCreneaux();
            SetTypeRepas();
        }
        public PlageHoraireRetrait PlageHoraireRetraitSuivante()
        {
            DateTime nouvelleDate = DernierCreneau + Pas + new TimeSpan(0, 1, 0);
            //bug sur le type de repas retourné
            return nouvelleDate.PlageHoraireRetrait();
        }
        public bool Contient(DateTime date)
        {
            if (PremierCreneau <= date && date <= DernierCreneau)
                return true;
            else
                return false;
        }
        public bool Apres(TimeSpan heuresMinutes)
        {
            TimeSpan creneau = new TimeSpan(PremierCreneau.Hour, PremierCreneau.Minute, 0);
            if (creneau > heuresMinutes)
                return true;
            else
                return false;
        }
        public bool Apres(DateTime date)
        {
            if (PremierCreneau > date)
                return true;
            else
                return false;
        }
        private void ConstruireCreneaux()
        {
            DateTime creneauCourant = PremierCreneau;
            Creneaux = new List<DateTime>();
            while (creneauCourant <= DernierCreneau)
            {
                Creneaux.Add(creneauCourant);
                creneauCourant = ObtenirCreneauSuivant(creneauCourant);
            }
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
        private void SetTypeRepas()
        {
            TimeSpan seizeHeure = new TimeSpan(16, 0, 0);
            if (Apres(seizeHeure))
            {
                TypeRepas = TypeRepas.Diner;
            }
            else
            {
                TypeRepas = TypeRepas.Dejeuner;
            }
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
                if(compteur >0)
                    Creneaux.RemoveRange(indexMin, compteur);
                PremierCreneau = Creneaux[0];
            }
        }
    }
}