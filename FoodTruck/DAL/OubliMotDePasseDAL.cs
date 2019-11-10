using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class OubliMotDePasseDAL
    {
        internal UtilisateurOubliMotDePasse Details(int utilisateurId, string guid)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                DateTime maintenant = DateTime.Now;
                var oubliMotDePasse = (from u in db.UtilisateurOubliMotDePasse
                                       where u.UtilisateurId == utilisateurId && u.Guid == guid && DbFunctions.DiffMinutes(maintenant, u.DateFinValidite) >= 0
                                       select u).FirstOrDefault();
                return oubliMotDePasse;
            }
        }
        internal void Ajouter(int utilisateurId, string guid, DateTime dateFinValidite)
        {
            Supprimer(utilisateurId);
            using (foodtruckEntities db = new foodtruckEntities())
            {
                UtilisateurOubliMotDePasse oubliMotDePasse = new UtilisateurOubliMotDePasse
                {
                    UtilisateurId = utilisateurId,
                    Guid = guid,
                    DateFinValidite = dateFinValidite
                };
                db.UtilisateurOubliMotDePasse.Add(oubliMotDePasse);
                db.SaveChanges();
            }
        }

        internal void Supprimer(int utilisateurId)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<UtilisateurOubliMotDePasse> listeOubliMotDePasse =
                    (from u in db.UtilisateurOubliMotDePasse
                     where u.UtilisateurId == utilisateurId
                     select u).ToList();
                db.UtilisateurOubliMotDePasse.RemoveRange(listeOubliMotDePasse);
                db.SaveChanges();
            }
        }

        internal bool Verifier(int utilisateurId, string guid)
        {
            UtilisateurOubliMotDePasse oubliMotDePasse = Details(utilisateurId, guid);
            if (oubliMotDePasse != null)
            {
                Supprimer(utilisateurId);
                return true;
            }
            else
            {
                return false;
            }
        }
        internal int Purger()
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                IQueryable<UtilisateurOubliMotDePasse> listeOubliMotDePasse = from u in db.UtilisateurOubliMotDePasse
                                                                              where DbFunctions.DiffMinutes(u.DateFinValidite, DateTime.Now) > 0
                                                                              select u;

                db.UtilisateurOubliMotDePasse.RemoveRange(listeOubliMotDePasse);
                return db.SaveChanges();
            }
        }
    }
}
