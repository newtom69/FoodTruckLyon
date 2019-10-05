using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (this.Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)this.Session["Panier"];
            this.Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur = null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ArticlesDAL articlesEntree = new ArticlesDAL();
            ViewBag.articlesEntree = articlesEntree.Lister(0, 1);

            ArticlesDAL articlesPlat = new ArticlesDAL();
            ViewBag.articlesPlat = articlesPlat.Lister(0, 2);

            ArticlesDAL articlesDessert = new ArticlesDAL();
            ViewBag.articlesDessert = articlesDessert.Lister(0, 3);

            ArticlesDAL articlesBoissonFraiche = new ArticlesDAL();
            ViewBag.articlesBoissonFraiche = articlesBoissonFraiche.Lister(0, 4);

            ArticlesDAL articlesBoissonChaude = new ArticlesDAL();
            ViewBag.articlesBoissonChaude = articlesBoissonChaude.Lister(0, 5);

            using (VisiteController visite = new VisiteController())
            {
                visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            }

            return View();
        }


        // GET: Article/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (this.Session["Panier"] == null) lePanier = new PanierUI();
            else lePanier = (PanierUI)this.Session["Panier"];
            this.Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ArticleDAL lArticleDAL = new ArticleDAL();
            Article articleCourant = lArticleDAL.Details(id);
            ViewBag.articleCourant = articleCourant;

            return View();
        }

        [HttpGet]
        public ActionResult Ajouter()
        {
            bool droitPage = VerifierDroit();



            //TEST
            Article lArticle = new Article
            {
                Nom = "Nom Test",
                Description = "test ajout produit",
                Image = "ImageTest.jpg",
                Prix = 2.5,
                Grammage = 150,
                DansCarte = true,
                Allergenes = "Gluten",
                FamilleId = 1
            };
            Ajouter(lArticle);
            // Fin TEST


            ViewBag.DroitPage = droitPage;
            return View();
        }

        [HttpPost]
        public ActionResult Ajouter(Article lArticle)
        {
            bool droitPage = VerifierDroit();
            
            if(droitPage)
            {
                ArticleDAL articleDAL = new ArticleDAL();
                articleDAL.AjouterArticleEnBase(lArticle);
                //TODO

            }

            ViewBag.DroitPage = droitPage;
            return View();
        }






        private bool VerifierDroit()
        {
            bool droitPage;
            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
                if (lUtilisateur.AdminArticle || lUtilisateur.AdminTotal)
                    droitPage = true;
                else
                    droitPage = false;
            }
            else
            {
                droitPage = false;
            }

            return droitPage;
        }
    }
}
