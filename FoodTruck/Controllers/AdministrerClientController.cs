using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AdministrerClientController : ControllerParentAdministrer
    {
        public ActionResult Client()
        {
            return View();
        }
    }
}