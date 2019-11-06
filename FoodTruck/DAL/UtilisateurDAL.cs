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
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from u in db.Utilisateur
                               where u.Id == id
                               select u).FirstOrDefault();
            }
            return utilisateur;
        }

        public Utilisateur Details(string email)
        {
            Utilisateur utilisateur;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from u in db.Utilisateur
                               where u.Email == email
                               select u).FirstOrDefault();
            }
            return utilisateur;
        }

        public Utilisateur Connexion(string email, string mdp)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);

            Utilisateur utilisateur = new Utilisateur();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from user in db.Utilisateur
                                where user.Email == email && user.Mdp == mdpHash
                                select user).FirstOrDefault();
            }
            return utilisateur;
        }
        public Utilisateur ConnexionCookies(string guid)
        {
            Utilisateur utilisateur = new Utilisateur();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from user in db.Utilisateur
                                where user.Guid == guid
                                select user).FirstOrDefault();
            }
            return utilisateur;
        }

        /// <summary>
        /// Si le solde est suffisant, retire "montant" euros de la cagnotte à l'utilisateur d'id "id" et retourne le solde restant
        /// Si le solde de la cagnotte est insuffisant retourne -1 sans rien modifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="montant"></param>
        /// <returns></returns>
        internal int RetirerCagnotte(int id, int montant)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                var utilisateur = (from user in db.Utilisateur
                                where user.Id == id
                                select user).FirstOrDefault();
                if (montant <= utilisateur.Cagnotte)
                {
                    utilisateur.Cagnotte -= montant;
                    db.SaveChanges();
                    return utilisateur.Cagnotte;
                }
                else
                {
                    return -1;
                }
            }
        }

        public Utilisateur Creation(string email, string mdp, string nom, string prenom, string telephone)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);
            string guid = Guid.NewGuid().ToString();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int id = (from user in db.Utilisateur
                          where user.Email == email || user.Guid == guid
                          select user.Id).FirstOrDefault();
                if (id == 0)
                {
                    Utilisateur utilisateur = new Utilisateur
                    {
                        Guid = guid,
                        Email = email,
                        Mdp = mdpHash,
                        Nom = nom,
                        Prenom = prenom,
                        Telephone = telephone,
                        Cagnotte = 0
                    };
                    db.Utilisateur.Add(utilisateur);
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
            using (foodtruckEntities db = new foodtruckEntities())
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
