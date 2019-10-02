using FoodTruck.Models;
using System.Web.Mvc;
using FoodTruck.DAL;

namespace FoodTruck.Controllers
{
    public class ArticleController : Controller
    {
        // GET: Article
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur=null;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ArticlesDAL articlesEntree = new ArticlesDAL();
            articlesEntree.Lister(0, 1);
            ViewBag.articlesEntree = articlesEntree;

            ArticlesDAL articlesPlat = new ArticlesDAL();
            articlesPlat.Lister(0, 2);
            ViewBag.articlesPlat = articlesPlat;

            ArticlesDAL articlesDessert = new ArticlesDAL();
            articlesDessert.Lister(0, 3);
            ViewBag.articlesDessert = articlesDessert;

            ArticlesDAL articlesBoissonFraiche = new ArticlesDAL();
            articlesBoissonFraiche.Lister(0, 4);
            ViewBag.articlesBoissonFraiche = articlesBoissonFraiche;

            ArticlesDAL articlesBoissonChaude = new ArticlesDAL();
            articlesBoissonChaude.Lister(0, 5);
            ViewBag.articlesBoissonChaude = articlesBoissonChaude;

            VisiteController visite = new VisiteController();
            visite.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            return View();
        }

        [HttpPost]
        public ActionResult Index(string strJour, string strTypeRepas)
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;
            
            Utilisateur lUtilisateur;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ArticlesDAL articlesEntree = new ArticlesDAL();
            articlesEntree.Lister(0, 1);
            ViewBag.articlesEntree = articlesEntree;

            ArticlesDAL articlesPlat = new ArticlesDAL();
            articlesPlat.Lister(0, 2);
            ViewBag.articlesPlat = articlesPlat;

            ArticlesDAL articlesDessert = new ArticlesDAL();
            articlesDessert.Lister(0, 3);
            ViewBag.articlesDessert = articlesDessert;

            ArticlesDAL articlesBoissonFraiche = new ArticlesDAL();
            articlesBoissonFraiche.Lister(0, 4);
            ViewBag.articlesBoissonFraiche = articlesBoissonFraiche;

            ArticlesDAL articlesBoissonChaude = new ArticlesDAL();
            articlesBoissonChaude.Lister(0, 5);
            ViewBag.articlesBoissonChaude = articlesBoissonChaude;

            return View();
        }


        // GET: Article/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.PanierAbsent = false;
            Panier lePanier;
            if (this.Session["MonPanier"] == null) lePanier = new Panier();
            else lePanier = (Panier)this.Session["MonPanier"];
            this.Session["MonPanier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur;
            if (this.Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)this.Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            Article articleCourant = new Article();
            ArticleDAL lArticleDAL = new ArticleDAL();
            articleCourant = lArticleDAL.Details(id);
            ViewBag.articleCourant = articleCourant;

            return View();
        }
    }
}
