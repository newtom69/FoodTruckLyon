﻿using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL
{
    public class OuvertureHebdomadaireDAL
    {
        internal List<PlageRepas> ListerOuverturesHebdomadaires()
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                List<PlageRepas> jours = (from p in db.PlageRepas
                                                orderby p.JourSemaineId, p.Debut
                                                select p).ToList();
                return jours;
            }
        }

        internal JourExceptionnel AjouterOuverture(int jourId, TimeSpan debut, TimeSpan fin)
        {
            throw new NotImplementedException(); // TODO
        }

        internal JourExceptionnel ModifierOuverture(int jourId, TimeSpan debut, TimeSpan fin)
        {
            throw new NotImplementedException(); // TODO
        }

        internal bool SupprimerOuverture(int jourId)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}