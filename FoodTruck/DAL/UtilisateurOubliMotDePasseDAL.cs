using System;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    public class UtilisateurOubliMotDePasseDAL
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
                var listOubliMotDePasse = (from u in db.UtilisateurOubliMotDePasse
                                           where u.UtilisateurId == utilisateurId
                                           select u).ToList();
                db.UtilisateurOubliMotDePasse.RemoveRange(listOubliMotDePasse);
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
    }
}
