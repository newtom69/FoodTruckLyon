using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodTruck.Models
{
    public class CreneauRepasDAL
    {
        internal List<PlageHoraireRetrait> PlagesHorairesRetrait(DateTime date)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int jourSemaineId = (int)date.DayOfWeek;
                List<CreneauRepas> Creneaux = (from creneau in db.CreneauRepas
                                               where creneau.JourSemaineId == jourSemaineId
                                               select creneau).ToList();
                List<PlageHoraireRetrait> plagesHorairesRetrait = new List<PlageHoraireRetrait>();
                foreach (CreneauRepas creneau in Creneaux)
                {
                    var a = creneau.Debut;
                    var b = creneau.Fin;
                    DateTime dateJourDate = new DateTime(date.Year, date.Month, date.Day);
                    PlageHoraireRetrait plage = new PlageHoraireRetrait(dateJourDate + creneau.Debut, dateJourDate + creneau.Fin, creneau.Pas, (TypeRepas)creneau.RepasId);
                    plagesHorairesRetrait.Add(plage);
                }
                return plagesHorairesRetrait;
            }
        }
    }
}