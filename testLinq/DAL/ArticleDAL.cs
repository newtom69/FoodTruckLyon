using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLinq.Models;

namespace testLinq.DAL
{
    class ArticleDAL
    {
        public Article Details(int id)
        {
            Article larticle;
            using (foodtruckEntities db = new foodtruckEntities())
            {
                larticle = (from article in db.Article
                            where article.Id == id
                            select article).FirstOrDefault();
            }
            return larticle;
        }

        public void AugmenterQuantiteVendue(int id, int nbre)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                larticle.NombreVendus += nbre;
                db.SaveChanges();
            }
        }
    }
}
