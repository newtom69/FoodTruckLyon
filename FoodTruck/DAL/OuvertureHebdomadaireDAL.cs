using FoodTruck.Models;
using OmniFW.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OuvertureHebdomadaireDAL
    {
        internal List<OuvertureHebdomadaire> OuverturesHebdomadaires()
        {
            CollectionEntite<OuvertureHebdomadaire> ouverturesHebdomadaire = new CollectionEntite<OuvertureHebdomadaire>();
            ouverturesHebdomadaire.AddToCache = false;
            ouverturesHebdomadaire.Rechercher();
            List<OuvertureHebdomadaire> listeOuverturesHebdomadaire = ouverturesHebdomadaire.Liste.OrderBy(o => o.JourSemaineId).ThenBy(o => o.Debut).ToList();
            return listeOuverturesHebdomadaire;
        }

        internal OuvertureHebdomadaire AjouterOuverture(int jourId, TimeSpan debut, TimeSpan fin)
        {
            CollectionEntite<OuvertureHebdomadaire> ouverturesHebdomadaire = new CollectionEntite<OuvertureHebdomadaire>();
            OmniFW.Data.Critere crit = new OmniFW.Data.Critere();
            crit.Parametres.Add(new OmniFW.Data.ParametreSQL("JourSemaineId", jourId, System.Data.DbType.Int32));
            ouverturesHebdomadaire.Rechercher(crit);
            OuvertureHebdomadaire chevauchement = ouverturesHebdomadaire.Liste.Find(c => fin - c.Debut > TimeSpan.Zero && c.Fin - debut > TimeSpan.Zero);
            if (chevauchement == null)
            {
                OuvertureHebdomadaire ouverture = new OuvertureHebdomadaire
                {
                    JourSemaineId = jourId,
                    Debut = debut,
                    Fin = fin,
                };
                ouverture.Enregistrer(); //TODO faire ce qu'il faut pour conversion time

            }
            return chevauchement;
        }

        internal OuvertureHebdomadaire ModifierOuverture(int id, int jourId, TimeSpan debut, TimeSpan fin)
        {
            //    OuvertureHebdomadaire ouverture = (from p in db.OuvertureHebdomadaire
            //                                       where p.Id == id
            //                                       select p).FirstOrDefault();

            //    OuvertureHebdomadaire chevauchement = (from p in db.OuvertureHebdomadaire
            //                                           where p.Id != id && p.JourSemaineId == jourId && DbFunctions.DiffMinutes(p.Debut, fin) > 0 && DbFunctions.DiffMinutes(debut, p.Fin) > 0
            //                                           select p).FirstOrDefault();

            //    if (chevauchement == null && ouverture != null)
            //    {
            //        ouverture.JourSemaineId = jourId;
            //        ouverture.Debut = debut;
            //        ouverture.Fin = fin;
            //        db.SaveChanges();
            //    }
            //    return chevauchement;
            throw new NotImplementedException();
        }

        internal bool SupprimerOuverture(int id)
        {
            OuvertureHebdomadaire ouverture = new OuvertureHebdomadaire(id);
            bool retour = ouverture.Supprimer();
            return retour;
        }
    }
}