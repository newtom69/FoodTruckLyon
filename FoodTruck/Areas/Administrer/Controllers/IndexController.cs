using System.Net;
using System.Web.Mvc;

namespace FoodTruck.Areas.Administrer.Controllers
{
    public class IndexController : ControllerParent
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