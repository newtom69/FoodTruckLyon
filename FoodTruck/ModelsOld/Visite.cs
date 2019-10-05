using System;

namespace FoodTruck.Models2
{
    public class Visite
    {
        public int Id;
        public string AdresseIp;
        public int UtilisateurId;
        public DateTime DateTimeVisite;
        public string Url;
        public string Navigateur;
        public string UrlOrigine;

        public Visite(string url, DateTime dateTimeVisite, string ip, int utilisateurId, string navigateur, string urlOrigine)
        {
            UtilisateurId = utilisateurId;
            AdresseIp = ip;
            DateTimeVisite = dateTimeVisite;
            Url = url;
            Navigateur = navigateur;
            UrlOrigine = urlOrigine;
        }
    }
}