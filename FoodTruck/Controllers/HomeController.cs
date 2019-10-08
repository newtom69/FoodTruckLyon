using FoodTruck.DAL;
using FoodTruck.Models;
using System;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            ArticlesDAL articles = new ArticlesDAL();
            ViewBag.Articles = articles.ListerRandom(3, 7);


            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.PanierAbsent = false;
            PanierUI lePanier;
            if (Session["Panier"] == null)
                lePanier = new PanierUI();
            else
                lePanier = (PanierUI)Session["Panier"];
            Session["Panier"] = lePanier;
            ViewBag.Panier = lePanier;

            Utilisateur lUtilisateur = null;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur)Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }

            ViewBag.Message = "Vous avez des questions sur nos produits ?" +
                " Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous " +
                "et un membre de notre équipe vous répondra dans les plus brefs délais.";
            ViewBag.MailEnvoye = "";

            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Contact(string nom, string prenom, string email, string comments)
        {
            ViewBag.PanierAbsent = false;

            Utilisateur lUtilisateur;
            if (Session["Utilisateur"] != null)
            {
                lUtilisateur = (Utilisateur) Session["Utilisateur"];
                ViewBag.lUtilisateur = lUtilisateur;
            }
            else
            {
                lUtilisateur = new Utilisateur();
            }

            ViewBag.Message = "Vous avez des questions sur nos produits ?" +
                              " Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous" +
                              " et un membre de notre équipe vous répondra dans les plus brefs délais.";

            string nomOk = Server.HtmlEncode(nom);
            string prenomOk = Server.HtmlEncode(prenom);
            string commentsOk = Server.HtmlEncode(comments);

            try
            {
                using (MailMessage message = new MailMessage { From = new MailAddress("info@foodtruck-lyon.com") })
                {
                    message.To.Add(email);
                    message.Subject = "Message à partir du formulaire de contact";

                    StringBuilder mastringbuilder = new StringBuilder();

                    mastringbuilder.Append(
                        "<html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                    mastringbuilder.Append(
                        "<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\"><title>Mail du site</title></head><body><h3>De : ");
                    mastringbuilder.Append(prenomOk + " " + nomOk);
                    mastringbuilder.Append("</h3><h3>Message : </h3><p>");
                    mastringbuilder.Append(commentsOk);
                    mastringbuilder.Append("<br/>");
                    mastringbuilder.Append("<br/>Ceci est une copie du message envoyé à votre FoodTruck. Vous recevrez bientôt une réponse. Merci");
                    mastringbuilder.Append("</p></body></html>");

                    message.Body = mastringbuilder.ToString();
                    message.IsBodyHtml = true;

                    using (SmtpClient client = new SmtpClient { EnableSsl = true })
                    {
                        client.Send(message);
                    }
                }
                ViewBag.MailEnvoye = "Votre message a bien été envoyé.";
                ViewBag.Message = "";
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.MailEnvoye = "Erreur dans l'envoi du mail, veuillez rééssayer s'il vous plait";
            }
            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(email)
                };
                message.To.Add("info@foodtruck-lyon.com");
                message.Subject = "Message à partir du formulaire de contact";

                StringBuilder mastringbuilder = new StringBuilder();

                mastringbuilder.Append(
                    "<html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                mastringbuilder.Append(
                    "<meta http-equiv=\"X-UA-Compatible\" content=\"ie=edge\"><title>Mail du site</title></head><body><h3>De : ");
                mastringbuilder.Append(prenomOk + " " + nomOk);
                mastringbuilder.Append("</h3><h3>Message : </h3><p>");
                mastringbuilder.Append(commentsOk);
                mastringbuilder.Append("<br/>");
                mastringbuilder.Append("</p></body></html>");

                message.Body = mastringbuilder.ToString();
                message.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient { EnableSsl = true })
                {
                    client.Send(message);
                }
                ViewBag.MailEnvoye = "Votre message a bien été envoyé.";
                ViewBag.Message = "";
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.MailEnvoye = "Erreur dans l'envoi du mail, veuillez rééssayer s'il vous plait";
            }
            
            VisiteDAL.Enregistrer(lUtilisateur != null ? lUtilisateur.Id : 0);

            return View();
        }
    }
}
