using digi_sante.Models;
using digi_sante.Repositories;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digi_sante.Controllers
{
    public class AdminController : Controller
    {
        private IPatientRepository _patientService;
        private IUserRepository _userService;
        private IDepartementRepository _departementService;
        private IAssuranceRepository _assuranceService;
        private IPaiementRepository _paiementService;
        private IStructureRepository _structureService;
        private IListeAnalyseRepository _listeAnalyseService;
        public AdminController(IPatientRepository patientService,
            IUserRepository userService, IDepartementRepository departementService,
            IStructureRepository structureService, IListeAnalyseRepository listeAnalyseService,
            IAssuranceRepository assuranceService, IPaiementRepository paiementService)
        {
            _patientService = patientService;
            _userService = userService;
            _departementService = departementService;
            _assuranceService = assuranceService;
            _paiementService = paiementService;
            _structureService = structureService;
            _listeAnalyseService = listeAnalyseService;


        }

        // PARTIE PATIENT
        public ActionResult Index()
        { 

            return View();
        }
        public ActionResult Parametres()
        {

            return View();
        }
        public ActionResult ListePatients(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.ListePatient(Convert.ToInt64(Session["utilisateurID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {

                        IPagedList<Patient> pt = new List<Patient>().OrderByDescending(a => a.patientID).ToPagedList(1, 1);
                        var tpl = new Tuple<IPagedList<Patient>, List<Utilisateur>, List<Consultation>>(pt, new List<Utilisateur>(), new List<Consultation>());
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                IPagedList<Patient> pt = new List<Patient>().OrderByDescending(a => a.patientID).ToPagedList(1, 1);
                var tpl = new Tuple<IPagedList<Patient>, List<Utilisateur>, List<Consultation>>(pt, new List<Utilisateur>(), new List<Consultation>());
                ViewBag.sms = "une erreur interne est survenue";
                return View(tpl);

            }


        }

        [HttpPost]
        public ActionResult AjouterPatient(string Accompagner, string Name, string Surname, string Gender, DateTime? Birth, string Adresse, string Profession, string Phone, Patient pa)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    pa.nom = Name;
                    pa.status = true;
                    pa.prenom = Surname;
                    pa.adresse = Adresse;
                    pa.sexe = Gender;
                    pa.structureID = Convert.ToInt64(Session["structureID"]);
                    pa.telephone_patient = Phone;
                    pa.profession = Profession;
                    pa.prenom_accompagnant = Accompagner;
                    pa.date_enregistrement = DateTime.Now;
                    pa.date_naissance = Birth.Value.Ticks;
                    Reponse reponse = _patientService.AjouterPatient(pa);

                    if (reponse.statusCode == 200)
                    {
                        return Json(new { code = 200, status = true, message = reponse.message });
                    }
                    else
                    {

                        ViewBag.Name = Name;
                        ViewBag.Surname = Surname;
                        ViewBag.Adresse = Adresse;
                        ViewBag.Gender = Gender;
                        ViewBag.Telephone = Phone;
                        ViewBag.Profession = Profession;
                        ViewBag.Acc = Accompagner;
                        ViewBag.Birth = Birth;
                        ViewBag.sms = reponse.message;
                        return Json(new { code = 201, status = false, message = reponse.message, Name = Name, Surname = Surname, Adresse = Adresse, Gender = Gender, Phone = Phone, Profession = Profession, Accompagner = Accompagner, Birth = Birth });

                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                ViewBag.Name = Name;
                ViewBag.Surname = Surname;
                ViewBag.Adresse = Adresse;
                ViewBag.Gender = Gender;
                ViewBag.Telephone = Phone;
                ViewBag.Profession = Profession;
                ViewBag.Acc = Accompagner;
                ViewBag.Birth = Birth;
                return Json(new { code = 500, status = false, message = "Une erreur interne est survenue", Name = Name, Surname = Surname, Adresse = Adresse, Gender = Gender, Phone = Phone, Profession = Profession, Accompagner = Accompagner, Birth = Birth });
            }

        }
        public ActionResult ModifierPatient(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.ChercherPatient(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.Name = reponse.resultat.nom;
                            ViewBag.Surname = reponse.resultat.prenom;
                            ViewBag.Adresse = reponse.resultat.adresse;
                            ViewBag.Gender = reponse.resultat.sexe;
                            ViewBag.Telephone = reponse.resultat.telephone_patient;
                            ViewBag.Profession = reponse.resultat.profession;
                            ViewBag.Acc = reponse.resultat.prenom_accompagnant;
                            ViewBag.Birth = reponse.resultat.date_naissance.ToString().Substring(0, 10);
                            ViewBag.message = reponse.message;
                            return View();
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.sms = "*ce patient n'exitse pas*";
                        return View();
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult DetailPatient(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.DetailPatient(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return View(reponse.resultat);
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return View(reponse.resultat);
                        }

                    }
                    else
                    {
                        ViewBag.sms = "*ce patient n'exitse pas*";
                        return View(new DetailPatient());
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpPost]
        public ActionResult ModifierPatient(long? No, string Accompagner, string Name, string Surname, string Gender, DateTime? Birth, string Adresse, string Profession, string Phone, Patient utl)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        ViewBag.No = No;
                        Reponse reponse = _patientService.ChercherPatient(No);
                        if (reponse.statusCode == 200)
                        {
                            utl = reponse.resultat;
                            utl.nom = Surname;
                            utl.prenom = Name;
                            utl.telephone_patient = Phone;
                            utl.adresse = Adresse;
                            utl.sexe = Gender;
                            utl.profession = Profession;
                            utl.prenom_accompagnant = Accompagner;
                            utl.date_naissance = Birth.Value.Ticks;
                            Reponse reponseSave = _patientService.ModifierPatient(utl);
                            if (reponseSave.statusCode == 200)
                            {
                                ViewBag.message = reponseSave.message;
                                return Json(new { code = 200, status = true, message = reponseSave.message });
                            }
                            else
                            {

                                return Json(new { code = 201, status = false, message = reponseSave.message, Name = Name, Surname = Surname, Adresse = Adresse, Gender = Gender, Phone = Phone, Profession = Profession, Accompagner = Accompagner, Birth = Birth });
                            }

                        }
                        else
                        {
                            return Json(new { code = 201, status = false, message = reponse.message, Name = Name, Surname = Surname, Adresse = Adresse, Gender = Gender, Phone = Phone, Profession = Profession, Accompagner = Accompagner, Birth = Birth });

                        }
                    }
                    else
                    {
                        ViewBag.sms = "*cet utilisateur n'exitse pas*";
                        return View();
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.Name = Name;
                    ViewBag.Surname = Surname;
                    ViewBag.Adresse = Adresse;
                    ViewBag.Gender = Gender;
                    ViewBag.Telephone = Phone;
                    ViewBag.Profession = Profession;
                    ViewBag.Acc = Accompagner;
                    ViewBag.Birth = Birth;
                    ViewBag.No = No;
                    ViewBag.sms = "*impossible de modifier cet utilisateur*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverPatient(long? No)
        {


            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.bloquerPatient(Convert.ToInt64(No));


                        TempData["lockPatient"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockPatient"] = " ce patient n'existe pas ";
                    }
                }
                else
                {
                    TempData["lockPatient"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockPatient"] = " une erreur majeur est survenue";


            }

        }

        [HttpPost]
        public ActionResult EchangePatientEntreDocteur(string No, string DeDocteur, string ADocteur)
        {


            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.EchangePatientEntreDocteur(Convert.ToInt64(Session["structureID"]), No, DeDocteur, ADocteur);
                        TempData["lockPatient"] = reponse.message;
                    }
                    else
                    {
                        TempData["lockPatient"] = " ce patient n'existe pas ";
                    }
                    return RedirectToAction("ListePatients", "Admin");
                }
                else
                {
                    TempData["lockPatient"] = " La session est écoulée";
                    return RedirectToAction("ListePatients", "Admin");

                }

            }
            catch (Exception)
            {
                TempData["lockPatient"] = " une erreur majeur est survenue";
                return RedirectToAction("ListePatients", "Admin");


            }

        }
        public ActionResult SalleAttente(int? page, string searching)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {


                    Reponse reponse = _patientService.SalleAttente(Convert.ToInt64(Session["structureID"]), page, searching);
                    ViewBag.CurrentFilter = searching;
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);
                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                ViewBag.sms = "une erreur interne est survenue";
                return View();
            }
        }
        public ActionResult ListeConsultation(long? No, int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.ListeConsultation(No, Convert.ToInt64(Session["structureID"]), page, searching);
                    ViewBag.No = No;
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new List<Consultation>().ToPagedList(1, 1);
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                return View(new List<Consultation>().ToPagedList(1, 1));

            }


        }
        public ActionResult ListConsultationPatient(long? No, int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.ListConsultationPatient(Convert.ToInt64(Session["structureID"]), No, page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new List<Consultation>().ToPagedList(1, 1);
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                return View(new List<Consultation>().ToPagedList(1, 1));

            }


        }
        public ActionResult Consultation(long? No)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.Consultation(No, Convert.ToInt64(Session["structureID"]));
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new Tuple<List<Ordonnance>, List<Analyse>, List<Hospitalisation>>(null, null, null);
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                return View();

            }


        }
        public ActionResult OrdonnanceList(long? patientID, int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.OrdonnanceList(Convert.ToInt64(Session["structureID"]), Convert.ToInt64(Session["utilisateurID"]), patientID, page, searching);
                    ViewBag.No = patientID;
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {

                ViewBag.sms = "Une erreur interne est survenue";
                return View(new OrdonnanceList());

            }


        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Consultation(string Patient, string Observation, string Note, string ConID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.Consultation(Observation, Note, ConID);
                    if (reponse.statusCode == 200)
                    {
                        TempData["Consultation"] = reponse.message;
                        return RedirectToAction("Consultation", "Admin", new { No = Convert.ToInt64(ConID) });

                    }
                    else
                    {
                        TempData["Consultation"] = reponse.message;
                        return RedirectToAction("Consultation", "Admin", new { No = Convert.ToInt64(ConID) });
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                TempData["Consultation"] = "* Impossible d'enregistrer cette consultation *";
                return RedirectToAction("Consultation", "Admin", new { No = Convert.ToInt64(ConID) });
            }
        }
        public ActionResult HospitalisationList(string No, int? page, string searching, DateTime? dateEntree, DateTime? dateSortie)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.HospitalisationList(Convert.ToInt64(Session["utilisateurID"]), Convert.ToInt64(No), page, searching, dateEntree, dateSortie);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.No = No;
                        ViewBag.message = reponse.message;
                        ViewBag.CurrentFilter = searching;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.CurrentFilter = searching;
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                var tpl = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);
                ViewBag.sms = "Une erreur interne est survenue";
                return View(tpl);

            }


        }
        [HttpPost]
        public void RecupererConsultation(long? No)
        {

            try
            {

                Reponse reponse = _patientService.RecupererConsultation(Convert.ToInt64(Session["structureID"]), No);
                if (reponse.statusCode == 200)
                {
                    TempData["lockListConsultationPatient"] = reponse.message;

                }
                else
                {
                    TempData["lockListConsultationPatient"] = reponse.message;
                }

            }
            catch
            {
                TempData["lockListConsultationPatient"] = "Une erreur interne est survenue";
            }


        }
        public ActionResult AnalyseList(string No, int? page, string searching)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.AnalyseList(Convert.ToInt64(Session["utilisateurID"]), Convert.ToInt64(No), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.No = No;
                        ViewBag.message = reponse.message;
                        ViewBag.CurrentFilter = searching;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.CurrentFilter = searching;
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                var tpl = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);
                ViewBag.sms = "Une erreur interne est survenue";
                return View(tpl);

            }


        }
        [HttpPost]
        public JsonResult AjouterOrdonnance(IEnumerable<Detail_ordonnance> things, long? consultationID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    var ordo = new Ordonnance
                    {
                        consultationID = consultationID,
                        date_ordonnance = DateTime.Now,
                        structureID = Convert.ToInt64(Session["structureID"]),
                        utilisateurID = Convert.ToInt64(Session["utilisateurID"])
                    };
                    Reponse reponse = _patientService.ajouterOrdonnance(things, consultationID, ordo);

                    return Json(new { status = reponse.resultat, message = reponse.message });


                }
                else
                {
                    return Json(new { status = false, message = "la structure ou l'utilisateur a été supprimé" });
                }
            }
            catch (Exception)
            {
                return Json(new { status = false, message = "une erreur interne est survenue" });
            }
        }

        [HttpPost]
        public ActionResult SelectOrdo(string ordo)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.SelectOrdo(Convert.ToInt64(Session["utilisateurID"]), ordo);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return PartialView(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new SelectOrdo();
                        ViewBag.sms = reponse.message;
                        return PartialView(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                var tpl = new SelectOrdo();
                return PartialView(tpl);

            }


        }

        [HttpPost]
        public ActionResult AnalyseFirst(string analyseID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (analyseID != null)
                    {
                        Reponse reponse = _patientService.AnalyseFirst(Convert.ToInt64(Session["utilisateurID"]), analyseID);
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return PartialView(reponse.resultat);
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return PartialView(reponse.resultat);
                        }

                    }
                    else
                    {
                        ViewBag.sms = "L'analyse n'existe plus ";
                        var tpl = new SelectOrdo();
                        return PartialView(tpl);
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                var tpl = new SelectOrdo();
                return PartialView(tpl);
            }
        }
        [HttpGet]
        public ActionResult PrintAnalyse(long? pres)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.PrintAnalyse(Convert.ToInt64(Session["utilisateurID"]), pres);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return PartialView(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return PartialView(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                var tpl = new PrintPrescription();
                return PartialView(tpl);

            }

        }
        [HttpPost]
        public ActionResult HoFirst(string hostpi)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (hostpi != null)
                    {
                        Reponse reponse = _patientService.HoFirst(Convert.ToInt64(Session["utilisateurID"]), hostpi);
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return PartialView(reponse.resultat);
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return PartialView(reponse.resultat);
                        }

                    }
                    else
                    {
                        ViewBag.sms = "L'hospitalisation n'existe plus ";
                        var tpl = new SelectOrdo();
                        return PartialView(tpl);
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                var tpl = new SelectOrdo();
                return PartialView(tpl);
            }
        }
        [HttpGet]
        public ActionResult PrintResultat(string patientID, long? analyseID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.PrintResultat(patientID, analyseID);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                var tpl = new PrintResultat();
                return View(tpl);

            }

        }
        [HttpPost]
        public void mettreFinHospitalisation(long? No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.mettreFinHospitalisation(Convert.ToInt64(No));


                        TempData["lockHospitalisationList"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockHospitalisationList"] = " cette hospitalisation n'existe pas ";
                    }
                }
                else
                {
                    TempData["lockHospitalisationList"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockHospitalisationList"] = " une erreur majeur est survenue";


            }

        }

        [HttpGet]
        public ActionResult PrintPrescription(long? pres)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.PrintPrescription(Convert.ToInt64(Session["utilisateurID"]), pres);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return PartialView(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return PartialView(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                var tpl = new PrintPrescription();
                return PartialView(tpl);

            }

        }
        [HttpPost]
        public JsonResult AjouterAnalyse(IEnumerable<Detail_analyse> things, long? consultationID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    var analyse = new Analyse
                    {
                        consultationID = consultationID,
                        date_analyse = DateTime.Now,
                        structureID = Convert.ToInt64(Session["structureID"]),
                        utilisateurID = Convert.ToInt64(Session["utilisateurID"])
                    };
                    Reponse reponse = _patientService.ajouterAnalyse(things, consultationID, analyse);
                    if (reponse.statusCode == 200)
                    {
                        return Json(new { status = reponse.resultat, message = reponse.message });


                    }
                    else
                    {
                        return Json(new { status = false, message = "la structure ou l'utilisateur a été supprimé" });
                    }
                }
                else
                {
                    return Json(new { status = false, message = "la structure ou l'utilisateur a été supprimé" });
                }
            }

            catch (Exception)
            {
                return Json(new { status = false, message = "une erreur interne est survenue" });
            }
        }
        [HttpPost]
        public ActionResult AjouterHospitalisation(long? consultationID, long? No, DateTime? dateEntree, DateTime? datesortie, string lit, string salle)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    var hospitalisation = new Hospitalisation
                    {
                        consultationID = consultationID,
                        date_entree = dateEntree,
                        date_sortie = datesortie,
                        numero_lit = lit,
                        numero_salle = salle

                    };
                    Reponse reponse = _patientService.ajouterhospitalisation(consultationID, No, hospitalisation);
                    if (reponse.statusCode == 200)
                    {
                        return Json(new { status = reponse.resultat, message = reponse.message });


                    }
                    else
                    {
                        return Json(new { status = false, message = "la structure ou l'utilisateur a été supprimé" });
                    }
                }
                else
                {
                    return Json(new { status = false, message = "la structure ou l'utilisateur a été supprimé" });
                }
            }

            catch (Exception)
            {
                return Json(new { status = false, message = "une erreur interne est survenue" });
            }
        }
        [HttpGet]
        public ActionResult VoirObservationSuivi(long? idPatient, long? hospitalisationID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.VoirObservationSuivi(idPatient, hospitalisationID);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur interne est survenue";
                var tpl = new VoirObservationSuivi();
                return PartialView(tpl);

            }
        }
        [HttpPost]
        public ActionResult AddObservationToSuivi(string No, string observation)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.AddObservationToSuivi(No, observation);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return View(reponse.resultat);
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return View(reponse.resultat);
                        }

                    }
                    else
                    {
                        ViewBag.sms = "*ce patient n'exitse pas*";
                        return View(new VoirObservationSuivi());
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        // PARTIE UTILISATEUR
        [HttpGet]
        public ActionResult AjouterUtilisateur()
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    Reponse reponse = _departementService.ListeDepartement(Convert.ToInt64(Session["utilisateurID"]), null, null);
                    if (reponse.statusCode == 200)
                    {
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new List<Departement>();
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AjouterUtilisateur(string Name, string Titre, string Surname, string Adresse, string Role, string Pwd, string Phone, string Email, string Id_departement, Utilisateur utl)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    utl.nom = Surname;
                    utl.prenom = Name;
                    utl.status = true;
                    utl.telephone = Phone;
                    utl.role = Role;
                    utl.titre = Titre;
                    utl.structureID = Convert.ToInt64(Session["structureID"]);
                    utl.validite_compte_temporaire = false;
                    utl.date_creation = DateTime.Now;
                    utl.departementID = Convert.ToInt64(Id_departement);
                    utl.adresse = Adresse;
                    utl.email = Email;
                    utl.pwd = BCrypt.Net.BCrypt.HashPassword(Pwd);
                    Reponse reponse = _userService.AjouterUser(Convert.ToInt64(Session["structureID"]), utl);
                    Reponse reponseDepart = _departementService.ListeDepartement(Convert.ToInt64(Session["utilisateurID"]), null, null);
                    if (reponseDepart.statusCode == 200)
                    {


                        if (reponse.statusCode == 200)
                        {

                            ViewBag.message = reponse.message;
                            return View(reponseDepart.resultat);


                        }
                        else
                        {
                            ViewBag.Surname = Surname;
                            ViewBag.Name = Name;
                            ViewBag.Phone = Phone;
                            ViewBag.Id_departement = Id_departement;
                            ViewBag.Role = Role;
                            ViewBag.Titre = Titre;
                            ViewBag.Adresse = Adresse;
                            ViewBag.Email = Email;
                            ViewBag.Pwd = Pwd;
                            ViewBag.message = reponse.message;
                            return View(reponseDepart.resultat);
                        }



                    }
                    else
                    {
                        var tpl = new List<Departement>();
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.Surname = Surname;
                    ViewBag.Name = Name;
                    ViewBag.Phone = Phone;
                    ViewBag.Id_departement = Id_departement;
                    ViewBag.Role = Role;
                    ViewBag.Titre = Titre;
                    ViewBag.Adresse = Adresse;
                    ViewBag.Email = Email;
                    ViewBag.Pwd = Pwd;
                    ViewBag.sms = "*impossible d'enregistrer cet utilisateur*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpGet]
        public ActionResult modifierUtilisateur(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    if (No != null)
                    {


                        Reponse reponseDepart = _departementService.ListeDepartement(Convert.ToInt64(Session["utilisateurID"]), null, null);
                        ViewBag.No = No;
                        if (reponseDepart.statusCode == 200)
                        {
                            Reponse reponse = _userService.ChercherUser(No);
                            if (reponse.statusCode == 200)
                            {
                                ViewBag.Surname = reponse.resultat.prenom;
                                ViewBag.Name = reponse.resultat.nom;
                                ViewBag.Phone = reponse.resultat.telephone;
                                ViewBag.Id_departement = reponse.resultat.departementID;
                                ViewBag.Role = reponse.resultat.role;
                                ViewBag.Titre = reponse.resultat.titre;
                                ViewBag.Adresse = reponse.resultat.adresse;
                                ViewBag.Email = reponse.resultat.email;
                                ViewBag.message = reponse.message;
                                return View(reponseDepart.resultat);

                            }
                            else
                            {
                                ViewBag.message = reponse.message;
                                return View(reponseDepart.resultat);
                            }
                        }
                        else
                        {
                            var tpl = new List<Departement>();
                            ViewBag.sms = reponseDepart.message;
                            return View(tpl);
                        }


                    }
                    else
                    {
                        ViewBag.message = " Cet utlisateur n'existe pas";
                        return View();
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception)
            {
                ViewBag.sms = " une erreur majeur est survenue";
                return View();

            }
        }

        [HttpPost]
        public ActionResult modifierUtilisateur(long? No, string Phone, string Name, string Titre, string Surname, string Adresse, string Role, string Email, string Id_departement, Utilisateur utl)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        ViewBag.No = No;
                        Reponse reponseDepart = _departementService.ListeDepartement(Convert.ToInt64(Session["utilisateurID"]), null, null);
                        if (reponseDepart.statusCode == 200)
                        {
                            Reponse reponse = _userService.ChercherUser(No);
                            if (reponse.statusCode == 200)
                            {
                                utl = reponse.resultat;
                                utl.nom = Surname;
                                utl.prenom = Name;
                                utl.telephone = Phone;
                                utl.role = Role;
                                utl.titre = Titre;
                                utl.departementID = Convert.ToInt64(Id_departement);
                                utl.adresse = Adresse;
                                utl.email = Email;
                                Reponse reponseSave = _userService.ModifierUser(utl);
                                if (reponseSave.statusCode == 200)
                                {
                                    ViewBag.message = reponseSave.message;
                                    return View(reponseDepart.resultat);
                                }
                                else
                                {
                                    ViewBag.Surname = Surname;
                                    ViewBag.Name = Name;
                                    ViewBag.Phone = Phone;
                                    ViewBag.Id_departement = Convert.ToInt64(Id_departement);
                                    ViewBag.Role = Role;
                                    ViewBag.Titre = Titre;
                                    ViewBag.Adresse = Adresse;
                                    ViewBag.Email = Email;
                                    ViewBag.sms = reponseSave.message;
                                    return View(reponseDepart.resultat);
                                }



                            }
                            else
                            {
                                ViewBag.No = No;
                                ViewBag.sms = reponse.message;
                                return View(reponseDepart.resultat);
                            }



                        }
                        else
                        {
                            var tpl = new List<Departement>();
                            ViewBag.sms = reponseDepart.message;
                            ViewBag.No = No;
                            return View(tpl);
                        }
                    }
                    else
                    {
                        ViewBag.Surname = Surname;
                        ViewBag.Name = Name;
                        ViewBag.Phone = Phone;
                        ViewBag.Id_departement = Convert.ToInt64(Id_departement);
                        ViewBag.Role = Role;
                        ViewBag.Titre = Titre;
                        ViewBag.Adresse = Adresse;
                        ViewBag.Email = Email;
                        ViewBag.sms = "*cet utilisateur n'exitse pas*";
                        return View();
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.Surname = Surname;
                    ViewBag.Name = Name;
                    ViewBag.Adresse = Adresse;
                    ViewBag.Email = Email;
                    ViewBag.Phone = Phone;
                    ViewBag.Role = Role;
                    ViewBag.Titre = Titre;
                    ViewBag.No = No;
                    ViewBag.sms = "*impossible de modifier cet utilisateur*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverUtilisateur(long? No)
        {


            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _userService.bloquerUser(Convert.ToInt64(No));


                        TempData["lockUser"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockUser"] = " ce compte n'existe pas ";
                    }
                }
                else
                {
                    TempData["lockUser"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockUser"] = " une erreur majeur est survenue";


            }

        }

        [HttpGet]
        public ActionResult ListeUtilisateur(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _userService.ListeUser(Convert.ToInt64(Session["utilisateurID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new Tuple<IPagedList<Utilisateur>, List<Departement>>(null, null);
                        ViewBag.message = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }

        // PARTIE DEPARTEMENT
         [HttpPost]
        public ActionResult AjouterDepartement(string NameDepartement, Departement departement)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    departement.nom = NameDepartement;
                    departement.status = true;
                    departement.structureID = Convert.ToInt64(Session["structureID"]);
                    Reponse reponse = _departementService.AjouterDepartement(Convert.ToInt64(Session["structureID"]), departement);
                    
                    return Json(new { code = reponse.statusCode, message = reponse.message });                   


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    ViewBag.sms = "*impossible d'enregistrer cet utilisateur*";
                    return Json(new { code = 500, message = "*impossible d'enregistrer cet utilisateur*" });
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult modifierDepartement(long? No, string NameDepartement)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _departementService.ChercherDepartement(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            Departement departement = reponse.resultat;
                            departement.nom = NameDepartement;
                            Reponse reponseNew = _departementService.ModifierDepartement(departement);
                            return Json(new { code = reponseNew.statusCode, message = reponseNew.message });                          

                        }
                        else
                        {
                             return Json(new { code = reponse.statusCode, message = reponse.message });
                        }

                    }
                    else
                    {
                   
                        return Json(new { code = 201, message = "*cet département n'exitse pas*" });

                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                   
                    return Json(new { code = 201, message = "*impossible de modifier cet utilisateur*" });

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverDepartement(string No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _departementService.bloquerDepartement(Convert.ToInt64(No));


                        TempData["lock"] = reponse.message;


                    }
                    else
                    {
                        TempData["lock"] = " ce département n'existe pas ";
                    }
                }
                else
                {
                    TempData["lock"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lock"] = " une erreur majeur est survenue";


            }
        }

        [HttpGet]
        public ActionResult ListeDepartement(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _departementService.ListeDepartement(Convert.ToInt64(Session["utilisateurID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new Tuple<IPagedList<Utilisateur>, List<Departement>>(null, null);
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }


        // PARTIE ASSURANCE
      
        [HttpPost]
        public ActionResult AjouterAssurance(string Assurance, string Adresse, string Phone, string Contact, Assurance ss)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    ss.nom = Assurance;
                    ss.adresse = Adresse;
                    ss.telephone = Phone;
                    ss.prenom_a_contacter = Contact;
                    ss.status = true;
                    ss.structureID = Convert.ToInt64(Session["structureID"]);
                    Reponse reponse = _assuranceService.AjouterAssurance(Convert.ToInt64(Session["structureID"]), ss);                 

                    return Json(new { code = reponse.statusCode, message = reponse.message });                  


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    return Json(new { code = 500, message = "*impossible d'enregistrer cette assurance*" });
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpPost]
        public ActionResult modifierAssurance(long? No, string Assurance, string Adresse, string Phone, string Contact)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _assuranceService.ChercherAssurance(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            Assurance assurance = reponse.resultat;
                            assurance.nom = Assurance;
                            assurance.adresse = Adresse;
                            assurance.telephone = Phone;
                            assurance.prenom_a_contacter = Contact;
                            Reponse reponseNew = _assuranceService.ModifierAssurance(assurance);
                            return Json(new { code = reponseNew.statusCode, message = reponseNew.message });                           


                        }
                        else
                        {
                        return Json(new { code = reponse.statusCode, message = reponse.message });

                        }

                    }
                    else
                    {
                        return Json(new { code = 500, message = "*cette assurance n'exitse pas*" });
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    
                    return Json(new { code = 500, message = "*impossible de modifier cette assurance*" });

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverAssurance(string No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _assuranceService.bloquerAssurance(Convert.ToInt64(No));


                        TempData["lockAssurance"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockAssurance"] = " cette assurance n'existe pas ";
                    }
                }
                else
                {
                    TempData["sms"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockAssurance"] = " une erreur majeur est survenue";


            }
        }

        [HttpGet]
        public ActionResult ListeAssurance(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _assuranceService.ListeAssurance(Convert.ToInt64(Session["utilisateurID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        IPagedList<Assurance> tpl = null;
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }
        //PARTIE PAIEMENT
        [HttpGet]
        public ActionResult AjouterPaiement(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _paiementService.ChercherPatientPourPaiement(No, Convert.ToInt64(Session["structureID"]));
                        ViewBag.InfoPa = No;

                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return View(reponse.resultat);
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.InfoPa = No;
                        ViewBag.sms = "Le paiement ne peut pas ètre éffectué ";
                        return View();
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpGet]
        public ActionResult voirPriseEncharge(long? paiementID)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (paiementID != null)
                    {
                        Reponse reponse = _paiementService.voirPriseEncharge(paiementID);
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.message = reponse.message;
                            return File(reponse.resultat, "application/pdf");
                        }
                        else
                        {
                            ViewBag.sms = reponse.message;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.sms = "Le paiement ne peut pas ètre éffectué ";
                        return View();
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                ViewBag.sms = "Une erreur interne est survenue";
                return View();
            }
        }

        [HttpPost]
        public ActionResult AjouterPaiement(Paiement pp, HttpPostedFileBase file, string MA, string Mcon, string ME, string Ob, string Assurance, Consultation cons, string Id_patient, string Id_docteur, string Id_departement, string taux_prise_en_charge, string url_lettre_prise_en_charge)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    pp.date_paiement = DateTime.Now;
                    pp.montant_consultation = Convert.ToSingle(Mcon);
                    pp.montant_espece = Convert.ToSingle(ME);
                    pp.montant_assurance = Convert.ToSingle(MA);
                    pp.taux_prise_en_charge = taux_prise_en_charge;
                    pp.url_lettre_prise_en_charge = url_lettre_prise_en_charge;
                    pp.observation = Ob;
                    pp.assuranceID = Convert.ToInt64(Assurance);
                    pp.departementID = Convert.ToInt64(Id_departement);
                    pp.caissierID = Convert.ToInt64(Session["utilisateurID"]);
                    pp.structureID = Convert.ToInt64(Session["structureID"]);
                    pp.status = true;
                    pp.patientID = Convert.ToInt64(Id_patient);
                    cons.date_consultation = DateTime.Now;
                    cons.patientID = Convert.ToInt64(Id_patient);
                    cons.departementID = Convert.ToInt64(Id_departement);
                    cons.docteurID = Convert.ToInt64(Id_docteur);
                    cons.status = true;
                    cons.structureID = Convert.ToInt64(Session["structureID"]);
                    cons.isClose = false;
                    ViewBag.InfoPa = Convert.ToInt64(Id_patient);
                    Reponse reponse = _paiementService.AjouterPaiement(pp, file, cons);
                    if (reponse.statusCode == 200)
                    {
                        return Json(new { code = 200, status = true, message = reponse.message, action = reponse.resultat.action, controller = reponse.resultat.controller, param = reponse.resultat.param });

                    }
                    else
                    {
                        return Json(new { code = 201, status = false, message = reponse.message });


                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    return Json(new { code = 500, status = false, message = "*impossible d'enregistrer cette assurance*" });

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }


        [HttpGet]
        public ActionResult ListePaiement(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _paiementService.ListePaiement(Convert.ToInt64(Session["utilisateurID"]), Convert.ToInt64(Session["structureID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }

        [HttpGet]
        public ActionResult PrintPaiement(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _paiementService.recuPaiement(No);
                        if (reponse.statusCode == 200)
                        {
                            return View(reponse.resultat);
                        }
                        else
                        {
                            TempData["lockListePaiement"] = reponse.message;
                            return RedirectToAction(reponse.resultat.action, reponse.resultat.controller);

                        }

                    }
                    else
                    {
                        TempData["lockListePaiement"] = "Le reçu ne peut pas ètre éffectué ";
                        return RedirectToAction("ListePaiement", "Admin");
                    }

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                TempData["lockListePaiement"] = "Une erreur interne est survenue";
                return RedirectToAction("ListePaiement", "Admin");
            }
        }

        // PARTIE STRUCTURE
        [HttpGet]
        public ActionResult AjouterStructure()
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    return View();

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AjouterStructure(string NameStructure, string AdresseStructure, string EmailStructure, string PhoneStructure, string TypeStructure,
                                          string NameManager, string SurnameManager, string TitreManager, string PwdManager, string PhoneManager, string EmailManager, Utilisateur utilisateur, Structure strct)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    strct.nom = NameStructure;
                    strct.adresse = AdresseStructure;
                    strct.email = EmailStructure;
                    strct.telephone = PhoneStructure;
                    strct.status = true;
                    strct.status_premiere_connexion_manager = false;
                    strct.type = TypeStructure;
                    strct.date_creation = DateTime.Now;

                    utilisateur.nom = NameManager;
                    utilisateur.prenom = SurnameManager;
                    utilisateur.titre = TitreManager;
                    utilisateur.pwd = BCrypt.Net.BCrypt.HashPassword(PwdManager);
                    utilisateur.telephone = PhoneManager;
                    utilisateur.email = EmailManager;
                    utilisateur.role = "Manager";
                    utilisateur.status = true;
                    utilisateur.date_creation = DateTime.Now;
                    utilisateur.validite_compte_temporaire = false;

                    Reponse reponse = _structureService.AjouterStructure(Convert.ToInt64(Session["utilisateurID"]), strct, utilisateur);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View();

                    }
                    else
                    {
                        ViewBag.Prenom = SurnameManager;
                        ViewBag.Nom = NameManager;
                        ViewBag.Titre = TitreManager;
                        ViewBag.Pwd = SurnameManager;
                        ViewBag.Email = EmailManager;
                        ViewBag.PhoneManager = PhoneManager;

                        ViewBag.NameStructure = NameStructure;
                        ViewBag.Adresse = AdresseStructure;
                        ViewBag.Email = EmailStructure;
                        ViewBag.Phone = PhoneStructure;
                        ViewBag.Type = TypeStructure;
                        ViewBag.sms = reponse.message;
                        return View();
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.Prenom = SurnameManager;
                    ViewBag.Nom = NameManager;
                    ViewBag.Titre = TitreManager;
                    ViewBag.Pwd = SurnameManager;
                    ViewBag.Email = EmailManager;
                    ViewBag.PhoneManager = PhoneManager;

                    ViewBag.NameStructure = NameStructure;
                    ViewBag.Adresse = AdresseStructure;
                    ViewBag.Email = EmailStructure;
                    ViewBag.Phone = PhoneStructure;
                    ViewBag.Type = TypeStructure;
                    ViewBag.sms = "*impossible d'enregistrer cette assurance*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpGet]
        public ActionResult modifierStructure(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    if (No != null)
                    {

                        Reponse reponse = _structureService.ChercherStructure(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.NameStructure = reponse.resultat.nom;
                            ViewBag.Adresse = reponse.resultat.adresse;
                            ViewBag.Email = reponse.resultat.email;
                            ViewBag.Phone = reponse.resultat.telephone;
                            ViewBag.sms = reponse.message;
                            return View();
                        }
                        else
                        {
                            TempData["lockStructure"] = reponse.message;
                            return RedirectToAction("ListeStructure", "Admin");
                        }
                    }
                    else
                    {
                        ViewBag.No = No;
                        TempData["lockStructure"] = " Cette structure n'existe pas";
                        return RedirectToAction("lockStructure", "Admin");
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception)
            {
                TempData["lockStructure"] = " une erreur majeur est survenue";
                return RedirectToAction("ListeStructure", "Admin");

            }
        }

        [HttpPost]
        public ActionResult modifierStructure(long? No, string NameStructure, string AdresseStructure, string EmailStructure, string PhoneStructure)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _structureService.ChercherStructure(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            Structure structure = reponse.resultat;
                            structure.nom = NameStructure;
                            structure.adresse = AdresseStructure;
                            structure.telephone = PhoneStructure;
                            structure.email = EmailStructure;

                            Reponse reponseNew = _structureService.ModifierStructure(structure);
                            if (reponseNew.statusCode == 200)
                            {

                                ViewBag.message = reponseNew.message;
                                return View();
                            }
                            else
                            {
                                ViewBag.NameStructure = NameStructure;
                                ViewBag.Adresse = AdresseStructure;
                                ViewBag.Email = EmailStructure;
                                ViewBag.Phone = PhoneStructure;
                                ViewBag.sms = reponseNew.message;
                                return View();
                            }



                        }
                        else
                        {
                            ViewBag.NameStructure = NameStructure;
                            ViewBag.Adresse = AdresseStructure;
                            ViewBag.Email = EmailStructure;
                            ViewBag.Phone = PhoneStructure;
                            ViewBag.sms = reponse.message;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.sms = "*cette structure n'exitse pas*";
                        return View();
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.No = No;
                    ViewBag.NameStructure = NameStructure;
                    ViewBag.Adresse = AdresseStructure;
                    ViewBag.Email = EmailStructure;
                    ViewBag.Phone = PhoneStructure;
                    ViewBag.sms = "*impossible de modifier cette structure*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverStructure(string No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _structureService.bloquerStructure(Convert.ToInt64(No));


                        TempData["lockStructure"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockStructure"] = " cette structure n'existe pas ";
                    }
                }
                else
                {
                    TempData["sms"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockStructure"] = " une erreur majeur est survenue";


            }
        }
        [HttpGet]
        public ActionResult ListeStructure(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _structureService.ListeStructure(Convert.ToInt64(Session["structureID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }
        // PARTIE ANALYSE
        [HttpGet]
        public ActionResult AjouterListeAnalyse()
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    return View();

                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult AjouterListeAnalyse(string NameAnalyse, Liste_analyse liste_analyse)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    liste_analyse.nom = NameAnalyse;
                    liste_analyse.status = true;
                    liste_analyse.structureID = Convert.ToInt64(Session["structureID"]);
                    Reponse reponse = _listeAnalyseService.AjouterListeAnalyse(Convert.ToInt64(Session["structureID"]), liste_analyse);
                    if (reponse.statusCode == 200)
                    {

                        ViewBag.message = reponse.message;
                        return View();

                    }
                    else
                    {
                        ViewBag.NameAnalyse = NameAnalyse;
                        ViewBag.sms = reponse.message;
                        return View();
                    }


                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    ViewBag.sms = "*impossible d'enregistrer cette analyse*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        [HttpGet]
        public ActionResult modifierListeAnalyse(long? No)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    if (No != null)
                    {

                        Reponse reponse = _listeAnalyseService.ChercherListeAnalyse(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            ViewBag.NameAnalyse = reponse.resultat.nom;
                            return View();

                        }
                        else
                        {
                            TempData["lockListeAnalyse"] = reponse.message;
                            return RedirectToAction("ListeAnalyse", "Admin");
                        }
                    }
                    else
                    {
                        ViewBag.No = No;
                        TempData["lockListeAnalyse"] = " Cette analyse n'existe pas";
                        return RedirectToAction("ListeAnalyse", "Admin");
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception)
            {
                TempData["lock"] = " une erreur majeur est survenue";
                return RedirectToAction("LisTeDepartement", "Admin");

            }
        }

        [HttpPost]
        public ActionResult modifierListeAnalyse(long? No, string NameAnalyse)
        {
            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _listeAnalyseService.ChercherListeAnalyse(No);
                        ViewBag.No = No;
                        if (reponse.statusCode == 200)
                        {
                            Liste_analyse liste_analyse = reponse.resultat;
                            liste_analyse.nom = NameAnalyse;
                            Reponse reponseNew = _listeAnalyseService.ModifierListeAnalyse(liste_analyse);
                            if (reponseNew.statusCode == 200)
                            {
                                ViewBag.NameAnalyse = reponseNew.resultat.nom;
                                ViewBag.message = reponseNew.message;
                                return View();
                            }
                            else
                            {
                                ViewBag.NameAnalyse = reponse.resultat.nom;
                                ViewBag.sms = reponseNew.message;
                                return View();
                            }



                        }
                        else
                        {
                            ViewBag.NameAnalyse = NameAnalyse;
                            ViewBag.sms = reponse.message;
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.NameAnalyse = NameAnalyse;
                        ViewBag.sms = "*cette analyse n'exitse pas*";
                        return View();
                    }
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    ViewBag.NameAnalyse = NameAnalyse;
                    ViewBag.sms = "*impossible de modifier cette analyse*";
                    return View();
                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }

            }
        }
        [HttpPost]
        public void desactiverListeAnalyse(string No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _listeAnalyseService.bloquerListeAnalyse(Convert.ToInt64(No));


                        TempData["lockListeAnalyse"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockListeAnalyse"] = " cette analyse n'existe pas ";
                    }
                }
                else
                {
                    TempData["lockListeAnalyse"] = " La session est écoulée";

                }

            }
            catch (Exception)
            {
                TempData["lockListeAnalyse"] = " une erreur majeur est survenue";


            }
        }

        [HttpGet]
        public ActionResult ListeAnalyse(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _listeAnalyseService.ListeAnalyse(Convert.ToInt64(Session["structureID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        var tpl = new Tuple<IPagedList<Liste_analyse>>(null);
                        ViewBag.sms = reponse.message;
                        return View(tpl);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Admin");

            }



        }
        [HttpGet]
        public ActionResult ListTransferts(int? page, string searching)
        {

            try
            {
                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {
                    Reponse reponse = _patientService.ListTransferts(Convert.ToInt64(Session["structureID"]), page, searching);
                    if (reponse.statusCode == 200)
                    {
                        ViewBag.CurrentFilter = searching;
                        ViewBag.message = reponse.message;
                        return View(reponse.resultat);

                    }
                    else
                    {
                        ViewBag.sms = reponse.message;
                        return View(reponse.resultat);
                    }


                }
                else
                {
                    TempData["sms"] = "la session est écoulée";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.sms = "Une erreur est survenue lors de la recherche de la liste de  patient transféré ";
                return View(new List<PatientPartageInfos>().ToPagedList(0, 0));

            }



        }
        [HttpGet]
        public ActionResult AnnulerEchangePatientEntreDocteur(long? No)
        {
            try
            {

                if (Session["utilisateurID"] != null && Session["structureID"] != null)
                {

                    if (No != null)
                    {
                        Reponse reponse = _patientService.AnnulerEchangePatientEntreDocteur(No);


                        TempData["lockPatientTransfert"] = reponse.message;


                    }
                    else
                    {
                        TempData["lockPatientTransfert"] = " ne erreur majeur est survenue ";
                    }
                }
                else
                {
                    TempData["lockPatientTransfert"] = " La session est écoulée";

                }
                return RedirectToAction("ListTransferts", "Admin");

            }
            catch (Exception)
            {
                TempData["lockPatientTransfert"] = " une erreur majeur est survenue";
                return RedirectToAction("ListTransferts", "Admin");


            }
        }


    }
}

