using FoodTruck.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OuvertureDAL
    {
        internal bool EstOuvert(DateTime date)
        {
            bool ouvertHabituellement;
            bool? ouvertExceptionnellement;
            bool ouvert;
            ouvertHabituellement = EstOuvertHabituellement(date);
            ouvertExceptionnellement = EstOuvertExceptionnellement(date);
            if (ouvertHabituellement)
            {
                if (ouvertExceptionnellement == null || (bool)ouvertExceptionnellement)
                    ouvert = true;
                else
                    ouvert = false;
            }
            else
            {
                if (ouvertExceptionnellement != null && (bool)ouvertExceptionnellement)
                    ouvert = true;
                else
                    ouvert = false;
            }
            return ouvert;
        }

        public PlageHoraireRetrait ProchainOuvert(DateTime date)
        {
            bool faireRecherche = true;
            PlageHoraireRetrait plageHoraireRetrait = new PlageHoraireRetrait(date, date); //assignation fantaisiste uniquement pour leurer return
            while (faireRecherche)
            {
                faireRecherche = false;
                PlageRepas prochainOuvertHabituellement = ProchainOuvertHabituellement(date);
                JourExceptionnel prochainOuvertExceptionnellement = ProchainOuvertExceptionnellement(date);
                JourExceptionnel prochainFermeExceptionnellement = ProchainFermeExceptionnellement(date);

                DateTime dateAMJ;
                int jourOuvertHabituellement = prochainOuvertHabituellement.JourSemaineId;
                int jourJ = (int)date.DayOfWeek;
                if (jourOuvertHabituellement < jourJ)
                {
                    dateAMJ = date.AddDays(7 - jourJ + jourOuvertHabituellement);
                }
                else
                {
                    dateAMJ = date.AddDays(jourOuvertHabituellement - jourJ);
                }
                dateAMJ = new DateTime(dateAMJ.Year, dateAMJ.Month, dateAMJ.Day);

                plageHoraireRetrait = new PlageHoraireRetrait(dateAMJ + prochainOuvertHabituellement.Debut, dateAMJ + prochainOuvertHabituellement.Fin);

                if (prochainOuvertExceptionnellement != null && prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.PremierCreneau && prochainOuvertExceptionnellement.DateDebut < prochainFermeExceptionnellement.DateDebut)
                {
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainOuvertExceptionnellement.DateDebut, prochainOuvertExceptionnellement.DateFin);
                }
                if (prochainFermeExceptionnellement != null && prochainFermeExceptionnellement.DateDebut < plageHoraireRetrait.PremierCreneau && prochainFermeExceptionnellement.DateFin > plageHoraireRetrait.DernierCreneau)
                {
                    faireRecherche = true;
                    date = prochainFermeExceptionnellement.DateFin;
                }
                else if (prochainFermeExceptionnellement != null && prochainFermeExceptionnellement.DateDebut < plageHoraireRetrait.PremierCreneau && prochainFermeExceptionnellement.DateFin > plageHoraireRetrait.PremierCreneau && prochainFermeExceptionnellement.DateFin < plageHoraireRetrait.DernierCreneau)
                {
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainFermeExceptionnellement.DateFin, plageHoraireRetrait.DernierCreneau);
                }
            }
            return plageHoraireRetrait;
        }

        private bool EstOuvertHabituellement(DateTime date)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DayOfWeek jourSemaine = date.DayOfWeek;
                TimeSpan heure = date.TimeOfDay;

                PlageRepas ouvert = (from p in db.PlageRepas
                                     where p.JourSemaineId == (int)jourSemaine &&
                                     DbFunctions.DiffMinutes(p.Debut, heure) >= 0 && DbFunctions.DiffMinutes(heure, p.Fin) >= 0
                                     select p).FirstOrDefault();
                if (ouvert != null)
                    return true;
                else
                    return false;
            }
        }
        internal PlageRepas ProchainOuvertHabituellement(DateTime date) // passer private
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas plageOk;
                TimeSpan minuit = new TimeSpan(0, 0, 0);
                int totalSecondes = 24 * 60 * 60 * ((int)date.DayOfWeek - 1) + (int)date.TimeOfDay.TotalSeconds;

                plageOk = (from c in db.PlageRepas
                           where totalSecondes <= 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin)
                           orderby 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin) // TODO voir alias ?
                           select c).FirstOrDefault();

                if (plageOk == null)
                {
                    plageOk = (from c in db.PlageRepas
                               orderby c.JourSemaineId, c.Debut
                               select c).First();
                }
                return plageOk;
            }
        }
        private bool? EstOuvertExceptionnellement(DateTime date)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jour = (from jexc in db.JourExceptionnel
                                         where DbFunctions.DiffMinutes(jexc.DateDebut, date) >= 0 && DbFunctions.DiffMinutes(date, jexc.DateFin) >= 0
                                         select jexc).FirstOrDefault();
                if (jour == null)
                    return null;
                else
                    return jour.Ouvert;
            }
        }

        internal JourExceptionnel ProchainOuvertExceptionnellement(DateTime date) // passer private
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jour = (from j in db.JourExceptionnel
                                         where DbFunctions.DiffSeconds(date, j.DateFin) > 0 && j.Ouvert
                                         orderby j.DateDebut
                                         select j).FirstOrDefault();
                return jour;
            }
        }

        internal JourExceptionnel ProchainFermeExceptionnellement(DateTime date) // passer private
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jour = (from j in db.JourExceptionnel
                                         where DbFunctions.DiffSeconds(date, j.DateFin) > 0 && !j.Ouvert
                                         orderby j.DateDebut
                                         select j).FirstOrDefault();
                return jour;
            }
        }
    }
}