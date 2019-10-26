﻿using FoodTruck.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OuvertureDAL
    {
        internal bool EstOuvert(PlageHoraireRetrait plageHoraireRetrait)
        {
            TypeRepas typeRepas = plageHoraireRetrait.RepasId;
            DayOfWeek jourSemaine = plageHoraireRetrait.PremierCreneau.DayOfWeek;
            bool ouvertHabituellement;
            bool? ouvertExceptionnellement;
            bool ouvert;
            ouvertHabituellement = EstOuvertHabituellement(jourSemaine, typeRepas);
            ouvertExceptionnellement = EstOuvertExceptionnellement(plageHoraireRetrait.PremierCreneau, typeRepas); // faire sans typeRepas (redondant)
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

        /// <summary>
        /// Retourne True si le foodtruck est ouvert
        /// </summary>
        /// <param name="jourSemaine"></param>
        /// <param name="repasId"></param>
        /// <returns></returns>
        private bool EstOuvertHabituellement(DayOfWeek jourSemaine, TypeRepas typeRepas)
        {
            using (dbEntities db = new dbEntities())
            {
                CreneauRepas ouvert = (from cr in db.CreneauRepas
                                       where cr.JourSemaineId == (int)jourSemaine && cr.RepasId == (int)typeRepas
                                       select cr).FirstOrDefault();
                if (ouvert != null)
                    return true;
                else
                    return false;
            }
        }
        private bool? EstOuvertExceptionnellement(DateTime date, TypeRepas typeRepas) // redondance typeRepas
        {
            int repasId = (int)typeRepas;
            using (dbEntities db = new dbEntities())
            {
                JourExceptionnel ouvert =
                    (from jexc in db.JourExceptionnel
                     where jexc.Jour == 0 && DbFunctions.DiffDays(jexc.DateDebut, date) == 0 && jexc.DebutRepasId <= repasId && jexc.FinRepasId >= repasId
                        || jexc.Jour > 0 && DbFunctions.DiffDays(jexc.DateDebut, date) == 0 && jexc.DebutRepasId <= repasId
                        || jexc.Jour > 0 && DbFunctions.DiffDays(date, DbFunctions.AddDays(jexc.DateDebut, jexc.Jour)) == 0 && jexc.FinRepasId >= repasId
                        || jexc.Jour > 0 && DbFunctions.DiffDays(jexc.DateDebut, date) > 0 && DbFunctions.DiffDays(date, DbFunctions.AddDays(jexc.DateDebut, jexc.Jour)) > 0
                     select jexc).FirstOrDefault();
                if (ouvert == null)
                    return null;
                else
                    return ouvert.Ouvert;
            }
        }
    }
}