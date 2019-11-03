using FoodTruck.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FoodTruck.DAL
{
    class UtilisateurDAL
    {
        public Utilisateur Details(int id)
        {
            Utilisateur utilisateur;
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                utilisateur = (from u in db.Utilisateur
                               where u.Id == id
                               select u).FirstOrDefault();
            }
            return utilisateur;
        }

        public Utilisateur Connexion(string email, string mdp)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);

            Utilisateur lUtilisateur = new Utilisateur();
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                lUtilisateur = (from user in db.Utilisateur
                                where user.Email == email && user.Mdp == mdpHash
                                select user).FirstOrDefault();
            }
            return lUtilisateur;
        }
        public Utilisateur ConnexionCookies(string guid)
        {
            Utilisateur lUtilisateur = new Utilisateur();
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                lUtilisateur = (from user in db.Utilisateur
                                where user.Guid == guid
                                select user).FirstOrDefault();
            }
            return lUtilisateur;
        }

        internal int RetirerPointsFidelite(int id, int points)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                var utilisateur = (from user in db.Utilisateur
                                where user.Id == id
                                select user).FirstOrDefault();

                utilisateur.Points -= points;
                return db.SaveChanges();
            }
        }

        public Utilisateur Creation(string email, string mdp, string nom, string prenom, string telephone)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);
            string guid = Guid.NewGuid().ToString();
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                int id = (from user in db.Utilisateur
                          where user.Email == email || user.Guid == guid
                          select user.Id).FirstOrDefault();
                if (id == 0)
                {
                    Utilisateur lUtilisateur = new Utilisateur
                    {
                        Guid = guid,
                        Email = email,
                        Mdp = mdpHash,
                        Nom = nom,
                        Prenom = prenom,
                        Telephone = telephone,
                        Points = 0
                    };
                    db.Utilisateur.Add(lUtilisateur);
                    db.SaveChanges();
                    return Connexion(email, mdp);
                }
                else
                {
                    return null;
                }
            }
        }

        internal int Modification(int id, string email, string mdp, string nom, string prenom, string telephone)
        {
            using (FoodTruckEntities db = new FoodTruckEntities())
            {
                Utilisateur utilisateur = (from user in db.Utilisateur
                                           where user.Id == id
                                           select user).FirstOrDefault();

                using (SHA256 Hash = SHA256.Create())
                    utilisateur.Mdp = GetHash(Hash, mdp);
                utilisateur.Nom = nom;
                utilisateur.Prenom = prenom;
                utilisateur.Email = email;
                utilisateur.Telephone = telephone;
                return db.SaveChanges();
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
