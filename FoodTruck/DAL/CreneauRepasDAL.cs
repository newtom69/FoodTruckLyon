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
            using (dbEntities db = new dbEntities())
            {
                List<CreneauRepas> Creneaux = (from creneau in db.CreneauRepas
                                               where creneau.JourSemaineId == (int)date.DayOfWeek
                                               select creneau).ToList();
                List<PlageHoraireRetrait> plagesHorairesRetrait = new List<PlageHoraireRetrait>();
                foreach (CreneauRepas creneau in Creneaux)
                {
                    DateTime dateAMJ = new DateTime(date.Year, date.Month, date.Day);
                    PlageHoraireRetrait plage = new PlageHoraireRetrait(dateAMJ + creneau.Debut, dateAMJ + creneau.Fin, creneau.Pas, (TypeRepas)creneau.RepasId);
                    plagesHorairesRetrait.Add(plage);
                }
                return plagesHorairesRetrait;
            }
        }
    }
}