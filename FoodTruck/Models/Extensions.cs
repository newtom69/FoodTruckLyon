using FoodTruck.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace FoodTruck.Models
{
    public static class Extensions
    {
        public static PlageHoraireRetrait PlageHoraireRetrait(this DateTime date)
        {
            PlageRepasDAL plageRepasDAL = new PlageRepasDAL();
            OuvertureDAL ouvertureDAL = new OuvertureDAL();
            List<PlageHoraireRetrait> plagesHorairesRetrait = plageRepasDAL.PlagesHorairesRetrait(date);
            List<PlageHoraireRetrait> plagesHorairesRetraitLendemain = plageRepasDAL.PlagesHorairesRetrait(date.AddDays(1));
                               
            PlageHoraireRetrait plageHoraireRetrait1 = plagesHorairesRetrait.Find(p => p.RepasId == TypeRepas.Déjeuner);
            //TODO : si pas de plage trouvé objet à null => erreur.
            // tester si null et prendre le suivant et ainsi de suite

           // PlageHoraireRetrait plage1 = plageRepasDAL.PlageHoraireRetrait(date); //nouvelle méthode

            PlageHoraireRetrait plageTest2 = ouvertureDAL.ProchainOuvert(date);



            PlageHoraireRetrait plageHoraireRetrait2;
            plageHoraireRetrait2 = plagesHorairesRetrait.Find(p => p.RepasId == TypeRepas.Dîner);




            PlageHoraireRetrait plageHoraireRetrait3 = plagesHorairesRetraitLendemain.Find(p => p.RepasId == TypeRepas.Déjeuner); //todo évolution prendre le RepasId min
            // TODO à optimiser

            if (plageHoraireRetrait1.Apres(date) || plageHoraireRetrait1.Contient(date))
            {
                plageHoraireRetrait1.Rogner(date);
                return plageHoraireRetrait1;
            }
            if (plageHoraireRetrait2.Apres(date) || plageHoraireRetrait2.Contient(date))
            {
                plageHoraireRetrait2.Rogner(date);
                return plageHoraireRetrait2;
            }
            return plageHoraireRetrait3;
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
        public static string OuiNon(this bool lebool)
        {
            if (lebool)
                return "Oui";
            else
                return "Non";
        }
    }
}