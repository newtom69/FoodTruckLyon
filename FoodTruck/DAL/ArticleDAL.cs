﻿using System;
using System.Linq;
using FoodTruck.Models;

namespace FoodTruck.DAL
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

        internal void AjouterArticleEnBase(Article lArticle)
        {
            using (foodtruckEntities db = new foodtruckEntities())
            {
                db.Article.Add(lArticle);
                db.SaveChanges();
            }
        }
    }
}
