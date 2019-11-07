using FoodTruck.Models;
using System;
using System.Linq;

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

        /// <summary>
        /// Teste la validité d'un code promo. Renseigne dans "montantRemise" le montant de la remise lorsque le code est valide
        /// </summary>
        /// <param name="code"></param>
        /// <param name="montantCommande"></param>
        /// <param name="montantRemise"></param>
        /// <returns></returns>
        internal ValiditeCodePromo Validite(string code, double montantCommande, ref double montantRemise)
        {
            ValiditeCodePromo validite;
            DateTime maintenant = DateTime.Now;
            CodePromo codePromo = Detail(code);
            if (codePromo != null)
            {
                if (codePromo.DateDebut > maintenant)
                {
                    validite = ValiditeCodePromo.DateFuture;
                }
                else if (codePromo.DateFin < maintenant)
                {
                    validite = ValiditeCodePromo.DateDepassee;
                }
                else if (codePromo.MontantMinimumCommande > montantCommande)
                {
                    validite = ValiditeCodePromo.MontantInsuffisant;
                }
                else
                {
                    validite = ValiditeCodePromo.Valide;
                    montantRemise = codePromo.Remise;
                }
            }
            else
            {
                validite = ValiditeCodePromo.Inconnu;
            }
            return validite;
        }
    }
}