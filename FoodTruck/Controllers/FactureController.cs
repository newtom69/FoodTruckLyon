﻿using FoodTruck.DAL;
using FoodTruck.Models;
using FoodTruck.ViewModels;
using SelectPdf;
using System.Web.Mvc;

namespace FoodTruck.Controllers
{
    public class FactureController : ControllerParent
    {
        [HttpGet]
        public ActionResult Guid(string id)
        {
            Facture facture = new FactureDAL().Details(id);
            if (facture != null)
            {
                Commande commande = new CommandeDAL().Detail(facture.CommandeId);
                Client client = new ClientDAL().Details(commande.ClientId);
                CommandeViewModel commandeVM = new CommandeViewModel(commande, client);
                FactureViewModel factureVM = new FactureViewModel(commandeVM);
                ViewBag.FactureId = facture.Id;
                return View(factureVM);
            }
            else
            {
                return View(null as FactureViewModel);
            }
        }
        [HttpPost]
        public ActionResult CommandeVersPdf(int commandeId)
        {
            Commande commande = new CommandeDAL().Detail(commandeId);
            if (commande != null && commande.ClientId == Client.Id && !commande.Annulation && commande.Retrait)
            {
                Facture facture = new FactureDAL().DetailsCommande(commandeId);
                HtmlToPdf htmlToPdf = new HtmlToPdf();
                PdfDocument facturePdf = htmlToPdf.ConvertUrl($"{Request.Url.Scheme}://{Request.Url.Authority}/{ControllerNom}/Guid/{facture.Guid}");
                return File(facturePdf.Save(), "application/pdf", $"factureFoodTruckLyon-commande{commandeId}.pdf");
            }
            else
            {
                TempData["message"] = new Message("Vous n'avez pas de facture associée à ce numéro de commande", TypeMessage.Erreur);
                return Redirect(UrlPrecedente());
            }
        }
    }
}