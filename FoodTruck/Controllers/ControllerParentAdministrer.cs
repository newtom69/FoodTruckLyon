namespace FoodTruck.Controllers
{
    public class ControllerParentAdministrer : ControllerParent
    {
        public ControllerParentAdministrer()
        {
            ViewBag.PanierLatteralDesactive = true;
            ViewBag.ModeAdmin = true;
            ViewBag.ModeClient = false;
        }
    }
}