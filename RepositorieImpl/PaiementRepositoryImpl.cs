using digi_sante.Models;
using System;
using System.Linq;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Web;
namespace digi_sante.Repositories
{
    public class PaiementRepositoryImpl : IPaiementRepository
    {
        Reponse IPaiementRepository.AjouterPaiement(Paiement pp, HttpPostedFileBase file, Consultation cons)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if (pp.patientID != null && pp.structureID != null)
                    {
                        var listOfDoctors = dc.Utilisateurs.Where(a => a.status == true && a.structureID == pp.structureID && a.titre.Equals("Docteur")).ToList();
                        var listOfPatients = dc.Patients.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        var listOfDepartements = dc.Departements.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        var listOfAssurance = dc.Assurances.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        string patientname = null;
                        if (pp.patientID != -1)
                        {
                            long patient = Convert.ToInt64(pp.patientID);
                            patientname = dc.Patients.Where(a => a.patientID == patient).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();

                        }

                        var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(listOfDoctors, listOfPatients, listOfDepartements, new NomPatientAssuranceList { listAssurance = listOfAssurance, nomCompletPatient = patientname });



                        var sizeTableConsultationStart = dc.Consultations.Count();
                        var consultationCurrent = dc.Consultations.Add(cons);
                        dc.SaveChanges();
                        var sizeTableConsultationEnd = dc.Consultations.Count();

                        if (sizeTableConsultationEnd > sizeTableConsultationStart)
                        {
                            var sizeTablePaiementStart = dc.Paiements.Count();
                            pp.consultationID = consultationCurrent.consultationID;
                            var paiementCurrent = dc.Paiements.Add(pp);
                            dc.SaveChanges();
                            var sizeTablePaiementEnd = dc.Paiements.Count();
                            if (sizeTablePaiementEnd > sizeTablePaiementStart)
                            {

                                if (file != null && file.ContentLength > 0)
                                {
                                    string _FileName = Path.GetFileName(file.FileName);
                                    string _FileNameNew = paiementCurrent.paiementID + "-" + _FileName.ToString();
                                    string _path = Path.Combine(HttpContext.Current.Server.MapPath("~/lettrePriseEnCharge"), _FileNameNew);
                                    file.SaveAs(_path);
                                    paiementCurrent.url_lettre_prise_en_charge = _FileNameNew;
                                    dc.Entry(paiementCurrent).State = System.Data.Entity.EntityState.Modified;
                                    dc.SaveChanges();
                                    reponse.statusCode = 200;
                                    reponse.resultat = new { action = "PrintPaiement", controller = "Admin", param = paiementCurrent.paiementID };
                                }
                                else
                                {
                                    reponse.message = "La lettre n'a pas été ajouté ";
                                    reponse.statusCode = 200;
                                    reponse.resultat = new { action = "PrintPaiement", controller = "Admin", param = paiementCurrent.paiementID };

                                }


                            }
                            else
                            {
                                dc.Consultations.Remove(consultationCurrent);
                                dc.SaveChanges();
                                reponse.statusCode = 201;
                                reponse.resultat = tpl;
                            }



                        }
                        else
                        {

                            reponse.statusCode = 201;
                            reponse.message = "Une erreur est survenue lors de la création de consultation ";
                            reponse.resultat = tpl;

                        }

                    }
                    else
                    {
                        var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(null, null, null, null);
                        reponse.statusCode = 201;
                        reponse.message = "Ce Paiement ne peut pas etre effectué ";
                        reponse.resultat = tpl;
                    }



                }
                catch (Exception)
                {
                    if (pp.patientID != null && pp.structureID != null)
                    {
                        var listOfDoctors = dc.Utilisateurs.Where(a => a.status == true && a.structureID == pp.structureID && a.titre.Equals("Docteur")).ToList();
                        var listOfPatients = dc.Patients.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        var listOfDepartements = dc.Departements.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        var listOfAssurance = dc.Assurances.Where(a => a.structureID == pp.structureID && a.status == true).ToList();
                        if (pp.patientID != -1)
                        {
                            long patient = Convert.ToInt64(pp.patientID);
                            var patientname = dc.Patients.Where(a => a.patientID == patient).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                            var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(listOfDoctors, listOfPatients, listOfDepartements, new NomPatientAssuranceList { listAssurance = listOfAssurance, nomCompletPatient = patientname });
                            reponse.resultat = tpl;
                        }
                        else
                        {

                            var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(listOfDoctors, listOfPatients, listOfDepartements, new NomPatientAssuranceList { listAssurance = listOfAssurance, nomCompletPatient = null });

                            reponse.resultat = tpl;
                        }



                    }
                    else
                    {
                        var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(null, null, null, null);

                        reponse.resultat = tpl;

                    }

                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors du paiement ";

                }


            }
            return reponse;
        }


        Reponse IPaiementRepository.ListePaiement(long? userConnectedID, long? structureID, int? page, string searching)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    int pageSize = 10;
                    int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    if (userConnectedID != null && structureID != null)
                    {
                        var tax = dc.Paiements.Where(a => a.structureID == structureID).ToList();
                        var Co = dc.Consultations.Where(a => a.structureID == structureID).ToList();
                        var pati = dc.Patients.ToList();
                        List<PaiementPatient> tecNew = (from c in tax
                                                        join dr in dc.Consultations on c.consultationID equals dr.consultationID
                                                        join cn in pati on dr.patientID equals cn.patientID
                                                        select new PaiementPatient
                                                        {
                                                            Patient = cn.prenom + " " + " " + cn.nom,
                                                            PatientConcat = string.Concat(cn.prenom.Trim() + cn.nom.Trim()).Trim(),
                                                            date_paiement = c.date_paiement,
                                                            paiementID = c.paiementID,
                                                            consultationID = dr.consultationID,
                                                            assurance = (dc.Assurances.Where(a => a.assuranceID == c.assuranceID).Select(b => b.nom).FirstOrDefault() != null) ? dc.Assurances.Where(a => a.assuranceID == c.assuranceID).Select(b => b.nom).FirstOrDefault()
                                                                                                                                                                                          : "-",
                                                            taux_prise_en_charge = c.taux_prise_en_charge,
                                                            url_lettre_prise_en_charge = c.url_lettre_prise_en_charge,
                                                            montant_espece = c.montant_espece,
                                                            montant_consultation = c.montant_consultation,
                                                            montant_assurance = c.montant_assurance,
                                                            departement = (dc.Departements.Where(a => a.structureID == structureID && a.departementID == c.departementID).Select(b => b.nom).FirstOrDefault() != null) ? dc.Departements.Where(a => a.structureID == structureID && a.departementID == c.departementID).Select(b => b.nom).FirstOrDefault() : "-"
                                                        }).ToList();


                        var tec = (searching != null) ? tecNew.Where(a => a.departement.StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.montant_consultation.ToString().StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.assurance.StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.date_paiement.ToString().StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.Patient.StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.PatientConcat.StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         ).OrderByDescending(a => a.date_paiement).ToPagedList(pageIndex, pageSize)
                                                      : tecNew.OrderByDescending(a => a.date_paiement).ToPagedList(pageIndex, pageSize);
                        var tpl = new Tuple<IPagedList<PaiementPatient>, List<Consultation>>(tec, Co);
                        reponse.statusCode = 201;
                        reponse.message = "Liste des  Paiement  ";
                        reponse.resultat = tpl;



                    }
                    else
                    {
                        var tpl = new Tuple<IPagedList<PaiementPatient>, List<Consultation>>(null, null);
                        reponse.statusCode = 201;
                        reponse.message = "une erreur est survenue ";
                        reponse.resultat = tpl;

                    }

                }
                catch (Exception)
                {
                    var tpl = new Tuple<IPagedList<PaiementPatient>, List<Consultation>>(null, null);
                    reponse.statusCode = 500;
                    reponse.message = "une erreur système est survenue ";
                    reponse.resultat = tpl;
                }


            }
            return reponse;

        }

        Reponse IPaiementRepository.ChercherPatientPourPaiement(long? No, long? structureID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null && structureID != null)
                    {
                        var listOfDoctors = dc.Utilisateurs.Where(a => a.status == true && a.structureID == structureID && a.titre.Equals("Docteur")).ToList();
                        var listOfPatients = dc.Patients.Where(a => a.structureID == structureID && a.status == true).ToList();
                        var listOfDepartements = dc.Departements.Where(a => a.structureID == structureID && a.status == true).ToList();
                        var listOfAssurance = dc.Assurances.Where(a => a.structureID == structureID && a.status == true).ToList();
                        if (No != -1)
                        {
                            long patient = Convert.ToInt64(No);
                            var patientname = dc.Patients.Where(a => a.patientID == patient).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                            var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(listOfDoctors, listOfPatients, listOfDepartements, new NomPatientAssuranceList { listAssurance = listOfAssurance, nomCompletPatient = patientname });
                            reponse.statusCode = 200;
                            reponse.resultat = tpl;
                        }
                        else
                        {

                            var tpl = new Tuple<List<Utilisateur>, List<Patient>, List<Departement>, NomPatientAssuranceList>(listOfDoctors, listOfPatients, listOfDepartements, new NomPatientAssuranceList { listAssurance = listOfAssurance, nomCompletPatient = null });
                            reponse.statusCode = 200;
                            reponse.resultat = tpl;
                        }



                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce Paiement n'existe pas ";
                        reponse.resultat = null;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du Paiement ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPaiementRepository.recuPaiement(long? No)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if (No != null)
                    {
                        var paiement = dc.Paiements.Where(a => a.paiementID == No).FirstOrDefault();

                        if (paiement != null)
                        {
                            var structureID = paiement.structureID;
                            var departement = dc.Departements.Where(a => a.structureID == structureID).ToList();

                            var Assurance = dc.Assurances.Where(a => a.structureID == structureID && a.status == true).OrderBy(a => a.assuranceID).ToList();


                            var patientGot = dc.Patients.Where(a => a.patientID == paiement.patientID).FirstOrDefault();
                            var structureGot = dc.Structures.Where(a => a.structureID == structureID).FirstOrDefault();
                            if (patientGot != null && structureGot != null)
                            {
                                var recuPaiement = new RecuPaiement
                                {
                                    assurances = Assurance,
                                    montantapayer = paiement.montant_consultation - paiement.montant_assurance,
                                    structure = structureGot,
                                    Tth = paiement.montant_consultation / 1.18,
                                    Tva = (paiement.montant_consultation * 18) / 118
                                };
                                var tpl = new Tuple<RecuPaiement, Patient, Paiement, List<Departement>>(recuPaiement, patientGot, paiement, departement);
                                reponse.statusCode = 200;
                                reponse.resultat = tpl;


                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "La structure ou le patient n'existe plus ";
                                reponse.resultat = new { action = "ListePaiement", controller = "Admin" };

                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Le paiement n'existe plus ";
                            reponse.resultat = new { action = "ListePaiement", controller = "Admin" };

                        }
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "La paiement ou la structure n'existe plus";
                        reponse.resultat = new { action = "ListePaiement", controller = "Admin" };

                    }


                }
                catch
                {

                    reponse.statusCode = 500;
                    reponse.message = "Une erreur interne est survenue lors de la génération du reçu ";
                    reponse.resultat = new { action = "ListePaiement", controller = "Admin" };
                }


            }
            return reponse;


        }
        Reponse IPaiementRepository.voirPriseEncharge(long? paiementID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (paiementID != null)
                    {
                        var paiement = dc.Paiements.Where(a => a.paiementID == paiementID).FirstOrDefault();
                        if (paiement != null)
                        {

                            var fileSavePath = System.Web.Hosting.HostingEnvironment.MapPath("~/lettrePriseEnCharge/" + paiement.url_lettre_prise_en_charge);
                            var result = System.IO.File.Exists(fileSavePath);
                            if (result)
                            {
                                reponse.statusCode = 200;
                                reponse.resultat = fileSavePath;
                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Le fichier est introuvable ";
                                reponse.resultat = null;

                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce Paiement n'existe pas ";
                            reponse.resultat = null;

                        }

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce Paiement n'existe pas ";
                        reponse.resultat = null;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du Paiement ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

    }
}


