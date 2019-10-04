﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLinq.Models;

namespace testLinq.DAL
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
