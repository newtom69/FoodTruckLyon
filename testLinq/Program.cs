using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testLinq.Models;

namespace testLinq
{


    class Program
    {
        static void Main(string[] args)
        {

            foodtruckEntities db = new foodtruckEntities();


            //public Article Details(int id)
            {
                int id = 5;
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
            }

            //public void AugmenterQuantiteVendue(int id, int nbre)
            {
                int id = 5;
                int nbre = 3;

                //Article larticle = db.Article.Where(c => c.Id == id).First();
                Article larticle = (from article in db.Article
                                    where article.Id == id
                                    select article).FirstOrDefault();
                larticle.NombreVendus += nbre;
                db.SaveChanges();
            }
            //***************
            //ArticleDAL done
            //***************

            //public void ListerRandom(int nombreRetour, int nombreTop)
            {
                int nombreTop = 10;
                int nombreRetour = 3;

                List<Article> articles = (from article in db.Article
                                          where article.DansCarte == true && article.FamilleId <= 3
                                          orderby article.NombreVendus descending
                                          select article)
                                          .Take(nombreTop)
                                          .OrderBy(random => Guid.NewGuid())
                                          .Take(nombreRetour)
                                          .ToList();
            }

            //public void Lister(int nombreMax, int familleId)

            {
                int nombreMax = 0;
                int familleId = 3;
                if (nombreMax == 0) nombreMax = 200;
                List<Article> articles = (from article in db.Article
                                          where article.DansCarte == true && article.FamilleId == familleId
                                          orderby article.Nom
                                          select article)
                                          .Take(nombreMax)
                                          .ToList();
            }
            //*******************
            // articlesDAL done
            //*******************



            // Création de notre client
            //client cli = new client()
            //{
            //    nom = "Dupont";
            //prenom = "Jean";
            //adresse = "160 rue Solferino 59000 Lille"

        }
    }
}
