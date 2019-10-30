using System.Net;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerController : ControllerParent
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (AdminCommande || AdminArticle || AdminPlanning || AdminUtilisateur)
                return View();
            else
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }
    }
}