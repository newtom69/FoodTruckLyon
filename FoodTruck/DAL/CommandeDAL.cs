using FoodTruck.Models;
using FoodTruck.Outils;
using FoodTruck.ViewModels;
using OmniFW.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace FoodTruck.DAL
{
    class CommandeDAL
    {
        internal Commande Detail(int id)
        {
            Commande commande = new Commande(id);
            commande.Lire();
            return commande;
        }

        public void Ajouter(Commande commande, List<ArticleViewModel> articlesVM)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    db.Commande.Add(commande);
            //    db.SaveChanges();
            //    int commandeId = (from cmd in db.Commande
            //                      where cmd.ClientId == commande.ClientId
            //                      orderby cmd.Id descending
            //                      select cmd.Id).FirstOrDefault();
            //    double remiseTotalCommandeHT = 0;
            //    double totalCommandeHT = 0;
            //    foreach (var articleVM in articlesVM)
            //    {
            //        float tauxTva = new TvaDAL().TauxArticle(articleVM.Article.Id);
            //        int quantite = articleVM.Quantite;
            //        double prixTotalTTC = articleVM.Article.PrixTTC * quantite;
            //        double prixTotalHT = prixTotalTTC * 100 / (100 + tauxTva);
            //        double fractionPrixArticle = prixTotalTTC / (commande.PrixTotalTTC + commande.RemiseCommerciale + commande.RemiseFidelite);
            //        double remiseArticleTTC = fractionPrixArticle * (commande.RemiseCommerciale + commande.RemiseFidelite);
            //        double remiseArticleHT = remiseArticleTTC * prixTotalHT / prixTotalTTC;
            //        remiseTotalCommandeHT += remiseArticleHT;
            //        totalCommandeHT += prixTotalHT;
            //        Commande_Article cmdArt = new Commande_Article
            //        {
            //            CommandeId = commandeId,
            //            ArticleId = articleVM.Article.Id,
            //            Quantite = quantite,
            //            PrixTotalTTC = Math.Round(prixTotalTTC, 2),
            //            PrixTotalHT = Math.Round(prixTotalHT, 2)
            //        };
            //        db.Commande_Article.Add(cmdArt);
            //    }
            //    commande.PrixTotalHT = Math.Round(totalCommandeHT - remiseTotalCommandeHT, 2);
            //    db.SaveChanges();
            //}
            throw new NotImplementedException();
        }
        internal void Annuler(int id)
        {
            MettreAJourStatut(id, false, true);
        }
        internal void Retirer(int id)
        {
            MettreAJourStatut(id, true, false);
        }

        private void MettreAJourStatut(int id, bool retrait, bool annule)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    Commande commande = (from cmd in db.Commande
            //                         where cmd.Id == id
            //                         select cmd).FirstOrDefault();
            //    if (commande != null)
            //    {
            //        commande.Annulation = annule;
            //        commande.Retrait = retrait;
            //        Client client = (from u in db.Client
            //                         where u.Id == commande.ClientId
            //                         select u).FirstOrDefault();
            //        if (commande.Retrait)
            //            client.Cagnotte += (int)commande.PrixTotalTTC / 10;
            //        if (commande.Annulation)
            //            client.Cagnotte += (int)commande.RemiseFidelite;
            //        db.SaveChanges();
            //    }
            //}
            throw new NotImplementedException();
        }

        internal List<Commande> CommandesAStatuer()
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    DateTime now = DateTime.Now;
            //    const int intervalleMax = 1;
            //    var commandes = (from cmd in db.Commande
            //                     where !cmd.Retrait && !cmd.Annulation && DbFunctions.DiffHours(cmd.DateRetrait, now) >= intervalleMax
            //                     orderby cmd.DateRetrait
            //                     select cmd).ToList();
            //    return commandes;
            //}
            throw new NotImplementedException();
        }

        public List<Commande> CommandesEnCours(int fourchetteHeures)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    var commandes = (from cmd in db.Commande
            //                     where !cmd.Retrait && !cmd.Annulation && Math.Abs((int)DbFunctions.DiffHours(DateTime.Now, cmd.DateRetrait)) < Math.Abs(fourchetteHeures)
            //                     orderby cmd.DateRetrait
            //                     select cmd).ToList();
            //    return commandes;
            //}
            throw new NotImplementedException();
        }

        internal List<Commande> CommandesClient(int id, int max = int.MaxValue)
        {
            OmniFW.Business.CollectionEntite<Commande> commandes = new OmniFW.Business.CollectionEntite<Commande>();
            OmniFW.Data.Critere critereClient = new OmniFW.Data.Critere();
            critereClient.Parametres.Add(new OmniFW.Data.ParametreSQL("ClientId", id, System.Data.DbType.Int32));
            commandes.Rechercher(critereClient);
            List<Commande> listCommandes = commandes.Liste.OrderBy(cmd => cmd.Annulation).ThenBy(cmd => cmd.Retrait).ThenBy(cmd => Math.Abs((cmd.DateRetrait - DateTime.Now).TotalMinutes)).Take(max).ToList();
            return listCommandes;
        }

        internal double RemiseTotaleClient(int id)
        {
            double remise = 0;
            List<Commande> commandes = CommandesClient(id);
            foreach (Commande commande in commandes)
            {
                remise += commande.RemiseCommerciale + commande.RemiseFidelite;
            }
            remise = Math.Round(remise, 2);
            return remise;
        }

        internal int NombreCommandes(DateTime date)
        {
            OmniFW.Business.CollectionEntite<Commande> commandes = new OmniFW.Business.CollectionEntite<Commande>();
            Critere critereAnnulation = new Critere();
            critereAnnulation.Parametres.Add(new ParametreSQL("Annulation", false, DbType.Boolean));
            Critere critereRetrait = new Critere();
            critereRetrait.Parametres.Add(new ParametreSQL("Retrait", false, DbType.Boolean));
            Critere critereDateRetrait = new Critere();
            critereDateRetrait.Parametres.Add(new ParametreSQL("DateRetrait", date, DbType.DateTime));
            commandes.Rechercher(critereAnnulation, critereRetrait, critereDateRetrait);
            return commandes.Liste.Count;
        }

        internal List<Commande> CommandesEnCoursClient(int id)
        {
            DateTime now = DateTime.Now;
            OmniFW.Business.CollectionEntite<Commande> commandes = new OmniFW.Business.CollectionEntite<Commande>();
            Critere critereClient = new Critere();
            critereClient.Parametres.Add(new ParametreSQL("ClientId", id, DbType.Int32));

            Critere critereAnnulation = new Critere();
            critereAnnulation.Parametres.Add(new ParametreSQL("Annulation", false, DbType.Boolean));

            Critere critereRetrait = new Critere();
            critereRetrait.Parametres.Add(new ParametreSQL("Retrait", false, DbType.Boolean));

            commandes.Rechercher(critereClient, critereAnnulation, critereRetrait);
            List<Commande> listCommandes = commandes.Liste.FindAll(cmd => (cmd.DateRetrait - now).TotalHours > -1);
            listCommandes = listCommandes.OrderBy(cmd => Math.Abs((cmd.DateRetrait - now).TotalMinutes)).ToList();
            return listCommandes;
        }

        internal List<Commande> CommandesRecherche(string recherche = "", DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            DateTime debut = dateDebut ?? DateTime.MinValue;
            DateTime fin = dateFin ?? DateTime.MaxValue;
            debut = debut.Date;
            fin = fin.Date;

            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Commande> commandes =
            //        (from cmd in db.Commande
            //         join u in db.Client on cmd.ClientId equals u.Id
            //         where DbFunctions.DiffDays(debut, cmd.DateRetrait) >= 0 && DbFunctions.DiffDays(cmd.DateRetrait, fin) >= 0 &&
            //         (cmd.Id.ToString().Contains(recherche) || u.Nom.Contains(recherche) || u.Prenom.Contains(recherche) || u.Email.Contains(recherche))
            //         orderby cmd.Id descending
            //         select cmd).ToList();

            //    return commandes;
            //}
            throw new NotImplementedException();
        }

        internal List<Commande> CommandesFutures()
        {
            DateTime maintenant = DateTime.Now;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Commande> commandes = (from cmd in db.Commande
            //                                where DbFunctions.DiffMinutes(maintenant, cmd.DateRetrait) >= 0 && !cmd.Retrait && !cmd.Annulation
            //                                orderby cmd.DateRetrait
            //                                select cmd).ToList();
            //    return commandes;
            //}
            throw new NotImplementedException();
        }
        internal List<Commande> CommandesPendantFermetures()
        {
            List<Commande> commandesPendantFermetures = CommandesPendantFermeturesExceptionnelles();
            commandesPendantFermetures.AddRange(CommandesPendantFermeturesHabituelles());
            List<Commande> commandesPendantOuverturesExceptionnelles = CommandesPendantOuverturesExceptionnelles();
            return commandesPendantFermetures.Except(commandesPendantOuverturesExceptionnelles, new CommandeEqualityComparer()).ToList(); ;
        }

        private List<Commande> CommandesPendantFermeturesExceptionnelles()
        {
            DateTime maintenant = DateTime.Now;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Commande> commandes = (from cmd in db.Commande
            //                                where DbFunctions.DiffMinutes(maintenant, cmd.DateRetrait) >= 0 && !cmd.Retrait && !cmd.Annulation
            //                                orderby cmd.DateRetrait
            //                                select cmd).ToList();

            //    List<JourExceptionnel> fermetures = (from j in db.JourExceptionnel
            //                                         where DbFunctions.DiffMinutes(maintenant, j.DateFin) >= 0 && !j.Ouvert
            //                                         select j).ToList();

            //    List<Commande> commandesDansFermeture = new List<Commande>();
            //    foreach (JourExceptionnel fermeture in fermetures)
            //    {
            //        commandesDansFermeture.AddRange(commandes.FindAll(c => fermeture.DateDebut <= c.DateRetrait && c.DateRetrait <= fermeture.DateFin));
            //    }
            //    return commandesDansFermeture;
            //}
            throw new NotImplementedException();
        }

        private List<Commande> CommandesPendantOuverturesExceptionnelles()
        {
            DateTime maintenant = DateTime.Now;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Commande> commandes = (from cmd in db.Commande
            //                                where DbFunctions.DiffMinutes(maintenant, cmd.DateRetrait) >= 0 && !cmd.Retrait && !cmd.Annulation
            //                                orderby cmd.DateRetrait
            //                                select cmd).ToList();

            //    List<JourExceptionnel> ouvertures = (from j in db.JourExceptionnel
            //                                         where DbFunctions.DiffMinutes(maintenant, j.DateFin) >= 0 && j.Ouvert
            //                                         select j).ToList();

            //    List<Commande> commandesDansOuvertures = new List<Commande>();
            //    foreach (JourExceptionnel fermeture in ouvertures)
            //    {
            //        commandesDansOuvertures.AddRange(commandes.FindAll(c => fermeture.DateDebut <= c.DateRetrait && c.DateRetrait <= fermeture.DateFin));
            //    }
            //    return commandesDansOuvertures;
            //}
            throw new NotImplementedException();
        }

        private List<Commande> CommandesPendantFermeturesHabituelles()
        {
            DateTime maintenant = DateTime.Now;
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    List<Commande> commandes = (from cmd in db.Commande
            //                                where DbFunctions.DiffMinutes(maintenant, cmd.DateRetrait) >= 0 && !cmd.Retrait && !cmd.Annulation
            //                                orderby cmd.DateRetrait
            //                                select cmd).ToList();

            //    List<OuvertureHebdomadaire> ouvertures = (from o in db.OuvertureHebdomadaire
            //                                              select o).ToList();

            //    List<Commande> commandesDansOuvertures = new List<Commande>();
            //    foreach (OuvertureHebdomadaire ouverture in ouvertures)
            //    {
            //        commandesDansOuvertures.AddRange(commandes.FindAll(c => (int)c.DateRetrait.DayOfWeek == ouverture.JourSemaineId && ouverture.Debut <= c.DateRetrait.TimeOfDay && c.DateRetrait.TimeOfDay <= ouverture.Fin));
            //    }
            //    List<Commande> commandesDansFermeture = commandes.Except(commandesDansOuvertures).ToList();
            //    return commandesDansFermeture;
            //}
            throw new NotImplementedException();
        }

        public List<ArticleViewModel> Articles(int commandeId)
        {
            //using (foodtruckEntities db = new foodtruckEntities())
            //{
            //    var donnees = (from cmd in db.Commande
            //                   join ca in db.Commande_Article on cmd.Id equals ca.CommandeId
            //                   join article in db.Article on ca.ArticleId equals article.Id
            //                   where cmd.Id == commandeId
            //                   orderby article.FamilleId, article.Nom
            //                   select new { article, ca.Quantite, PrixTTC = Math.Round(ca.PrixTotalTTC/ ca.Quantite,2), PrixHT = Math.Round(ca.PrixTotalHT / ca.Quantite,2) }).ToList();

            //    List<ArticleViewModel> listArticles = new List<ArticleViewModel>();
            //    foreach (var donnee in donnees)
            //    {
            //        donnee.article.PrixTTC = donnee.PrixTTC;
            //        donnee.article.PrixHT = donnee.PrixHT;
            //        listArticles.Add(new ArticleViewModel(donnee.article, donnee.Quantite));
            //    }
            //    return listArticles;
            //}
            //TODO OMNIFW
            return new List<ArticleViewModel>();
        }
    }
}
