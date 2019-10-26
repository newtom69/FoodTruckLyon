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
            using (dbEntities db = new dbEntities())
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
            using (dbEntities db = new dbEntities())
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
            using (dbEntities db = new dbEntities())
            {
                lUtilisateur = (from user in db.Utilisateur
                                where user.Guid == guid
                                select user).FirstOrDefault();
            }
            return lUtilisateur;
        }
        public Utilisateur Creation(string email, string mdp, string nom, string prenom, string telephone)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);
            string guid = Guid.NewGuid().ToString();
            using (dbEntities db = new dbEntities())
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
                        Telephone = telephone
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
