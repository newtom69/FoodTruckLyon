using FoodTruck.Models;
using FoodTruck.ViewModels;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class AProposController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Stationnement()
        {
            return View();
        }
    }
}