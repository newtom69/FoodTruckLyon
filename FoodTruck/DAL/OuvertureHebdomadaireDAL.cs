using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        internal PlageRepas AjouterOuverture(int jourId, TimeSpan debut, TimeSpan fin, TimeSpan pas)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas chevauchement = (from p in db.PlageRepas
                                            where p.JourSemaineId == jourId && DbFunctions.DiffMinutes(p.Debut, fin) > 0 && DbFunctions.DiffMinutes(debut, p.Fin) > 0
                                            select p).FirstOrDefault();

                if (chevauchement == null)
                {
                    PlageRepas plage = new PlageRepas
                    {
                        JourSemaineId = jourId,
                        Debut = debut,
                        Fin = fin,
                        Pas = pas
                    };
                    db.PlageRepas.Add(plage);
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }

        internal PlageRepas ModifierOuverture(int id, int jourId, TimeSpan debut, TimeSpan fin, TimeSpan pas)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas plage = (from p in db.PlageRepas
                                    where p.Id == id
                                    select p).FirstOrDefault();

                PlageRepas chevauchement = (from p in db.PlageRepas
                                            where p.Id != id && p.JourSemaineId == jourId && DbFunctions.DiffMinutes(p.Debut, fin) > 0 && DbFunctions.DiffMinutes(debut, p.Fin) > 0
                                            select p).FirstOrDefault();

                if (chevauchement == null && plage != null)
                {
                    plage.JourSemaineId = jourId;
                    plage.Debut = debut;
                    plage.Fin = fin;
                    plage.Pas = pas;
                    db.SaveChanges();
                }
                return chevauchement;
            }
        }

        internal bool SupprimerOuverture(int id)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                PlageRepas plage = (from p in db.PlageRepas
                                    where p.Id == id
                                    select p).FirstOrDefault();
                db.PlageRepas.Remove(plage);
                if (db.SaveChanges() != 1)
                    return false;
                else
                    return true;
            }
        }
    }
}