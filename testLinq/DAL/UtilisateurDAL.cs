using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using testLinq.Models;

namespace testLinq.DAL
{
    class UtilisateurDAL
    {
        public Utilisateur Connexion(string email, string mdp)
        {
            string mdpHash;
            using (SHA256 Hash = SHA256.Create())
                mdpHash = GetHash(Hash, mdp);

            Utilisateur lUtilisateur = new Utilisateur();
            using (foodtruckEntities db = new foodtruckEntities())
            {
                lUtilisateur = (from user in db.Utilisateur
                                where user.Email == email && user.Mdp == mdpHash
                                select user).FirstOrDefault();
            }
            return lUtilisateur;
        }






        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
