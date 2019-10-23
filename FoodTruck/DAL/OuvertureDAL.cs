using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL
{
    public class OuvertureDAL
    {
        internal bool EstOuvert(DateTime date, int repasId)
        {
            DayOfWeek jourSemaine = date.DayOfWeek;
            bool? ouvertHabituellement;
            bool? ouvertExceptionnellement;
            bool ouvert;

            ouvertHabituellement = EstOuvertHabituellement((int)jourSemaine, repasId);
            ouvertExceptionnellement = EstOuvertExceptionnellement(date);
            if (ouvertHabituellement != null && (bool)ouvertHabituellement)
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
        /// <param name="jourSemaine">1=lundi</param>
        /// <param name="repasId">1=dejeuner ; 2=diner</param>
        /// <returns></returns>
        private bool? EstOuvertHabituellement(int jourSemaine, int repasId)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Ouverture ouvert = (from ouverture in db.Ouverture
                                    where ouverture.JourSemaine == jourSemaine && ouverture.RepasId == repasId
                                    select ouverture).FirstOrDefault();

                if (ouvert == null)
                    return null;
                else
                    return ouvert.Ouvert;
            }
        }
        private bool? EstOuvertExceptionnellement(DateTime date)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                JourExceptionnel ouvert = (from jour in db.JourExceptionnel
                                           where DbFunctions.DiffDays(jour.DateDebut, date) >= 0 && DbFunctions.DiffDays(date, DbFunctions.AddDays(jour.DateDebut, jour.Duree - 1)) >= 0
                                           select jour).FirstOrDefault();
                if (ouvert == null)
                    return null;
                else
                    return ouvert.Ouvert;
            }
        }
    }
}