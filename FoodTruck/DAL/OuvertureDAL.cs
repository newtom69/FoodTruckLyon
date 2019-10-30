using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OuvertureDAL
    {
        internal List<JourExceptionnel> ListerFutursFermeturesExceptionnelles()
        {
            return ListerFutursPeriodesExceptionnelles(false);
        }
        internal List<JourExceptionnel> ListerFutursOuverturesExceptionnelles()
        {
            return ListerFutursPeriodesExceptionnelles(true);
        }
        private List<JourExceptionnel> ListerFutursPeriodesExceptionnelles(bool ouvert)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                DateTime date = DateTime.Now;
                List<JourExceptionnel> jours = (from j in db.JourExceptionnel
                                                where DbFunctions.DiffDays(date, j.DateFin) >= 0 && j.Ouvert == ouvert
                                                orderby j.DateDebut
                                                select j).ToList();
                return jours;
            }
        }

        internal JourExceptionnel AjouterFermeture(DateTime dateDebut, DateTime dateFin)
        {
            return AjouterPeriodeExceptionnelle(dateDebut, dateFin, false);
        }
        internal JourExceptionnel AjouterOuverture(DateTime dateDebut, DateTime dateFin)
        {
            return AjouterPeriodeExceptionnelle(dateDebut, dateFin, true);
        }
        private JourExceptionnel AjouterPeriodeExceptionnelle(DateTime dateDebut, DateTime dateFin, bool ouvert)
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
                        Ouvert = ouvert,
                    };
                    db.JourExceptionnel.Add(jour);
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }
        internal JourExceptionnel ModifierFermeture(DateTime dateId, DateTime dateDebut, DateTime dateFin)
        {
            return ModifierPeriodeExceptionnelle(dateId, dateDebut, dateFin, false);
        }
        internal JourExceptionnel ModifierOuverture(DateTime dateId, DateTime dateDebut, DateTime dateFin)
        {
            return ModifierPeriodeExceptionnelle(dateId, dateDebut, dateFin, true);
        }
        private JourExceptionnel ModifierPeriodeExceptionnelle(DateTime dateId, DateTime dateDebut, DateTime dateFin, bool ouvert)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jourSelectionne = (from j in db.JourExceptionnel
                                                    where j.DateDebut == dateId && j.Ouvert == ouvert
                                                    select j).FirstOrDefault();

                JourExceptionnel chevauchement = (from j in db.JourExceptionnel
                                                  where j.DateDebut != jourSelectionne.DateDebut &&
                                                  !(DbFunctions.DiffMinutes(dateFin, j.DateDebut) >= 0 || DbFunctions.DiffMinutes(j.DateFin, dateDebut) >= 0)
                                                  select j).FirstOrDefault();

                if (chevauchement == null && jourSelectionne != null)
                {
                    if (jourSelectionne.DateDebut == dateDebut)
                    {
                        jourSelectionne.DateFin = dateFin;
                        jourSelectionne.Ouvert = ouvert;
                    }
                    else
                    {
                        JourExceptionnel nouveau = new JourExceptionnel
                        {
                            DateDebut = dateDebut,
                            DateFin = dateFin,
                            Ouvert = ouvert,
                        };
                        db.JourExceptionnel.Remove(jourSelectionne);
                        db.JourExceptionnel.Add(nouveau);
                    }
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }

        internal bool SupprimerFermeture(DateTime dateId)
        {
            return SupprimerPeriodeExceptionnelle(dateId, false);
        }
        internal bool SupprimerOuverture(DateTime dateId)
        {
            return SupprimerPeriodeExceptionnelle(dateId, true);
        }
        private bool SupprimerPeriodeExceptionnelle(DateTime dateId, bool ouvert)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jourSelectionne = (from j in db.JourExceptionnel
                                                    where j.DateDebut == dateId && j.Ouvert == ouvert
                                                    select j).FirstOrDefault();

                db.JourExceptionnel.Remove(jourSelectionne);
                if (db.SaveChanges() != 1)
                    return false;
                else
                    return true;
            }
        }

        private JourExceptionnel ProchaineOuvertureExceptionnelle(DateTime date)
        {
            return ProchainePeriodeExceptionnelle(date, true);
        }
        private JourExceptionnel ProchaineFermetureExceptionnelle(DateTime date)
        {
            return ProchainePeriodeExceptionnelle(date, false);
        }
        private JourExceptionnel ProchainePeriodeExceptionnelle(DateTime date, bool ouvert)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                JourExceptionnel jour = (from j in db.JourExceptionnel
                                         where DbFunctions.DiffSeconds(date, j.DateFin) > 0 && j.Ouvert == ouvert
                                         orderby j.DateDebut
                                         select j).FirstOrDefault();
                if (jour == null)
                {
                    jour = new JourExceptionnel
                    {
                        DateDebut = DateTime.MaxValue,
                        DateFin = DateTime.MaxValue,
                        Ouvert = ouvert
                    };
                }
                return jour;
            }
        }

        internal PlageHoraireRetrait ProchainOuvert(DateTime date)
        {
            date = date.AddMinutes(int.Parse(ConfigurationManager.AppSettings["DelaiMinimumAvantRetraitCommande"]));
            bool faireRecherche;
            PlageHoraireRetrait plageHoraireRetrait;
            do
            {
                faireRecherche = false;
                PlageRepas prochainOuvertHabituellement = ProchainOuvertHabituellement(date);
                JourExceptionnel prochainOuvertExceptionnellement = ProchaineOuvertureExceptionnelle(date);
                JourExceptionnel prochainFermeExceptionnellement = ProchaineFermetureExceptionnelle(date);

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

                // Test avec ouverture exceptionnelle
                //
                //cas plage ouverture exceptionnelle complètement avant plage ouverture habituelle
                if (prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First() && prochainOuvertExceptionnellement.DateFin < plageHoraireRetrait.Creneaux.First())
                {
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainOuvertExceptionnellement.DateDebut, prochainOuvertExceptionnellement.DateFin, prochainOuvertHabituellement.Pas);
                }
                // cas ouverture exceptionnelle commence avant plage
                else if (prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First())
                {
                    DateTime fin = prochainOuvertExceptionnellement.DateFin > plageHoraireRetrait.Creneaux.Last() ? prochainOuvertExceptionnellement.DateFin : plageHoraireRetrait.Creneaux.Last();
                    plageHoraireRetrait = new PlageHoraireRetrait(prochainOuvertExceptionnellement.DateDebut, fin, prochainOuvertHabituellement.Pas);
                }
                // cas ouverture exceptionnelle fini après plage
                else if (prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.Last() && prochainOuvertExceptionnellement.DateFin > plageHoraireRetrait.Creneaux.Last())
                {
                    DateTime debut = prochainOuvertExceptionnellement.DateDebut < plageHoraireRetrait.Creneaux.First() ? prochainOuvertExceptionnellement.DateDebut : plageHoraireRetrait.Creneaux.First();
                    plageHoraireRetrait = new PlageHoraireRetrait(debut, prochainOuvertExceptionnellement.DateFin, prochainOuvertHabituellement.Pas);
                }

                //Test avec fermeture exceptionnelle
                //
                // fermeture englobe complètement l'ouverture => recherche à nouveau des plages
                if (prochainFermeExceptionnellement.DateDebut <= plageHoraireRetrait.Creneaux.First() && prochainFermeExceptionnellement.DateFin >= plageHoraireRetrait.Creneaux.Last())
                {
                    faireRecherche = true;
                    date = prochainFermeExceptionnellement.DateFin;
                }
                // fermeture à cheval sur ouverture
                else if (!(prochainFermeExceptionnellement.DateFin <= plageHoraireRetrait.Creneaux.First() || prochainFermeExceptionnellement.DateDebut >= plageHoraireRetrait.Creneaux.Last()))
                {
                    DateTime debut;
                    DateTime fin;
                    if (prochainFermeExceptionnellement.DateFin < plageHoraireRetrait.Creneaux.Last())
                    {
                        debut = prochainFermeExceptionnellement.DateFin;
                        fin = plageHoraireRetrait.Creneaux.Last();
                    }
                    else
                    {
                        debut = plageHoraireRetrait.Creneaux.First();
                        fin = prochainFermeExceptionnellement.DateDebut;
                    }
                    plageHoraireRetrait = new PlageHoraireRetrait(debut, fin, prochainOuvertHabituellement.Pas);
                }
            } while (faireRecherche);
            return plageHoraireRetrait;
        }

        private PlageRepas ProchainOuvertHabituellement(DateTime date)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas plage = new PlageRepas(); // TODO supprimer new
                TimeSpan minuit = new TimeSpan(0, 0, 0);
                int totalSecondes = 24 * 60 * 60 * ((int)date.DayOfWeek - 1) + (int)date.TimeOfDay.TotalSeconds;

                plage = (from c in db.PlageRepas
                         where totalSecondes <= 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin)
                         orderby 24 * 60 * 60 * (c.JourSemaineId - 1) + DbFunctions.DiffSeconds(minuit, c.Fin) // TODO voir as ?
                         select c).FirstOrDefault();

                DateTime maintenant = DateTime.Now;
                if (plage == null)
                {
                    plage = (from c in db.PlageRepas
                             orderby c.JourSemaineId, c.Debut
                             select c).First();
                }
                else if (date.Date == maintenant.Date && plage.JourSemaineId == (int)maintenant.DayOfWeek && plage.Debut < maintenant.TimeOfDay) // TODO BUG ICI
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
    }
}