using FoodTruck.DAL;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerBddController : ControllerParentAdministrer
    {
        public ActionResult PurgerBdd()
        {
            if (AdminUtilisateur)
            {
                ViewBag.PanierProspectSupprimes = "Nombre d'enregistrements de PanierProspect supprimés : " + new PanierProspectDAL("").Purger(30);
                ViewBag.OubliMotDePasseSupprimes = "Nombre d'enregistrements de OubliMotDePasse supprimés : " + new OubliMotDePasseDAL().Purger();
                ViewBag.JourExceptionnelSupprimes = "Nombre d'enregistrements de JourExceptionnel supprimés : " + new JourExceptionnelDAL().Purger();
                ViewBag.ImagesSupprimees = $"Nombre d'images supprimées : {PurgerImages()}";
            }
            return View();
        }

        private int PurgerImages()
        {
            string dossierImage = Server.MapPath(ConfigurationManager.AppSettings["PathImagesArticles"]);
            var nomsFichiers = (from fullFilename in Directory.EnumerateFiles(dossierImage)
                                select Path.GetFileName(fullFilename)).ToList();

            List<Article> tousArticles = new ArticleDAL().Tous();

            var nomsImages = (from art in tousArticles
                              select art.Image.Trim()).ToList();

            nomsFichiers.RemoveAll(nom => nomsImages.Contains(nom));
            foreach (string fichier in nomsFichiers)
            {
                System.IO.File.Delete(Path.Combine(dossierImage, fichier));
            }
            return nomsFichiers.Count;
        }
    }
}