using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using System;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            //if (session.Utilisateur.Id == 0)
            //{
            //    HttpCookie cookie = Request.Cookies.Get("Email");
            //    if (cookie != null)
            //    {
            //        UtilisateurDAL utilisateurDAL = new UtilisateurDAL();
            //        Utilisateur utilisateur = utilisateurDAL.ConnexionCookies(cookie.Value);
            //        Session["Utilisateur"] = utilisateur;
            //        ViewBag.Utilisateur = utilisateur;
            //    }
            //}


            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new HomeViewModel());
        }

        [HttpGet]
        public ActionResult Contact()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            ViewBag.Message = "Vous avez des questions sur nos produits ?" +
                " Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous " +
                "et un membre de notre équipe vous répondra dans les plus brefs délais.";
            ViewBag.MailEnvoye = "";

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Contact(string nom, string prenom, string email, string comments)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Panier = session.PanierViewModel;
            ViewBag.Utilisateur = session.Utilisateur;

            ViewBag.Message = "Vous avez des questions sur nos produits ? Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous" +
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
            
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }
    }
}
