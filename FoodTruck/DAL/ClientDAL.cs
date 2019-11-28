using FoodTruck.Models;
using FoodTruck.Outils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodTruck.DAL
{
    class ClientDAL
    {
        public Client Details(int id)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client client = (from c in db.Client
                                 where c.Id == id
                                 select c).FirstOrDefault();
                return client;
            }
        }

        public Client Details(string email)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client client = (from c in db.Client
                                 where c.Email == email
                                 select c).FirstOrDefault();
                return client;
            }
        }

        public bool ExisteEmail(string email)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int clientId = (from c in db.Client
                                where c.Email == email
                                select c.Id).FirstOrDefault();

                return clientId != 0 ? true : false;

            }
        }
        public bool ExisteLogin(string login)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                int clientId = (from c in db.Client
                                where c.Login == login
                                select c.Id).FirstOrDefault();

                return clientId != 0 ? true : false;

            }
        }
        public List<Client> Recherche(string recherche)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                List<Client> clients = (from c in db.Client
                                        where c.Id != 0 && (c.Nom.Contains(recherche) || c.Prenom.Contains(recherche) || c.Email.Contains(recherche) || c.Telephone.Contains(recherche))
                                        select c).ToList();
                return clients;
            }
        }

        public Client Connexion(string loginEmail, string mdp)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                string mdpHash = mdp.GetHash();
                Client client = (from c in db.Client
                                 where (c.Email == loginEmail || c.Login == loginEmail) && c.Mdp == mdpHash
                                 select c).FirstOrDefault();
                return client;
            }
        }
        public Client ConnexionCookies(string guid)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Client client = (from c in db.Client
                                 where c.Guid == guid
                                 select c).FirstOrDefault();
                return client;
            }

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
                var client = (from c in db.Client
                              where c.Id == id
                              select c).FirstOrDefault();
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

        public Client Creation(string email, string login, string mdp, string nom, string prenom, string telephone)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                string mdpHash = mdp.GetHash();
                string guid = Guid.NewGuid().ToString();
                int id = (from c in db.Client
                          where c.Email == email || c.Guid == guid
                          select c.Id).FirstOrDefault();
                if (id == 0)
                {
                    Client client = new Client
                    {
                        Guid = guid,
                        Email = email,
                        Login = login,
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
