using FoodTruck.DAL;
using FoodTruck.Extensions;
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
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View(new HomeViewModel());
        }

        [HttpGet]
        public ActionResult Contact()
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Message = "Vous avez des questions sur nos produits ?" +
                " Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous " +
                "et un membre de notre équipe vous répondra dans les plus brefs délais.";
            ViewBag.MailEnvoye = "";
            ViewBag.MailErreur = "";

            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Contact(string nom, string prenom, string email, string comments)
        {
            SessionVariables session = new SessionVariables();
            ViewBag.Message = "Vous avez des questions sur nos produits ? Vous souhaitez prendre contact avec nous ? Remplissez le formulaire ci-dessous" +
                              " et un membre de notre équipe vous répondra dans les plus brefs délais.";
            string nomOk = Server.HtmlEncode(nom);
            string prenomOk = Server.HtmlEncode(prenom);
            string commentsOk = Server.HtmlEncode(comments);
            try
            {
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("info@foodtrucklyon.fr");
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
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = false;
                        client.Send(message);
                    }
                }
                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("info@foodtrucklyon.fr");
                    message.To.Add("info@foodtrucklyon.fr");
                    message.Subject = "Message à partir du formulaire de contact";
                    message.ReplyToList.Add(email);
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
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = false;
                        client.Send(message);
                    }
                }
                ViewBag.MailEnvoye = "Votre message a bien été envoyé.";
                ViewBag.MailErreur = "";
                ViewBag.Message = "";
            }
            catch (Exception)
            {
                Response.StatusCode = 400;
                ViewBag.MailErreur = "Erreur dans l'envoi du mail, veuillez rééssayer s'il vous plait";
                ViewBag.MailEnvoye = "";
            }
            VisiteDAL.Enregistrer(session.Utilisateur.Id);
            return View();
        }
    }
}
