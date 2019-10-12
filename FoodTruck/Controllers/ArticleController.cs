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

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        [HttpGet]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new ArticleIndexViewModel());
        }

        [HttpGet]
        public ActionResult Details(string nom)
        {
            nom = nom.UrlVersNom();

            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article articleCourant;
            articleCourant = lArticleDAL.Details(nom);
            if (articleCourant == null)
            {
                TempData["ArticleOk"] = false;
            }
            else if (!articleCourant.DansCarte)
            {
                TempData["ArticleOk"] = true;
                TempData["ArticleDansCarte"] = false;
            }
            else
            {
                TempData["ArticleDansCarte"] = true;
                TempData["ArticleOk"] = true;
            }

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new ArticleDetailsViewModel(articleCourant));
        }
        [HttpGet]
        public ActionResult AjouterEnBase()
        {
            bool droitPage = VerifierDroit();
            TempData["DroitPage"] = droitPage;
            return View();
        }

        [HttpPost]
        public ActionResult AjouterEnBase(string nom, string description, string prix, int? grammage, int? litrage, string allergenes, int familleId, bool dansCarte, HttpPostedFileBase file)
        {
            string nomOk = nom.FormatAutoriseNom();
            double prixOk = Math.Abs(Math.Round(float.Parse(prix, CultureInfo.InvariantCulture.NumberFormat), 2));
            int grammageOk = Math.Abs(grammage ?? 0);
            int litrageOk = Math.Abs(litrage ?? 0);
            string descriptionOk = description;
            string allergenesOk = allergenes ?? "";
            int familleIdOk = familleId;
            bool dansCarteOk = dansCarte;
            bool droitPage = VerifierDroit();
            TempData["DroitPage"] = droitPage;
            if (droitPage)
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
                    string fileName = nomOk.ToUrl() + Path.GetExtension(file.FileName);
                    string chemin = Path.Combine(Server.MapPath("/Content/Images"), fileName);
                    Image image = Image.FromStream(file.InputStream);
                    int tailleMin = image.Height < image.Width ? image.Height : image.Width;
                    var nouvelleImage = new Bitmap(image, tailleMin, tailleMin);
                    nouvelleImage.Save(chemin);
                    nouvelleImage.Dispose();
                    image.Dispose();
                    lArticle.Image = fileName;
                    articleDAL.AjouterArticleEnBase(lArticle);
                    TempData["UploadOK"] = "Votre article a bien été ajouté";
                }
                catch (Exception ex)
                {
                    TempData["Erreur"] = ex.Message;
                }
            }

            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }

        private bool VerifierDroit()
        {
            SessionVariables session = new SessionVariables();

            if (session.Utilisateur.AdminArticle || session.Utilisateur.AdminTotal)
                return true;
            else
#if DEBUG
                return true;
#endif
            return false;
        }
    }
}
