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

                // purger dosssier image article
                string dossierImage = Server.MapPath(ConfigurationManager.AppSettings["PathImagesArticles"]);
                var fichiersPresents = (from fullFilename in Directory.EnumerateFiles(dossierImage)
                                       select Path.GetFileName(fullFilename)).ToList();
                
                List<Article> tousArticles = new ArticleDAL().Tous();

                var nomsImages = (from art in tousArticles
                                  select art.Image.Trim()).ToList();

                fichiersPresents.RemoveAll(nom => nomsImages.Contains(nom));
                foreach (string fichier in fichiersPresents)
                {
                    System.IO.File.Delete(Path.Combine(dossierImage, fichier));
                }
                ViewBag.ImagesSupprimees = $"Nombre d'images supprimées : {fichiersPresents.Count}";
            }
            return View();
        }
    }
}