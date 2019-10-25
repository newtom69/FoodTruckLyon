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
            CreneauRepasDAL creneauRepasDAL = new CreneauRepasDAL();
            List<PlageHoraireRetrait> plagesHorairesRetrait = creneauRepasDAL.PlagesHorairesRetrait(date);
            List<PlageHoraireRetrait> plagesHorairesRetraitLendemain = creneauRepasDAL.PlagesHorairesRetrait(date.AddDays(1));
            PlageHoraireRetrait plageHoraireRetraitDejeuner = plagesHorairesRetrait.Find(p => p.RepasId == TypeRepas.Déjeuner);
            PlageHoraireRetrait plageHoraireRetraitDiner = plagesHorairesRetrait.Find(p => p.RepasId == TypeRepas.Dîner);
            PlageHoraireRetrait plageHoraireRetraitDejeunerLendemain = plagesHorairesRetraitLendemain.Find(p => p.RepasId == TypeRepas.Déjeuner); //todo évolution prendre le RepasId min
            // TODO à optimiser

            if (plageHoraireRetraitDejeuner.Apres(date) || plageHoraireRetraitDejeuner.Contient(date))
            {
                plageHoraireRetraitDejeuner.Rogner(date);
                return plageHoraireRetraitDejeuner;
            }
            if (plageHoraireRetraitDiner.Apres(date) || plageHoraireRetraitDiner.Contient(date))
            {
                plageHoraireRetraitDiner.Rogner(date);
                return plageHoraireRetraitDiner;
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
        public static string OuiNon(this bool lebool)
        {
            if (lebool)
                return "Oui";
            else
                return "Non";
        }
    }
}