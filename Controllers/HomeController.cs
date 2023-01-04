using digi_sante.Models;
using digi_sante.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digi_sante.Controllers
{
    public class HomeController : Controller
    {
        private IAuthentificationRepository _authentificationService;
        public HomeController(IAuthentificationRepository authentificationService)
        {
            _authentificationService = authentificationService;
        }

        public ActionResult Index()
        {
            Session["UserA"] = null;
            Session["User"] = null;
            Session["FullName"] =null;
            Session["structureID"] = null;
            Session["utilisateurID"] = null;
            return View();
        }


        [HttpPost] 
        public ActionResult Index(string phone, string pwd)
        {
            Reponse reponse = _authentificationService.Seconnecter(phone, pwd);
           if(reponse.statusCode == 200)
           {
                Session["UserA"] =reponse.resultat.userConnectedID;
               

                return RedirectToAction("Activation", "Home");
           }
            else
           {
        
                 TempData["sms"] = reponse.message;
                    return View();
            }          
            
        }


        [HttpGet] 
        public ActionResult Activation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Activation(string code)
        {
            try
            {
                var userConnectedID = Convert.ToInt64(Session["UserA"]);
                Reponse reponse = _authentificationService.activationCode(userConnectedID, code);
                if (reponse.statusCode == 200)
                {
                    Session["UserA"] = null;
                    Session["FullName"] = reponse.resultat.userConnected.prenom + " " + reponse.resultat.userConnected.nom;
                    Session["structureID"] = reponse.resultat.userConnected.structureID;
                    Session["utilisateurID"] = reponse.resultat.userConnected.utilisateurID;

                    if (reponse.resultat.param != "Null")
                    {
                        return RedirectToAction(reponse.resultat.action, reponse.resultat.controller, reponse.resultat.param);

                    }
                    else
                    {
                        return RedirectToAction(reponse.resultat.action, reponse.resultat.controller);

                    }
                }
                else
                {
                    ViewBag.message = reponse.message;                  
                    return View();
                }
            }
            catch
            {
                ViewBag.message = "L'activation a échoué suite à un problème interne ";
                return View();
            } 
            
        }


    }
}