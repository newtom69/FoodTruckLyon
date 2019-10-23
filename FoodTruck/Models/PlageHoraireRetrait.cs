using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.Models
{
    public class PlageHoraireRetrait
    {
        public DateTime PremierCreneau { get; private set; }
        public DateTime DernierCreneau { get; private set; }
        public int Annee { get; private set; }
        public int Mois { get; private set; }
        public int Jour { get; private set; }
        public TimeSpan Pas { get; private set; }
        public int TypeRepas { get; private set; }
        public List<DateTime> Creneaux { get; private set; }


        public PlageHoraireRetrait(int heurePremierCreneau, int minutePremierCreneau, int heureDernierCreneau, int minuteDernierCreneau, TimeSpan pas, int annee = 0, int mois = 0, int jour = 0)
        {
            DateTime maintenant = DateTime.Now;
            Annee = annee == 0 ? maintenant.Year : annee;
            Mois = mois == 0 ? maintenant.Month : mois;
            Jour = jour == 0 ? maintenant.Day : jour;
            PremierCreneau = new DateTime(Annee, Mois, Jour, heurePremierCreneau, minutePremierCreneau, 0);
            DernierCreneau = new DateTime(Annee, Mois, Jour, heureDernierCreneau, minuteDernierCreneau, 0);
            if (pas > new TimeSpan(0))
                Pas = pas;
            else
                Pas = -pas;
            ConstruireCreneaux();
            SetTypeRepas();
        }
        public PlageHoraireRetrait(DateTime premierCreneau, DateTime dernierCreneau, TimeSpan pas)
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
            if (pas > new TimeSpan(0))
                Pas = pas;
            else
                Pas = -pas;

            ConstruireCreneaux();
            SetTypeRepas();
        }

        public PlageHoraireRetrait AddDays(int nbJours = 1)
        {

            DateTime premierCreneau = PremierCreneau.AddDays(nbJours);
            DateTime dernierCreneau = DernierCreneau.AddDays(nbJours);
            TimeSpan pas = Pas;
            return new PlageHoraireRetrait(premierCreneau, dernierCreneau, pas);
        }

        public bool Contient(DateTime date)
        {
            if (PremierCreneau <= date && date <= DernierCreneau)
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
        public bool Avant(DateTime date)
        {
            if (DernierCreneau + Pas < date)
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
            int annee = date.Year;
            int mois = date.Month;
            int jour = date.Day;
            int heure = date.Hour;
            int minute = date.Minute;
            int minuteCreneauCourant = (minute / (int)Pas.TotalMinutes) * (int)Pas.TotalMinutes;
            return new DateTime(annee, mois, jour, heure, minuteCreneauCourant, 0);
        }
        private DateTime ObtenirCreneauSuivant(DateTime date)
        {
            return ObtenirCreneauCourant(date).AddMinutes(Pas.TotalMinutes);
        }

        private void SetTypeRepas()
        {
            DateTime maintenant = DateTime.Now;
            int jAnnee = maintenant.Year;
            int jMois = maintenant.Month;
            int jJour = maintenant.Day;
            DateTime seizeHeure = new DateTime(jAnnee, jMois, jJour, 16, 0, 0);
            if (Apres(seizeHeure))
            {
                TypeRepas = 2;
            }
            else
            {
                TypeRepas = 1;
            }
        }
    }
}