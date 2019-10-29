using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OuvertureDAL
    {
        internal List<JourExceptionnel> ListerFutursFermeturesExceptionnelles()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime date = DateTime.Now;
                List<JourExceptionnel> jours = (from j in db.JourExceptionnel
                                                where DbFunctions.DiffDays(date, j.DateDebut) >= 0 && !j.Ouvert
                                                orderby j.DateDebut
                                                select j).ToList();
                return jours;
            }
        }

        internal JourExceptionnel AjouterFermeture(DateTime dateDebut, DateTime dateFin)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel chevauchement = (from j in db.JourExceptionnel
                                                  where !(DbFunctions.DiffMinutes(dateFin, j.DateDebut) >= 0 || DbFunctions.DiffMinutes(j.DateFin, dateDebut) >= 0)
                                                  select j).FirstOrDefault();
                if (chevauchement == null)
                {
                    JourExceptionnel jour = new JourExceptionnel
                    {
                        DateDebut = dateDebut,
                        DateFin = dateFin,
                        Ouvert = false
                    };
                    db.JourExceptionnel.Add(jour);
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }
        internal JourExceptionnel ModifierFermeture(DateTime dateId, DateTime dateDebut, DateTime dateFin)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jourSelectionne = (from j in db.JourExceptionnel
                                                    where j.DateDebut == dateId && !j.Ouvert
                                                    select j).FirstOrDefault();

                JourExceptionnel chevauchement = (from j in db.JourExceptionnel
                                                  where j.DateDebut != jourSelectionne.DateDebut &&
                                                  !(DbFunctions.DiffMinutes(dateFin, j.DateDebut) >= 0 || DbFunctions.DiffMinutes(j.DateFin, dateDebut) >= 0)
                                                  select j).FirstOrDefault();
                
                if (chevauchement == null && jourSelectionne != null)
                {
                    jourSelectionne.DateDebut = dateDebut;
                    jourSelectionne.DateFin = dateFin;
                    jourSelectionne.Ouvert = false;
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }

        internal PlageHoraireRetrait ProchainOuvert(DateTime date)
        {
            const int minutesMinPrepaCommande = 10; //TODO conf offset pour préparation min commande
            date = date.AddMinutes(minutesMinPrepaCommande);
            bool faireRecherche;
            PlageHoraireRetrait plageHoraireRetrait;
            do
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

                plageHoraireRetrait = new PlageHoraireRetrait(dateAMJ + prochainOuvertHabituellement.Debut, dateAMJ + prochainOuvertHabituellement.Fin, prochainOuvertHabituellement.Pas);

                if (prochainOuvertExceptionnellement != null && prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First() && prochainOuvertExceptionnellement.DateDebut < prochainFermeExceptionnellement.DateDebut)
                {
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainOuvertExceptionnellement.DateDebut, prochainOuvertExceptionnellement.DateFin, prochainOuvertHabituellement.Pas);
                }
                if (prochainFermeExceptionnellement != null && prochainFermeExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First() && prochainFermeExceptionnellement.DateFin > plageHoraireRetrait.Creneaux.Last())
                {
                    faireRecherche = true;
                    date = prochainFermeExceptionnellement.DateFin;
                }
                else if (prochainFermeExceptionnellement != null && prochainFermeExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First() && prochainFermeExceptionnellement.DateFin > plageHoraireRetrait.Creneaux.First() && prochainFermeExceptionnellement.DateFin < plageHoraireRetrait.Creneaux.Last())
                {
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainFermeExceptionnellement.DateFin, plageHoraireRetrait.Creneaux.Last(), plageHoraireRetrait.Pas);
                }
            } while (faireRecherche);
            return plageHoraireRetrait;
        }

        private PlageRepas ProchainOuvertHabituellement(DateTime date)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas plage;
                TimeSpan minuit = new TimeSpan(0, 0, 0);
                int totalSecondes = 24 * 60 * 60 * ((int)date.DayOfWeek - 1) + (int)date.TimeOfDay.TotalSeconds;

                plage = (from c in db.PlageRepas
                         where totalSecondes <= 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin)
                         orderby 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin) // TODO voir as ?
                         select c).FirstOrDefault();

                if (plage == null)
                {
                    plage = (from c in db.PlageRepas
                             orderby c.JourSemaineId, c.Debut
                             select c).First();
                }
                if (plage.Debut.TotalSeconds < (int)date.TimeOfDay.TotalSeconds)
                {
                    TimeSpan heureH = date.TimeOfDay;
                    int pasMinutes = (int)plage.Pas.TotalMinutes;
                    int minutes = (int)Math.Ceiling(heureH.TotalMinutes / pasMinutes) * pasMinutes;
                    int heures = minutes / 60;
                    minutes -= heures * 60;
                    plage.Debut = new TimeSpan(heures, minutes, 0);
                }
                return plage;
            }
        }

        private JourExceptionnel ProchainOuvertExceptionnellement(DateTime date)
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

        private JourExceptionnel ProchainFermeExceptionnellement(DateTime date)
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