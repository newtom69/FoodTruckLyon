using FoodTruck.Models;
using FoodTruck.Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FoodTruck.DAL
{
    class ClientDAL
    {
        public Client Details(int id)
        {
            Client client;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                client = (from c in db.Client
                               where c.Id == id
                               select c).FirstOrDefault();
            }
            return client;
        }

        public Client Details(string email)
        {
            Client client;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                client = (from c in db.Client
                               where c.Email == email
                               select c).FirstOrDefault();
            }
            return client;
        }

        public List<Client> Recherche(string recherche)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Client> utilisateurs = (from c in db.Client
                                                  where c.Id != 0 && (c.Nom.Contains(recherche) || c.Prenom.Contains(recherche) || c.Email.Contains(recherche) || c.Telephone.Contains(recherche))
                                                  select c).ToList();
                return utilisateurs;
            }
        }

        public Client Connexion(string email, string mdp)
        {
            string mdpHash = mdp.GetHash();
            Client client = new Client();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                client = (from user in db.Client
                               where user.Email == email && user.Mdp == mdpHash
                               select user).FirstOrDefault();
            }
            return client;
        }
        public Client ConnexionCookies(string guid)
        {
            Client client = new Client();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                client = (from user in db.Client
                               where user.Guid == guid
                               select user).FirstOrDefault();
            }
            return client;
        }

        /// <summary>
        /// Si le solde est suffisant, retire "montant" euros de la cagnotte de client d'id "id" et retourne le solde restant
        /// Si le solde de la cagnotte est insuffisant retourne -1 sans rien modifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="montant"></param>
        /// <returns></returns>
        internal int RetirerCagnotte(int id, int montant)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                var client = (from user in db.Client
                                   where user.Id == id
                                   select user).FirstOrDefault();
                if (montant <= client.Cagnotte)
                {
                    client.Cagnotte -= montant;
                    db.SaveChanges();
                    return client.Cagnotte;
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
                    Client client = new Client
                    {
                        Guid = guid,
                        Email = email,
                        Mdp = mdpHash,
                        Nom = nom,
                        Prenom = prenom,
                        Telephone = telephone,
                        Cagnotte = 0,
                        Inscription = DateTime.Today
                    };
                    db.Client.Add(client);
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
                Client client = (from user in db.Client
                                           where user.Id == id
                                           select user).FirstOrDefault();

                client.Mdp = mdp.GetHash();
                if (email != null)
                    client.Email = email;
                if (nom != null)
                    client.Nom = nom;
                if (prenom != null)
                    client.Prenom = prenom;
                if (telephone != null)
                    client.Telephone = telephone;
                return db.SaveChanges();
            }
        }

        internal int DonnerDroitAdmin(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client client = (from user in db.Client
                                           where user.Id == id
                                           select user).FirstOrDefault();

                client.AdminArticle = client.AdminCommande = client.AdminPlanning = true;
                return db.SaveChanges();
            }
        }
    }
}
