using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    public class PlageRepasDAL
    {
        internal List<PlageHoraireRetrait> PlagesHorairesRetrait(DateTime date)
        {
            using (dbEntities db = new dbEntities())
            {
                List<PlageRepas> Creneaux = (from creneau in db.PlageRepas
                                               where creneau.JourSemaineId == (int)date.DayOfWeek
                                               select creneau).ToList();
                List<PlageHoraireRetrait> plagesHorairesRetrait = new List<PlageHoraireRetrait>();
                foreach (PlageRepas creneau in Creneaux)
                {
                    DateTime dateAMJ = new DateTime(date.Year, date.Month, date.Day);
                    PlageHoraireRetrait plage = new PlageHoraireRetrait(dateAMJ + creneau.Debut, dateAMJ + creneau.Fin, creneau.Pas, (TypeRepas)creneau.RepasId);
                    plagesHorairesRetrait.Add(plage);
                }
                return plagesHorairesRetrait;
            }
        }
        internal PlageHoraireRetrait PlageHoraireRetrait(DateTime date)
        {
            using (dbEntities db = new dbEntities())
            {
                OuvertureDAL ouvertureDAL = new OuvertureDAL();
                if (!ouvertureDAL.EstOuvert(date))
                {
                    
                }
                PlageRepas plageRepas = ouvertureDAL.ProchainOuvertHabituellement(date);
                
                DateTime dateAMJ = new DateTime(date.Year, date.Month, date.Day);
                PlageHoraireRetrait plage = new PlageHoraireRetrait(dateAMJ + plageRepas.Debut, dateAMJ + plageRepas.Fin, plageRepas.Pas, (TypeRepas)plageRepas.RepasId);
                JourExceptionnel ouvertExceptionnellement = ouvertureDAL.ProchainOuvertExceptionnellement(date);
                JourExceptionnel fermeExceptionnellement = ouvertureDAL.ProchainFermeExceptionnellement(date);

                return plage;
            }
        }
    }
}