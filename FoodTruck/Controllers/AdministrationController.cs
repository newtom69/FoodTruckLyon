using System;
using System.Collections.Generic;
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
            if ((bool)Session["AdminSuper"] || (bool)Session["AdminUtilisateur"] || (bool)Session["AdminArticle"] || (bool)Session["AdminCommande"])
            {

            }
            return View(session.Utilisateur);
        }
    }
}