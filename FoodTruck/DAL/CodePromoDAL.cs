using FoodTruck.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodTruck.DAL
{
    public class CodePromoDAL
    {
        internal CodePromo Detail(string code)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                CodePromo codePromo = (from cp in db.CodePromo
                                       where cp.Code == code
                                       select cp).FirstOrDefault();
                return codePromo;
            }
        }
    }
}