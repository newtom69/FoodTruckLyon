using FoodTruck.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FoodTruck.DAL
{
    class UtilisateurDAL
    {
        public Client Details(int id)
        {
            Client utilisateur;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from u in db.Client
                               where u.Id == id
                               select u).FirstOrDefault();
            }
            return utilisateur;
        }

        public Client Details(string email)
        {
            Client utilisateur;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from u in db.Client
                               where u.Email == email
                               select u).FirstOrDefault();
            }
            return utilisateur;
        }

        public List<Client> Recherche(string recherche)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Client> utilisateurs = (from u in db.Client
                                                  where u.Id != 0 && (u.Nom.Contains(recherche) || u.Prenom.Contains(recherche) || u.Email.Contains(recherche) || u.Telephone.Contains(recherche))
                                                  select u).ToList();
                return utilisateurs;
            }
        }

        public Client Connexion(string email, string mdp)
        {
            string mdpHash = mdp.GetHash();
            Client utilisateur = new Client();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from user in db.Client
                               where user.Email == email && user.Mdp == mdpHash
                               select user).FirstOrDefault();
            }
            return utilisateur;
        }
        public Client ConnexionCookies(string guid)
        {
            Client utilisateur = new Client();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                utilisateur = (from user in db.Client
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
                var utilisateur = (from user in db.Client
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

        public Client Creation(string email, string mdp, string nom, string prenom, string telephone)
        {
            string mdpHash = mdp.GetHash();
            string guid = Guid.NewGuid().ToString();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int id = (from user in db.Client
                          where user.Email == email || user.Guid == guid
                          select user.Id).FirstOrDefault();
                if (id == 0)
                {
                    Client utilisateur = new Client
                    {
                        Guid = guid,
                        Email = email,
                        Mdp = mdpHash,
                        Nom = nom,
                        Prenom = prenom,
                        Telephone = telephone,
                        Cagnotte = 0
                    };
                    db.Client.Add(utilisateur);
                    db.SaveChanges();
                    return Connexion(email, mdp);
                }
                else
                {
                    return null;
                }
            }
        }

        internal int Modification(int id, string mdp, string email = null, string nom = null, string prenom = null, string telephone = null)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client utilisateur = (from user in db.Client
                                           where user.Id == id
                                           select user).FirstOrDefault();

                utilisateur.Mdp = mdp.GetHash();
                if (email != null)
                    utilisateur.Email = email;
                if (nom != null)
                    utilisateur.Nom = nom;
                if (prenom != null)
                    utilisateur.Prenom = prenom;
                if (telephone != null)
                    utilisateur.Telephone = telephone;
                return db.SaveChanges();
            }
        }

        internal int DonnerDroitAdmin(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client utilisateur = (from user in db.Client
                                           where user.Id == id
                                           select user).FirstOrDefault();

                utilisateur.AdminArticle = utilisateur.AdminCommande = utilisateur.AdminPlanning = true;
                return db.SaveChanges();
            }
        }
    }
}
