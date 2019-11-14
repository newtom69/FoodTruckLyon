using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL
{
    public class AdminTemporaireDAL
    {
        internal AdminTemporaire Details(string identifiant)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                DateTime maintenant = DateTime.Now;
                var adminTemporaire = (from u in db.AdminTemporaire
                                       where u.CodeVerification == identifiant && DbFunctions.DiffMinutes(maintenant, u.DateFinValidite) >= 0
                                       select u).FirstOrDefault();
                return adminTemporaire;
            }
        }
        internal void Ajouter(Utilisateur utilisateur, string codeVerification, DateTime dateFinValidite)
        {

            Supprimer(utilisateur.Email);
            using (foodtruckEntities db = new foodtruckEntities())
            {
                AdminTemporaire adminTemporaire = new AdminTemporaire
                {
                    Email = utilisateur.Email,
                    Nom = utilisateur.Nom,
                    Prenom = utilisateur.Prenom,
                    CodeVerification = codeVerification,
                    DateFinValidite = dateFinValidite
                };
                db.AdminTemporaire.Add(adminTemporaire);
                db.SaveChanges();
            }
        }

        internal AdminTemporaire Verifier(string identifiant)
        {
            AdminTemporaire adminTemporaire = Details(identifiant);
            if (adminTemporaire != null)
            {
                Supprimer(adminTemporaire.Email);
            }
            return adminTemporaire;
        }
        internal void Supprimer(string email)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<AdminTemporaire> listeAdminTemporaire =
                    (from a in db.AdminTemporaire
                     where a.Email == email
                     select a).ToList();
                db.AdminTemporaire.RemoveRange(listeAdminTemporaire);
                db.SaveChanges();
            }
        }
    }
}