using FoodTruck.DAL;
using FoodTruck.Extensions;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrationController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CommandesEnCours()
        {
            AdministrationViewModel administrationViewModel = null;
            if (VariablesSession.AdminSuper || VariablesSession.AdminCommande)
            {
                //lister les commandes en cours
                CommandeDAL commandeDAL = new CommandeDAL();
                List<Commande> commandesEnCours = commandeDAL.ListerCommandesEnCours();
                administrationViewModel = new AdministrationViewModel(commandesEnCours);
            }
            return View(administrationViewModel);
        }

        [HttpPost]
        public ActionResult CommandesEnCours(int id, string statut)
        {
            bool retire = false;
            bool annule = false;
            if (statut == "retire")
                retire = true;
            else if (statut == "annule")
                annule = true;
            new CommandeDAL().MettreAJourStatut(id, retire, annule);
            return RedirectToAction(RouteData.Values["action"].ToString());
        }

        [HttpGet]
        public ActionResult CommandesAMarquer()
        {
            AdministrationViewModel administrationViewModel = null;
            if (VariablesSession.AdminSuper || VariablesSession.AdminCommande)
            {
                CommandeDAL commandeDAL = new CommandeDAL();
                var commandes = commandeDAL.ListerCommandesAMarquer();
                administrationViewModel = new AdministrationViewModel(commandes);
            }
            return View(administrationViewModel);
        }

        [HttpPost]
        public ActionResult CommandesAMarquer(int id, string statut)
        {
            bool retire = false;
            bool annule = false;
            if (statut == "retire")
                retire = true;
            else if (statut == "annule")
                annule = true;
            new CommandeDAL().MettreAJourStatut(id, retire, annule);
            return RedirectToAction(RouteData.Values["action"].ToString());
        }

        [HttpGet]
        public ActionResult Commandes()
        {
            AdministrationViewModel administrationViewModel = null;
            if (VariablesSession.AdminSuper || VariablesSession.AdminCommande)
            {
                CommandeDAL commandeDAL = new CommandeDAL();
                var commandes = commandeDAL.ListerCommandesToutes();
                administrationViewModel = new AdministrationViewModel(commandes);
            }
            return View(administrationViewModel);
        }

        [HttpGet]
        public ActionResult AjouterArticle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AjouterArticle(string nom, string description, string prix, int? grammage, int? litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            string nomOk = nom.NomAdmis();
            double prixOk = Math.Abs(Math.Round(float.Parse(prix, CultureInfo.InvariantCulture.NumberFormat), 2));
            int grammageOk = Math.Abs(grammage ?? 0);
            int litrageOk = Math.Abs(litrage ?? 0);
            string descriptionOk = description;
            string allergenesOk = allergenes ?? "";
            int familleIdOk = familleId;
            bool dansCarteOk = dansCarte;
            if (VariablesSession.AdminSuper || VariablesSession.AdminArticle)
            {
                Article lArticle = new Article
                {
                    Nom = nomOk,
                    Description = descriptionOk,
                    Prix = prixOk,
                    Grammage = grammageOk,
                    Litrage = litrageOk,
                    Allergenes = allergenesOk,
                    FamilleId = familleIdOk,
                    DansCarte = dansCarteOk,
                };
                ArticleDAL articleDAL = new ArticleDAL();
                try
                {
                    string dossierImage = ConfigurationManager.AppSettings["PathImagesArticles"];
                    string fileName = nomOk.ToUrl() + Path.GetExtension(file.FileName);
                    string chemin = Path.Combine(Server.MapPath(dossierImage), fileName);
                    Image image = Image.FromStream(file.InputStream);
                    int tailleImage = Int32.Parse(ConfigurationManager.AppSettings["ImagesArticlesSize"]);
                    var nouvelleImage = new Bitmap(image, tailleImage, tailleImage);
                    nouvelleImage.Save(chemin);
                    nouvelleImage.Dispose();
                    image.Dispose();
                    lArticle.Image = fileName;
                    articleDAL.Ajouter(lArticle);
                    TempData["AjoutOK"] = "Votre article a bien été ajouté";
                }
                catch (Exception ex)
                {
                    TempData["Erreur"] = ex.Message;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult ModifierArticle()
        {
            if (VariablesSession.AdminSuper || VariablesSession.AdminArticle)
                return View(new ArticleDAL().ListerTout());
            else
                return View();
        }

        [HttpPost]
        public ActionResult ModifierArticle(int id)
        {
            if (VariablesSession.AdminSuper || VariablesSession.AdminArticle)
            {
                ArticleDAL articleDAL = new ArticleDAL();
                ViewBag.ArticleAModifier = articleDAL.Details(id);
                return View(articleDAL.ListerTout());
            }
            else
                return View();
        }

        [HttpPost]
        public ActionResult ModifierArticleEtape2(int id, string nom, string description, string prix, int? grammage, int? litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            if (VariablesSession.AdminSuper || VariablesSession.AdminArticle)
            {
                string nomOk = nom.NomAdmis();
                double prixOk = Math.Abs(Math.Round(float.Parse(prix, CultureInfo.InvariantCulture.NumberFormat), 2));
                int grammageOk = Math.Abs(grammage ?? 0);
                int litrageOk = Math.Abs(litrage ?? 0);
                string descriptionOk = description;
                string allergenesOk = allergenes ?? "";
                int familleIdOk = familleId;
                bool dansCarteOk = dansCarte;

                ArticleDAL articleDAL = new ArticleDAL();
                if (articleDAL.NomExiste(nomOk, id))
                {
                    TempData["Erreur"] = "Le nom de l'article existe déjà. Merci de choisir un autre nom ou bien de renommer d'abord l'article en doublon.";
                }
                else
                {
                    Article lArticle = new Article
                    {
                        Id = id,
                        Nom = nomOk,
                        Description = descriptionOk,
                        Prix = prixOk,
                        Grammage = grammageOk,
                        Litrage = litrageOk,
                        Allergenes = allergenesOk,
                        FamilleId = familleIdOk,
                        DansCarte = dansCarteOk,
                    };
                    try
                    {
                        if (file != null)
                        {
                            string dossierImage = ConfigurationManager.AppSettings["PathImagesArticles"];
                            string fileName = nomOk.ToUrl() + Path.GetExtension(file.FileName);
                            string chemin = Path.Combine(Server.MapPath(dossierImage), fileName);
                            Image image = Image.FromStream(file.InputStream);
                            int tailleImage = Int32.Parse(ConfigurationManager.AppSettings["ImagesArticlesSize"]);
                            var nouvelleImage = new Bitmap(image, tailleImage, tailleImage);
                            nouvelleImage.Save(chemin);
                            nouvelleImage.Dispose();
                            image.Dispose();
                            lArticle.Image = fileName;
                        }
                        else
                        {
                            Article ancienArticle = articleDAL.Details(id);
                            lArticle.Image = ancienArticle.Image;
                        }
                        articleDAL.Modifier(lArticle);
                        TempData["ModifOK"] = "Votre article a bien été modifié";

                    }
                    catch (Exception ex)
                    {
                        TempData["Erreur"] = ex.Message;
                    }
                }
            }
            return View();
        }
    }
}