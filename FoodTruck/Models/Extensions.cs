using System;
using System.Configuration;
using System.IO;

namespace FoodTruck.Models
{
    public static class Extensions
    {
        public static PlageHoraireRetrait PlageHoraireRetrait(this DateTime date)
        {
            int heurePremierCreneauDejeuner = 0, minutePremierCreneauDejeuner = 0;
            int heureDernierCreneauDejeuner = 0, minuteDernierCreneauDejeuner = 0;
            int heurePremierCreneauDiner = 0, minutePremierCreneauDiner = 0;
            int heureDernierCreneauDiner = 0, minuteDernierCreneauDiner = 0;
            string[] tabpremierCreneauDejeuner = ConfigurationManager.AppSettings["PremierCreneauDejeuner"].Split(':');
            string[] tabdernierCreneauDejeuner = ConfigurationManager.AppSettings["DernierCreneauDejeuner"].Split(':');
            string[] tabpremierCreneauDiner = ConfigurationManager.AppSettings["PremierCreneauDiner"].Split(':');
            string[] tabdernierCreneauDiner = ConfigurationManager.AppSettings["DernierCreneauDiner"].Split(':');
            
            ObtenirHeureMinute(tabpremierCreneauDejeuner, ref heurePremierCreneauDejeuner, ref minutePremierCreneauDejeuner);
            ObtenirHeureMinute(tabdernierCreneauDejeuner, ref heureDernierCreneauDejeuner, ref minuteDernierCreneauDejeuner);
            ObtenirHeureMinute(tabpremierCreneauDiner, ref heurePremierCreneauDiner, ref minutePremierCreneauDiner);
            ObtenirHeureMinute(tabdernierCreneauDiner, ref heureDernierCreneauDiner, ref minuteDernierCreneauDiner);
            DateTime premierCreneauDejeuner = new DateTime(date.Year, date.Month, date.Day, heurePremierCreneauDejeuner, minutePremierCreneauDejeuner, 0);
            DateTime dernierCreneauDejeuner = new DateTime(date.Year, date.Month, date.Day, heureDernierCreneauDejeuner, minuteDernierCreneauDejeuner, 0);
            DateTime premierCreneauDiner = new DateTime(date.Year, date.Month, date.Day, heurePremierCreneauDiner, minutePremierCreneauDiner, 0);
            DateTime dernierCreneauDiner = new DateTime(date.Year, date.Month, date.Day, heureDernierCreneauDiner, minuteDernierCreneauDiner, 0);
            DateTime premierCreneauDejeunerLendemain = premierCreneauDejeuner.AddDays(1);
            DateTime dernierCreneauDejeunerLendemain = dernierCreneauDejeuner.AddDays(1);

            PlageHoraireRetrait plageHoraireRetraitDejeuner = new PlageHoraireRetrait(premierCreneauDejeuner, dernierCreneauDejeuner);
            PlageHoraireRetrait plageHoraireRetraitDiner = new PlageHoraireRetrait(premierCreneauDiner, dernierCreneauDiner);
            PlageHoraireRetrait plageHoraireRetraitDejeunerLendemain = new PlageHoraireRetrait(premierCreneauDejeunerLendemain, dernierCreneauDejeunerLendemain);

            if (plageHoraireRetraitDejeuner.Apres(date))
            {
                return plageHoraireRetraitDejeuner;
            }
            if (plageHoraireRetraitDejeuner.Contient(date))
            {
                return plageHoraireRetraitDejeuner; //TODO rogner
            }
            if (plageHoraireRetraitDiner.Apres(date))
            {
                return plageHoraireRetraitDiner;
            }
            if (plageHoraireRetraitDiner.Contient(date))
            {
                return plageHoraireRetraitDiner; //TODO Rogner
            }
            return plageHoraireRetraitDejeunerLendemain;
        }
        public static string UrlVersNom(this string url)
        {
            return url.Replace("-", " ").Replace("_", "-");
        }
        public static string ToUrl(this string nom)
        {
            return nom.TrimEnd(' ').Replace("-", "_").Replace(" ", "-");
        }
        public static string NomAdmis(this string nom)
        {
            const char espace = ' ';
            const string interdit = "@=&#_;%^";
            return nom.Replace(interdit, espace).Replace(Path.GetInvalidFileNameChars(), espace);
        }
        public static string Replace(this string orig, string to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }
        public static string Replace(this string orig, char[] to, char by)
        {
            foreach (char car in to)
            {
                orig = orig.Replace(car, by);
            }
            return orig;
        }
        private static bool ObtenirHeureMinute(string[] heureEtMinute, ref int heure, ref int minute)
        {
            bool retour = true;
            if (heureEtMinute.Length == 2)
            {
                if (!int.TryParse(heureEtMinute[0], out heure))
                    retour = false;
                if (!int.TryParse(heureEtMinute[1], out minute))
                    retour = false;
            }
            else
            {
                retour = false;
            }
            return retour;
        }
    }
}