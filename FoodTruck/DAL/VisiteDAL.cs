using FoodTruck.Models;

namespace FoodTruck.DAL
{
    class VisiteDAL
    {
        public VisiteDAL(Visite laVisite)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Visite.Add(laVisite);
                db.SaveChanges();
            }

        }
    }
}
