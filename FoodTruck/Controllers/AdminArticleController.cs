using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;
using System;
using System.Web;
using System.IO;
using FoodTruck.Extensions;
using FoodTruck.ViewModels;
using System.Globalization;
using System.Drawing;
using System.Configuration;


namespace FoodTruck.Controllers
{
    public class AdminArticleController : ControllerParent
    {
        [HttpGet]
        public ActionResult Ajouter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ajouter(string nom, string description, string prix, int? grammage, int? litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            string nomOk = nom.NomAdmis();
            double prixOk = Math.Abs(Math.Round(float.Parse(prix, CultureInfo.InvariantCulture.NumberFormat), 2));
            int grammageOk = Math.Abs(grammage ?? 0);
            int litrageOk = Math.Abs(litrage ?? 0);
            string descriptionOk = description;
            string allergenesOk = allergenes ?? "";
            int familleIdOk = familleId;
            bool dansCarteOk = dansCarte;
            if ((bool)Session["AdminSuper"] || (bool)Session["AdminArticle"])
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
        public ActionResult Modifier()
        {
            if ((bool)Session["AdminSuper"] || (bool)Session["AdminArticle"])
                return View(new ArticleDAL().ListerTout());
            else
                return View();
        }

        [HttpPost]
        public ActionResult Modifier(int id)
        {
            if ((bool)Session["AdminSuper"] || (bool)Session["AdminArticle"])
            {
                ArticleDAL articleDAL = new ArticleDAL();
                ViewBag.ArticleAModifier = articleDAL.Details(id);
                return View(articleDAL.ListerTout());
            }
            else
                return View();
        }

        [HttpPost]
        public ActionResult ModifierEtape2(int id, string nom, string description, string prix, int? grammage, int? litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            if ((bool)Session["AdminSuper"] || (bool)Session["AdminArticle"])
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