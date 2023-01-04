using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Web.UI;
using System.Net;
using System.IO;

namespace digi_sante.Repositories
{
    public class PatientRepositoryImpl : IPatientRepository
    {
        Reponse IPatientRepository.AjouterPatient(Patient patient)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {

                    int listPatientSize = db.Patients.Where(a => a.structureID == patient.structureID
                                                                  && a.telephone_patient.Equals(patient.telephone_patient)
                                                                ).Select(a => a.patientID).ToList().Count();
                    if (listPatientSize > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Ce numéro de téléphone est déjà utilisé par un patient*";
                        reponse.resultat = patient;
                    }
                    else
                    {
                        Patient patientSave = db.Patients.Add(patient);
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "Enregistrement du patient a reussi";
                        reponse.resultat = patientSave;

                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du patient ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.bloquerPatient(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var patient = db.Patients.Where(a => a.patientID == id && a.status == true).FirstOrDefault();
                        if (patient != null)
                        {
                            patient.status = false;
                            db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation du patient a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce patient n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation du patient ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.ListePatient(long? userConnectedID, int? page, string searching)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if (userConnectedID != null)
                    {

                        var UserNeedList = dc.Utilisateurs.Where(a => a.utilisateurID == userConnectedID && a.status == true).FirstOrDefault();
                        int pageSize = 10;
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var listeDocteur = dc.Utilisateurs.Where(a => a.structureID == UserNeedList.structureID && a.titre == "Docteur" && a.status == true).ToList();
                        var listConsultations = dc.Consultations.Where(c => c.structureID == UserNeedList.structureID).ToList();
                        var listOfPatients = dc.Patients.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToList();
                        var listOfDepartements = dc.Departements.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToList();
                        var listOfAssurance = dc.Assurances.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToList();

                        if (UserNeedList != null)
                        {

                            if (UserNeedList.role == "Manager")
                            {


                                var st = UserNeedList.structureID;
                                var lstPatient = dc.Patients.Where(a => a.structureID == st && a.status == true && a.sexe.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.nom.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.prenom.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.profession.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.date_enregistrement.ToString().StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.date_naissance.ToString().StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.adresse.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && a.telephone_patient.ToString().StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && searching == null
                                                                             || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                                             ).OrderByDescending(a => a.patientID).ToPagedList(pageIndex, pageSize);

                                var listPaiementData = new ListPatientData
                                {
                                    liste_assurances = listOfAssurance,
                                    liste_consultations = listConsultations,
                                    liste_departements = listOfDepartements,
                                    liste_docteurs = listeDocteur,
                                    patients = lstPatient
                                };

                                reponse.statusCode = 200;
                                reponse.message = "Liste des patients pour manager";
                                reponse.resultat = listPaiementData;
                            }
                            else
                            {
                                switch (UserNeedList.titre)
                                {
                                    case "Docteur":
                                        var st = UserNeedList.structureID;
                                        var listConsultationOfDoctor = dc.Consultations.Where(a => a.docteurID == UserNeedList.utilisateurID && a.structureID == UserNeedList.structureID).ToList();
                                        var lstPatient = dc.Patients.Where(a => a.structureID == st && a.status == true && a.sexe.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.nom.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.prenom.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.profession.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.date_enregistrement.ToString().StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.date_naissance.ToString().StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.adresse.StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && a.telephone_patient.ToString().StartsWith(searching)
                                                                 || a.structureID == st && a.status == true && searching == null
                                                                 || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching))
                                        .OrderByDescending(a => a.patientID);
                                        var listPatientOfDoctor = (from ta in lstPatient
                                                                   join tb in listConsultationOfDoctor on ta.patientID equals tb.patientID
                                                                   select new Patient
                                                                   {
                                                                       patientID = ta.patientID,
                                                                       nom = ta.nom,
                                                                       prenom = ta.prenom,
                                                                       adresse = ta.adresse,
                                                                       sexe = ta.sexe,
                                                                       telephone_patient = ta.telephone_patient,
                                                                       profession = ta.profession,
                                                                       prenom_accompagnant = ta.prenom_accompagnant,
                                                                       date_enregistrement = ta.date_enregistrement,
                                                                       date_naissance = ta.date_naissance
                                                                   }).ToPagedList(pageIndex, pageSize);
                                        var listPaiementData = new ListPatientData
                                        {
                                            liste_assurances = listOfAssurance,
                                            liste_consultations = listConsultations,
                                            liste_departements = listOfDepartements,
                                            liste_docteurs = listeDocteur,
                                            patients = listPatientOfDoctor
                                        };
                                        reponse.statusCode = 200;
                                        reponse.message = "Liste des patients pour docteur";
                                        reponse.resultat = listPaiementData;


                                        break;
                                    case "Infirmier":
                                        // code block
                                        break;
                                    default:
                                        var stN = UserNeedList.structureID;
                                        var listPatients = dc.Patients.Where(a => a.structureID == stN && a.status == true && a.sexe.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.nom.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.prenom.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.profession.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.date_enregistrement.ToString().StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.date_naissance.ToString().StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.adresse.StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && a.telephone_patient.ToString().StartsWith(searching)
                                                                                                                     || a.structureID == stN && a.status == true && searching == null
                                                                                                                     || a.structureID == stN && a.status == true && a.prenom_accompagnant.StartsWith(searching))
                                                                                            .OrderByDescending(a => a.patientID).ToPagedList(pageIndex, pageSize); ;
                                        var listPaiementDataAutre = new ListPatientData
                                        {
                                            liste_assurances = listOfAssurance,
                                            liste_consultations = listConsultations,
                                            liste_departements = listOfDepartements,
                                            liste_docteurs = listeDocteur,
                                            patients = listPatients
                                        };
                                        reponse.statusCode = 200;
                                        reponse.message = "Liste des patients pour ce compte";
                                        reponse.resultat = listPaiementDataAutre;
                                        break;
                                }

                            }

                        }
                        else
                        {

                            reponse.statusCode = 201;
                            reponse.message = "La liste est vide car ce compte n'existe pas";
                            reponse.resultat = new ListPatientData();
                        }


                    }
                    else
                    {

                        reponse.statusCode = 201;
                        reponse.message = "Ce compte n'existe plus ";
                        reponse.resultat = new ListPatientData();


                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement des patients ";
                    reponse.resultat = new ListPatientData();
                }


            }
            return reponse;

        }

        Reponse IPatientRepository.ModifierPatient(Patient patient)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {

                try
                {

                    int listPatientSize = db.Patients.Where(a => a.telephone_patient != null
                    && a.structureID == patient.structureID && a.patientID != patient.patientID
                    && a.telephone_patient.Equals(patient.telephone_patient)
                                                       ).Select(a => a.patientID).ToList().Count();
                    if (listPatientSize > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Ce numéro de téléphone est déjà utilisé par un patient*";
                        reponse.resultat = patient;
                    }
                    else
                    {
                        db.Entry(patient).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "La modification du patient a reussi";
                        reponse.resultat = patient;

                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du patient ";
                    reponse.resultat = null;
                }

            }
            return reponse;
        }

        Reponse IPatientRepository.ChercherPatient(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var patient = db.Patients.Where(a => a.patientID == id && a.status == true).FirstOrDefault();
                        if (patient != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat = patient;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce patient n'existe pas ";
                            reponse.resultat = id;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient n'existe pas ";
                        reponse.resultat = id;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.DetailPatient(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var patient = db.Patients.Where(a => a.patientID == id && a.status == true).FirstOrDefault();
                        var listUsers = db.Utilisateurs.Where(a => a.structureID == patient.structureID).ToList();
                        var cons = db.Consultations.Where(a => a.patientID == patient.patientID).OrderByDescending(a => a.consultationID).ToList();
                        var list = db.Analyses.ToList();
                        var lstAnalyse = (from ta in cons
                                          join tb in list on ta.consultationID equals tb.consultationID
                                          select new Analyse
                                          {
                                              analyseID = tb.analyseID,
                                              date_analyse = tb.date_analyse
                                          }).ToList();

                        if (patient != null)
                        {
                            var detailPatient = new DetailPatient
                            {
                                Acc = patient.prenom_accompagnant,
                                analyses = lstAnalyse,
                                Birth = patient.date_naissance.ToString().Substring(0, 10),
                                Gender = patient.sexe,
                                Name = patient.nom,
                                Profession = patient.profession,
                                Surname = patient.prenom,
                                Telephone = patient.telephone_patient,
                                Adresse = patient.adresse,
                                Age = (DateTime.Now.Subtract(patient.date_naissance.Value)).Days / 365,
                                Nationalite = patient.nationnalite,
                                utilisateurs = listUsers

                            };
                            reponse.statusCode = 200;
                            reponse.resultat = detailPatient;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce patient n'existe pas ";
                            reponse.resultat = new DetailPatient();
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient n'existe pas ";
                        reponse.resultat = new DetailPatient();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = new DetailPatient();
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.ListeConsultation(long? No, long? structureID, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())

            {
                try
                {
                    if (No != null && structureID != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var docteurList = dc.Utilisateurs.Where(a => a.structureID == structureID && a.titre.Equals("Docteur")).ToList();
                        var departementList = dc.Departements.Where(a => a.structureID == structureID).ToList();
                        var listConsultationOfUserConnectedStructure = dc.Consultations.Where(a => a.structureID == structureID && a.docteurID != 0 && a.patientID == No).OrderByDescending(a => a.consultationID).ToPagedList(pageIndex, pageSize);
                        var listeConsultaton = (from consult in listConsultationOfUserConnectedStructure
                                                join docteur in docteurList on consult.docteurID equals docteur.utilisateurID
                                                join depart in departementList on docteur.departementID equals depart.departementID
                                                select new ListConsultation
                                                {
                                                    consultationID = consult.consultationID,
                                                    dateDebut = consult.date_consultation,
                                                    dateFin = consult.date_fin,
                                                    departement = depart.nom,
                                                    isClose = consult.isClose,
                                                    nomTraitant = docteur.nom,
                                                    patientID = No,
                                                    prenomTraitant = docteur.prenom
                                                }).ToList();

                        var listeConsultatonFinal = (searching != null) ? listeConsultaton.Where(a => a.consultationID != null && a.departement.StartsWith(searching)
                                                                || a.consultationID != null && a.nomTraitant.StartsWith(searching)
                                                                || a.consultationID != null && a.prenomTraitant.StartsWith(searching)
                                                                 ).OrderByDescending(a => a.patientID).ToPagedList(pageIndex, pageSize)
                                                            : listeConsultaton.OrderByDescending(a => a.patientID).ToPagedList(pageIndex, pageSize);





                        reponse.statusCode = 200;
                        reponse.message = "Liste de consultation ";
                        reponse.resultat = listeConsultatonFinal;
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient n'exitse pas ";
                        reponse.resultat = No;

                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.Consultation(long? No, long? structureID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if (No != null)
                    {
                        var con = Convert.ToInt64(No);
                        var id_patient = dc.Consultations.Where(a => a.consultationID == con).Select(a => a.patientID).FirstOrDefault();
                        var paiement = dc.Paiements.Where(o => o.consultationID == con).FirstOrDefault();
                        if (structureID != null && id_patient != null && paiement != null)
                        {
                            var patient = dc.Patients.Where(b => b.patientID == id_patient).FirstOrDefault();
                            var listOrdo = dc.Ordonnances.Where(a => a.consultationID == con).OrderByDescending(a => a.ordonnanceID).ToList();
                            var listAna = dc.Analyses.Where(a => a.consultationID == con).OrderByDescending(a => a.analyseID).ToList();
                            var con12 = dc.Hospitalisations.Where(a => a.consultationID == con).OrderByDescending(a => a.hospitalisationID).ToList();

                            var consultation = dc.Consultations.Where(a => a.consultationID == No).FirstOrDefault();
                            if (consultation != null && patient != null)
                            {


                                var tuple = new Tuple<List<Ordonnance>, List<Analyse>, List<Hospitalisation>, ConsultationData>(listOrdo, listAna, con12
                                         , new ConsultationData
                                         {
                                             patientID = id_patient,
                                             constant = consultation.constant,
                                             consultationID = con,
                                             observationPaiement = paiement.observation,
                                             observationTraitant = consultation.observation,
                                             liste_anaylses = dc.Liste_analyses.ToList(),
                                             liste_structures = dc.Structures.Where(g => g.status == true).ToList()

                                         });
                                reponse.statusCode = 200;
                                reponse.message = "Consultation n°" + consultation.consultationID + "/ " + patient.prenom + "-" + patient.nom;
                                reponse.resultat = tuple;
                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "La consultation n'existe pas ";
                                reponse.resultat = null;
                            }


                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "La structure ou le patient ou le paiement n'existe pas ";
                            reponse.resultat = false;

                        }



                    }
                    else
                    {

                        reponse.statusCode = 201;
                        reponse.message = "La consultation n'existe pas ";
                        reponse.resultat = false;
                    }




                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation du patient ";
                    reponse.resultat = false;
                }


            }
            return reponse;

        }

        Reponse IPatientRepository.ajouterOrdonnance(IEnumerable<Detail_ordonnance> things, long? consultationID, Ordonnance dd)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    var tableSizeOrdonnanceStart = dc.Ordonnances.Count();
                    var currentOrdo = dc.Ordonnances.Add(dd);
                    dc.SaveChanges();
                    var tableSizeOrdonnanceEnd = dc.Ordonnances.Count();
                    if (tableSizeOrdonnanceEnd > tableSizeOrdonnanceStart)
                    {
                        var listDetailsOrdo = (from x in things

                                               select new Detail_ordonnance
                                               {
                                                   ordonnanceID = currentOrdo.ordonnanceID,
                                                   description = x.description,
                                                   consultationID = consultationID,
                                                   patientID = x.patientID,
                                                   status = true
                                               });
                        dc.Detail_ordonnances.AddRange(listDetailsOrdo);
                        dc.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "L'ordonnance a été ajouté";
                        reponse.resultat = true;
                    }
                    else
                    {
                        dc.Ordonnances.Remove(currentOrdo);
                        dc.SaveChanges();
                        reponse.statusCode = 201;
                        reponse.message = "L'ordonnance n'a pas  été ajouté";
                        reponse.resultat = false;
                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'ajout de l'ordonnance ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.ajouterAnalyse(IEnumerable<Detail_analyse> things, long? consultationID, Analyse dd)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    var tableSizeAnalyseStart = dc.Analyses.Count();
                    dd.ResultatEstDisponible = false;
                    var currentAnalyse = dc.Analyses.Add(dd);
                    dc.SaveChanges();
                    var tableSizeAnalyseEnd = dc.Analyses.Count();
                    if (tableSizeAnalyseEnd > tableSizeAnalyseStart)
                    {

                        var listDetailsAnalyse = (from x in things

                                                  select new Detail_analyse
                                                  {
                                                      analyseID = currentAnalyse.analyseID,
                                                      listeAnalyseID = x.listeAnalyseID,
                                                      IdLab = Convert.ToInt32(x.IdLab),
                                                      patientID = x.patientID,
                                                      consultationID = consultationID,
                                                      status = true
                                                  });
                        dc.Detail_analyses.AddRange(listDetailsAnalyse);
                        dc.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "L'analyse a été ajouté";
                        reponse.resultat = true;
                    }
                    else
                    {
                        dc.Analyses.Remove(currentAnalyse);
                        dc.SaveChanges();
                        reponse.statusCode = 201;
                        reponse.message = "L'analyse n'a pas  été ajouté";
                        reponse.resultat = false;
                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'ajout de l'analyse ";
                    reponse.resultat = false;
                }



            }
            return reponse;
        }

        Reponse IPatientRepository.ajouterhospitalisation(long? consultationID, long? No, Hospitalisation ho)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    var hospitalisationSizeStart = dc.Hospitalisations.Count();
                    var hospitalisationCreate = dc.Hospitalisations.Add(ho);
                    dc.SaveChanges();
                    var hospitalisationSizeEnd = dc.Hospitalisations.Count();
                    if (hospitalisationSizeEnd > hospitalisationSizeStart)
                    {

                        var detail_hospitalisation = new Detail_hospitalisation
                        {
                            consultationID = consultationID,
                            patientID = No,
                            status = true,
                            hospitalisationID = hospitalisationCreate.hospitalisationID
                        };
                        var detailsHospitalisationSizeStart = dc.Detail_hospitalisations.Count();
                        var detail_hospitalisationCreate = dc.Detail_hospitalisations.Add(detail_hospitalisation);
                        dc.SaveChanges();
                        var detailsHospitalisationSizeEnd = dc.Detail_hospitalisations.Count();

                        if (detailsHospitalisationSizeEnd > detailsHospitalisationSizeStart)
                        {


                            var suivi = new Suivi
                            {
                                consultationID = hospitalisationCreate.consultationID,
                                date_suivi = DateTime.Now,
                                hospitalisationID = hospitalisationCreate.hospitalisationID
                            };

                            var suiviSizeStart = dc.Suivis.Count();
                            dc.Suivis.Add(suivi);
                            dc.SaveChanges();
                            var suiviSizeEnd = dc.Suivis.Count();

                            if (suiviSizeEnd > suiviSizeStart)
                            {
                                reponse.statusCode = 200;
                                reponse.message = "L'hospitalisation a été ajouté";
                                reponse.resultat = true;

                            }
                            else
                            {
                                dc.Hospitalisations.Remove(hospitalisationCreate);
                                dc.SaveChanges();
                                dc.Detail_hospitalisations.Remove(detail_hospitalisationCreate);
                                dc.SaveChanges();
                                reponse.statusCode = 201;
                                reponse.message = "L'hospitalisation n'a pas  été ajouté";
                                reponse.resultat = false;
                            }

                        }
                        else
                        {
                            dc.Hospitalisations.Remove(hospitalisationCreate);
                            dc.SaveChanges();
                            reponse.statusCode = 201;
                            reponse.message = "L'hospitalisation n'a pas  été ajouté";
                            reponse.resultat = false;
                        }



                    }
                    else
                    {
                        dc.Hospitalisations.Remove(hospitalisationCreate);
                        dc.SaveChanges();
                        reponse.statusCode = 201;
                        reponse.message = "L'hospitalisation n'a pas  été ajouté";
                        reponse.resultat = false;
                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'ajout de l'hospitalisation ";
                    reponse.resultat = false;
                }

            }
            return reponse;
        }

        Reponse IPatientRepository.Consultation(string Observation, string Note, string ConID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (ConID != null)
                    {
                        long consta = Convert.ToInt64(ConID);
                        var confist = dc.Consultations.Where(a => a.consultationID == consta).FirstOrDefault();
                        if (confist != null)
                        {
                            confist.constant = Note;
                            confist.observation = Observation;
                            dc.Entry(confist).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            reponse.message = "La consultation a été modifiée ";
                            reponse.statusCode = 200;
                            reponse.resultat = confist;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "La consultation n'existe pas ";
                            reponse.resultat = null;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "La consultation n'existe pas ";
                        reponse.resultat = null;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la modification de la consultation ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.SelectOrdo(long? userConnectedID, string ordo)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (ordo != null)
                    {
                        long ta = Convert.ToInt64(ordo);
                        var techno = dc.Detail_ordonnances.Where(a => a.ordonnanceID == ta).FirstOrDefault();
                        if (techno != null)
                        {

                            var selectOrdo = new SelectOrdo
                            {
                                consultationID = techno.consultationID,
                                detailsOrdos = dc.Detail_ordonnances.Where(a => a.ordonnanceID == ta).ToList(),
                                nomCompletPatient = dc.Patients.Where(a => a.patientID == techno.patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault(),
                                NumberOrdo = ta,
                                userConnected = userConnectedID
                            };
                            reponse.statusCode = 200;
                            reponse.message = "L'ordonnance a été retrouvé ";
                            reponse.resultat = selectOrdo;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "L'ordonnance n'a pas été retrouvé ";
                            reponse.resultat = null;

                        }

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "L'ordonnance n'a pas été retrouvé ";
                        reponse.resultat = null;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de l'ordonnance ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.PrintPrescription(long? userConnectedID, long? pres)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (pres != null && userConnectedID != null)
                    {

                        var info = Convert.ToInt64(pres);
                        var list = dc.Detail_ordonnances.Where(a => a.ordonnanceID == info).ToList();
                        var numeroConsultation = dc.Ordonnances.Where(a => a.ordonnanceID == info).Select(a => a.consultationID).FirstOrDefault();
                        if (numeroConsultation != null)
                        {
                            var date = dc.Consultations.Where(a => a.consultationID == numeroConsultation).Select(a => a.date_consultation).FirstOrDefault();
                            var idPatient = dc.Consultations.Where(a => a.consultationID == numeroConsultation).Select(a => a.patientID).FirstOrDefault();
                            var patient = dc.Patients.Where(a => a.patientID == idPatient).Select(a => a.nom + "-" + a.prenom).FirstOrDefault();
                            var id_user = dc.Consultations.Where(a => a.consultationID == numeroConsultation).Select(a => a.docteurID).FirstOrDefault();
                            var id_structure = dc.Consultations.Where(a => a.consultationID == numeroConsultation).Select(a => a.structureID).FirstOrDefault();
                            var structure = dc.Structures.Where(a => a.structureID == id_structure).FirstOrDefault();
                            if (structure != null)
                            {
                                var user = dc.Utilisateurs.Where(a => a.utilisateurID == id_user && a.structureID == id_structure).FirstOrDefault();

                                var printPaiement = new PrintPrescription
                                {
                                    adresse_structure = structure.adresse,
                                    Date = DateTime.Now,
                                    email_structure = structure.email,
                                    FullNamePatient = patient,
                                    No = idPatient,
                                    nom_structure = structure.nom,
                                    NOrd = info,
                                    numeroConsultation = numeroConsultation,
                                    telephone_structure = structure.telephone,
                                    detail_ordonnances = list,
                                    utilisateur = user


                                };

                                reponse.statusCode = 200;
                                reponse.resultat = printPaiement;


                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "La structure  n'existe pas ";
                                reponse.resultat = new PrintPrescription();

                            }

                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Le reçu ou le compte connecté n'existe plus ";
                        reponse.resultat = new PrintPrescription();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du reçu ";
                    reponse.resultat = new PrintPrescription();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.AnalyseFirst(long? userConnectedID, string analyseID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (userConnectedID != null && analyseID != null)
                    {
                        long ta = Convert.ToInt64(analyseID);
                        var patron = dc.Analyses.Where(a => a.analyseID == ta).Select(a => a.consultationID).FirstOrDefault();
                        if (patron != null)
                        {
                            var tc = dc.Consultations.Where(a => a.consultationID == patron).Select(a => a.patientID).FirstOrDefault();
                            if (tc != null)
                            {
                                var analyseList = new AnalyseFirst
                                {
                                    detail_analyses = dc.Detail_analyses.Where(a => a.analyseID == ta).ToList(),
                                    Join = patron,
                                    liste_analyses = dc.Liste_analyses.ToList(),
                                    nomCompletPatient = dc.Patients.Where(a => a.patientID == tc).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault(),
                                    NumberAnalyse = ta,
                                    suis = userConnectedID,
                                    consultationID = patron
                                };
                                reponse.statusCode = 200;
                                reponse.message = "Ce patient n'existe pas ";
                                reponse.resultat = analyseList;
                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Cette consultation n'existe pas ";
                                reponse.resultat = new AnalyseFirst();

                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce patient n'existe pas ";
                            reponse.resultat = new AnalyseFirst();
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce compte ou l'analyse n'existe plus ";
                        reponse.resultat = new AnalyseFirst();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = new AnalyseFirst();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.PrintAnalyse(long? userConnectedID, long? pres)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (pres != null && userConnectedID != null)
                    {

                        var info = Convert.ToInt64(pres);
                        var numeroConsultation = dc.Ordonnances.Where(a => a.ordonnanceID == info).Select(a => a.consultationID).FirstOrDefault();
                        var list = dc.Detail_analyses.Where(a => a.analyseID == pres).ToList();
                        var anayleToPrint = dc.Analyses.Where(a => a.analyseID == pres).FirstOrDefault();

                        if (anayleToPrint != null)
                        {
                            var listany = dc.Liste_analyses.ToList();
                            var consultation = dc.Consultations.Where(a => a.consultationID == anayleToPrint.consultationID).FirstOrDefault();
                            if (consultation != null)
                            {
                                var date = dc.Consultations.Where(a => a.consultationID == consultation.consultationID).Select(a => a.date_consultation).FirstOrDefault();
                                var idPatient = dc.Consultations.Where(a => a.consultationID == consultation.consultationID).Select(a => a.patientID).FirstOrDefault();
                                var patient = dc.Patients.Where(a => a.patientID == idPatient).FirstOrDefault();
                                if (patient != null)
                                {
                                    var structure = dc.Structures.Where(a => a.structureID == patient.structureID).FirstOrDefault();
                                    if (structure != null)
                                    {
                                        var docteur = dc.Utilisateurs.Where(a => a.utilisateurID == consultation.docteurID && a.structureID == anayleToPrint.structureID).FirstOrDefault();
                                        if (docteur != null)
                                        {
                                            var analysePrint = new PrintAnalyse
                                            {
                                                adresse_patient = patient.adresse,
                                                adresse_structure = structure.adresse,
                                                date_naissance_patient = patient.date_naissance.ToString(),
                                                detail_analyses = list,
                                                email_structure = structure.email,
                                                FullNameDocteur = docteur.prenom + " " + docteur.nom,
                                                FullNamePatient = patient.prenom + " " + patient.nom,
                                                id_patient = patient.patientID,
                                                liste_analyses = listany,
                                                nom_structure = structure.nom,
                                                NumeroConsultation = consultation.consultationID,
                                                telephone_structure = structure.telephone,
                                                titre_docteur = docteur.titre

                                            };
                                            reponse.statusCode = 200;
                                            reponse.message = "L'analyse n'existe plus ";
                                            reponse.resultat = analysePrint;


                                        }
                                        else
                                        {
                                            reponse.statusCode = 201;
                                            reponse.message = "Le docteur n'existe plus ";
                                            reponse.resultat = new PrintAnalyse();


                                        }


                                    }
                                    else
                                    {
                                        reponse.statusCode = 201;
                                        reponse.message = "La structure n'existe plus ";
                                        reponse.resultat = new PrintAnalyse();

                                    }


                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "Le patient n'existe plus ";
                                    reponse.resultat = new PrintAnalyse();

                                }


                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "La consultation n'existe plus ";
                                reponse.resultat = new PrintAnalyse();

                            }
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "L'analyse n'existe plus ";
                            reponse.resultat = new PrintAnalyse();

                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "L'analyse ou le compte connecté n'existe plus ";
                        reponse.resultat = new PrintPrescription();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du reçu ";
                    reponse.resultat = new PrintPrescription();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.HoFirst(long? userConnectedID, string hostpi)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (userConnectedID != null && hostpi != null)
                    {
                        long ta = Convert.ToInt64(hostpi);
                        var techno = dc.Hospitalisations.Where(a => a.hospitalisationID == ta).FirstOrDefault();
                        if (techno != null)
                        {
                            var ct = dc.Consultations.Where(a => a.consultationID == techno.consultationID).Select(a => a.patientID).FirstOrDefault();

                            if (ct != null)
                            {
                                var hoFirst = new HoFirst
                                {
                                    FullName = dc.Patients.Where(a => a.patientID == ct).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault(),
                                    suis = userConnectedID,
                                    techno = techno

                                };
                                reponse.statusCode = 200;
                                reponse.resultat = hoFirst;

                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Ce patient n'existe pas ";
                                reponse.resultat = new HoFirst();

                            }




                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cette hospitalisation n'existe pas ";
                            reponse.resultat = new HoFirst();
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce compte ou l'hospitalisation n'existe plus ";
                        reponse.resultat = new HoFirst();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = new HoFirst();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.AnalyseList(long? userConnectedID, long? No, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var patient = dc.Patients.Where(a => a.patientID == No).FirstOrDefault();
                        if (patient != null)
                        {
                            var listUsers = dc.Utilisateurs.Where(a => a.structureID == patient.structureID).ToList();
                            var cons = dc.Consultations.Where(a => a.patientID == patient.patientID).OrderByDescending(a => a.consultationID).ToList();
                            var list = dc.Analyses.Where(a => patient.status == true && a.analyseID.ToString().StartsWith(searching)
                                                          || patient.status == true && a.date_analyse.ToString().Contains(searching)
                                                          || patient.status == true && searching == null).ToList();
                            var ordlst = dc.Detail_analyses.ToList();
                            var lstAnalyse = (from ta in cons
                                              join tb in list on ta.consultationID equals tb.consultationID
                                              select new Analyse
                                              {
                                                  analyseID = tb.analyseID,
                                                  date_analyse = tb.date_analyse
                                              }).ToPagedList(pageIndex, pageSize);

                            // RECUPERATION DES SUIVIS RECU A TRAVERS LE PARTAGE DU DOSSIER MEDICAL ENVOYER PAR UN AUTRE DOCTEUR INTERNE
                            var myHealthFolderID = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == patient.structureID).Select(a => a.partageInfoID).FirstOrDefault();
                            if (myHealthFolderID > 0)
                            {
                                var med_doc = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == patient.structureID).FirstOrDefault();
                                if (med_doc != null)
                                {

                                    var listConsultationForPatientExterne = dc.Consultations.Where(a => a.patientID == med_doc.patientID && a.structureID == med_doc.structre_secondaireID).ToList();
                                    var analyseOfConsultationExterne = (med_doc.id_choisi != null) ? (from ta in listConsultationForPatientExterne
                                                                                                      join tb in list.Where(g => g.analyseID == med_doc.id_choisi).ToList() on ta.consultationID equals tb.consultationID
                                                                                                      select new Analyse
                                                                                                      {
                                                                                                          analyseID = tb.analyseID,
                                                                                                          date_analyse = tb.date_analyse
                                                                                                      }).ToPagedList(pageIndex, pageSize)
                                                                                               : (from ta in listConsultationForPatientExterne
                                                                                                  join tb in list on ta.consultationID equals tb.consultationID
                                                                                                  select new Analyse
                                                                                                  {
                                                                                                      analyseID = tb.analyseID,
                                                                                                      date_analyse = tb.date_analyse
                                                                                                  }).ToPagedList(pageIndex, pageSize);
                                    var tuple = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(lstAnalyse, ordlst, listUsers, analyseOfConsultationExterne);

                                    reponse.statusCode = 200;
                                    reponse.message = "Liste des analyses de " + dc.Patients.Where(a => a.patientID == patient.patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                                    reponse.resultat = tuple;
                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "Une erreur interne est survenue 1 ";
                                    reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);

                                }
                            }
                            else
                            {
                                reponse.statusCode = 200;
                                reponse.message = "Liste des analyses de " + dc.Patients.Where(a => a.patientID == patient.patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                                reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(lstAnalyse, ordlst, listUsers, lstAnalyse);
                            }
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Le patient n'existe pas ";
                            reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);

                        }
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Le patient n'existe pas ";
                        reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);

                    }

                }

                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de l'ordonnance ";
                    reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.HospitalisationList(long? userConnectedID, long? No, int? page, string searching, DateTime? dateEntree, DateTime? dateSortie)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var patient = dc.Patients.Where(a => a.patientID == No).FirstOrDefault();
                        if (patient != null)
                        {
                            var listUsers = dc.Utilisateurs.Where(a => a.structureID == patient.structureID).ToList();
                            var cons = dc.Consultations.Where(a => a.patientID == patient.patientID).OrderByDescending(a => a.consultationID).ToList();
                            var list = dc.Hospitalisations.Where(a => patient.status == true && a.hospitalisationID.ToString().StartsWith(searching)
                                                                  || patient.status == true && a.date_sortie == dateSortie && a.date_entree == dateEntree && a.hospitalisationID.ToString().StartsWith(searching)
                                                                  || patient.status == true && a.date_sortie == dateSortie && a.hospitalisationID.ToString().StartsWith(searching)
                                                                  || patient.status == true && a.date_entree == dateEntree && a.hospitalisationID.ToString().StartsWith(searching)
                                                                  || patient.status == true && searching == null && dateSortie == null && dateEntree == null).ToList();



                            var ordlst = dc.Detail_hospitalisations.ToList();
                            var lstHospi = (from ta in cons
                                            join tb in list on ta.consultationID equals tb.consultationID
                                            select new Hospitalisation
                                            {
                                                hospitalisationID = tb.hospitalisationID,
                                                consultationID = tb.consultationID,
                                                date_entree = tb.date_entree,
                                                date_sortie = tb.date_sortie,
                                                numero_lit = tb.numero_lit,
                                                numero_salle = tb.numero_salle
                                            }).ToPagedList(pageIndex, pageSize);

                            // RECUPERATION DES HOSPITALISATION RECU A TRAVERS LE PARTAGE DU DOSSIER MEDICAL ENVOYER PAR UN AUTRE DOCTEUR INTERNE
                            var myHealthFolderID = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == patient.structureID).Select(a => a.partageInfoID).FirstOrDefault();
                            if (myHealthFolderID > 0)
                            {
                                var med_doc = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == patient.structureID).FirstOrDefault();
                                if (med_doc != null)
                                {

                                    var listConsultationForPatientExterne = dc.Consultations.Where(a => a.patientID == med_doc.patientID && a.structureID == med_doc.structre_secondaireID).ToList();
                                    var analyseOfConsultationExterne = (med_doc.id_choisi != null) ? (from ta in listConsultationForPatientExterne
                                                                                                      join tb in list.Where(g => g.hospitalisationID == med_doc.id_choisi).ToList() on ta.consultationID equals tb.consultationID
                                                                                                      select new Analyse
                                                                                                      {
                                                                                                          analyseID = tb.hospitalisationID,
                                                                                                          date_analyse = tb.date_entree
                                                                                                      }).ToPagedList(pageIndex, pageSize)
                                                                                               : (from ta in listConsultationForPatientExterne
                                                                                                  join tb in list on ta.consultationID equals tb.consultationID
                                                                                                  select new Analyse
                                                                                                  {
                                                                                                      analyseID = tb.hospitalisationID,
                                                                                                      date_analyse = tb.date_entree
                                                                                                  }).ToPagedList(pageIndex, pageSize);
                                    var tuple = new Tuple<IPagedList<Hospitalisation>, List<Detail_hospitalisation>, List<Utilisateur>, IPagedList<Analyse>>(lstHospi, ordlst, listUsers, analyseOfConsultationExterne);

                                    reponse.statusCode = 200;
                                    reponse.message = "Liste des analyses de " + dc.Patients.Where(a => a.patientID == patient.patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                                    reponse.resultat = tuple;
                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "Une erreur interne est survenue 1 ";
                                    reponse.resultat = new Tuple<IPagedList<Analyse>, List<Detail_analyse>, List<Utilisateur>, IPagedList<Analyse>>(null, null, null, null);

                                }
                            }
                            else
                            {
                                reponse.statusCode = 200;
                                reponse.message = "Liste des hospitalisation de " + dc.Patients.Where(a => a.patientID == patient.patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();
                                reponse.resultat = new Tuple<IPagedList<Hospitalisation>, List<Detail_hospitalisation>, List<Utilisateur>, IPagedList<Hospitalisation>>(lstHospi, ordlst, listUsers, lstHospi);
                            }
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Le patient n'existe pas ";
                            reponse.resultat = new Tuple<IPagedList<Hospitalisation>, List<Detail_hospitalisation>, List<Utilisateur>, IPagedList<Hospitalisation>>(null, null, null, null);

                        }
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Le patient n'existe pas ";
                        reponse.resultat = new Tuple<IPagedList<Hospitalisation>, List<Detail_hospitalisation>, List<Utilisateur>, IPagedList<Hospitalisation>>(null, null, null, null);

                    }

                }

                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de l'hospitalisation ";
                    reponse.resultat = new Tuple<IPagedList<Hospitalisation>, List<Detail_hospitalisation>, List<Utilisateur>, IPagedList<Hospitalisation>>(null, null, null, null);
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.PrintResultat(string patientID, long? analyseID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (patientID != null && analyseID != null)
                    {
                        long Idpatient = Convert.ToInt64(patientID);
                        var patient = dc.Patients.Where(a => a.patientID == Idpatient).FirstOrDefault();
                        if (patient != null)
                        {
                            var idDocteur = dc.Consultations.Where(a => a.patientID == Idpatient).Select(a => a.docteurID).FirstOrDefault();
                            var structure = dc.Structures.Where(a => a.structureID == patient.structureID).FirstOrDefault();

                            if (idDocteur > 0 && structure != null)
                            {
                                var id_analyse = dc.Detail_analyses.Where(a => a.analyseID == analyseID).Select(a => a.analyseID).FirstOrDefault();
                                var docteur = dc.Utilisateurs.Where(a => a.utilisateurID == idDocteur).FirstOrDefault();
                                if (id_analyse > 0 && docteur != null)
                                {
                                    var resultat = dc.Resultats.Where(a => a.analyseID == analyseID).FirstOrDefault();
                                    if (resultat != null)
                                    {
                                        var printResultat = new PrintResultat
                                        {
                                            DatePatient = patient.date_naissance,
                                            DescriptionCommentaire = resultat.commentaire,
                                            DescriptionResultat = resultat.description,
                                            email_adresse = structure.email,
                                            No = Idpatient,
                                            NomAnalyse = dc.Liste_analyses.Where(a => a.liste_analyseID == id_analyse).Select(a => a.nom).FirstOrDefault(),
                                            NomComplet = patient.prenom + " " + patient.nom,
                                            NomCompletDocteur = docteur.prenom + "  " + docteur.nom,
                                            No_dossier = analyseID,
                                            structure_adresse = structure.adresse,
                                            TitreDocteur = docteur.titre,
                                            telephone_adresse = structure.telephone,
                                            NomStructure = structure.nom

                                        };
                                        reponse.statusCode = 200;
                                        reponse.resultat = printResultat;
                                    }
                                    else
                                    {
                                        reponse.statusCode = 201;
                                        reponse.message = "Ce resultat n'existe pas ";
                                        reponse.resultat = new PrintResultat();

                                    }




                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "Le traitant ou l'analyse n'existe pas ";
                                    reponse.resultat = new PrintResultat();

                                }

                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Le traitant ou la structure n'existe pas ";
                                reponse.resultat = new PrintResultat();

                            }


                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce patient  n'existe pas ";
                            reponse.resultat = new PrintResultat();

                        }



                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient ou l'analyse n'existe pas ";
                        reponse.resultat = new PrintResultat();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'archivage ";
                    reponse.resultat = new PrintResultat();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.ConfiguerPolitiqueAccess(long? structureID, long? userConnectedID, long? idPatient, long? DateLimiteLong, long? id_choisi, string Type_dossier, string Phone)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    long phForSendMessage = Convert.ToInt64(Phone.Remove(0, 3));
                    var getUserId = dc.Utilisateurs.Where(a => a.telephone == Phone).Select(a => a.utilisateurID).FirstOrDefault();
                    // CASE IF VISITOR IS SYSTEME USER 
                    if (getUserId > 0)
                    {
                        var getUser = dc.Utilisateurs.Where(a => a.telephone == Phone).FirstOrDefault();
                        if (getUser != null)
                        {
                            if (getUser.status == false)
                            {
                                var password = new Random().Next(1000, 9999).ToString();
                                getUser.status = true;
                                getUser.pwd = BCrypt.Net.BCrypt.HashPassword(password);
                                getUser.validite_compte_temporaire = true;
                                dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                dc.SaveChanges();

                                var sizeTransfertPatientStart = dc.PartageInfos.Count();
                                var partageInfo = new PartageInfo
                                {
                                    date_partageInfo = DateTime.Now,
                                    docteur_primaireID = userConnectedID,
                                    docteur_secondaireID = getUser.utilisateurID,
                                    duree_partage = DateLimiteLong,
                                    id_choisi = id_choisi,
                                    patientID = idPatient,
                                    status = true,
                                    structre_secondaireID = getUser.structureID,
                                    structure_primaireID = structureID,
                                    type_dossier = Type_dossier

                                };
                                var currentTransfertPatient = dc.PartageInfos.Add(partageInfo);
                                dc.SaveChanges();
                                var sizeTransfertPatientEnd = dc.PartageInfos.Count();

                                if (sizeTransfertPatientEnd > sizeTransfertPatientStart)
                                {
                                    //Fayen.Where(a => a.thank == THANKS).Select(a => a.you).FirstOrDefault();
                                    string message = "Bonjour cher(e) invité(e)  veuillez vous connecter en entrant le mot de passe suivant : " + password + " sur  http://bullz.net , nous vous souhaitons une excellente journee, Equipe HOSTO";
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://41.214.66.206:65/api/SendSMS?tel=" + phForSendMessage + "&sms=" + message);
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                    {

                                        string jsont = "{\"tel\":\"" + phForSendMessage + "\"," +
                                                   "\"sms\":\"" + message + "}";

                                        streamWriter.WriteLine();
                                    }
                                    try
                                    {
                                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                        if (httpResponse.StatusCode.GetHashCode().Equals(200))
                                        {
                                            getUser.validite_compte_temporaire = true;
                                            dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                            dc.SaveChanges();
                                            reponse.statusCode = 200;
                                            reponse.message = "La déléguation a été éffectué avec succès  ";
                                            reponse.resultat = null;

                                        }
                                        else
                                        {
                                            getUser.status = false;
                                            getUser.validite_compte_temporaire = false;
                                            dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                            dc.SaveChanges();
                                            dc.PartageInfos.Remove(currentTransfertPatient);
                                            dc.SaveChanges();
                                            reponse.statusCode = 201;
                                            reponse.message = "Envoi SMS refusé  ";
                                            reponse.resultat = null;


                                        }
                                    }
                                    catch (WebException)
                                    {
                                        getUser.status = false;
                                        getUser.validite_compte_temporaire = false;
                                        dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                        dc.SaveChanges();
                                        dc.PartageInfos.Remove(currentTransfertPatient);
                                        dc.SaveChanges();
                                        reponse.statusCode = 201;
                                        reponse.message = "Envoi SMS refusé  ";
                                        reponse.resultat = null;

                                    }

                                }
                                else
                                {
                                    getUser.status = false;
                                    getUser.validite_compte_temporaire = false;
                                    dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                    dc.SaveChanges();
                                    dc.PartageInfos.Remove(currentTransfertPatient);
                                    dc.SaveChanges();
                                    reponse.statusCode = 201;
                                    reponse.message = "Une erreur majeur est survenue ,veuillez patienter svp";
                                    reponse.resultat = null;


                                }
                            }
                            else
                            {

                                var sizeTransfertPatientStart = dc.PartageInfos.Count();
                                var partageInfo = new PartageInfo
                                {
                                    date_partageInfo = DateTime.Now,
                                    docteur_secondaireID = getUserId,
                                    duree_partage = DateLimiteLong,
                                    id_choisi = id_choisi,
                                    patientID = idPatient,
                                    status = true,
                                    structre_secondaireID = getUser.structureID,
                                    structure_primaireID = structureID,
                                    type_dossier = Type_dossier

                                };
                                var currentTransfertPatient = dc.PartageInfos.Add(partageInfo);
                                dc.SaveChanges();
                                var sizeTransfertPatientEnd = dc.PartageInfos.Count();
                                if (sizeTransfertPatientEnd > sizeTransfertPatientStart)
                                {
                                    getUser.validite_compte_temporaire = true;
                                    dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                    dc.SaveChanges();
                                    reponse.statusCode = 200;
                                    reponse.message = "La déléguation a été éffectué avec succès";
                                    reponse.resultat = null;


                                }
                                else
                                {
                                    dc.PartageInfos.Remove(currentTransfertPatient);
                                    dc.SaveChanges();
                                    reponse.statusCode = 201;
                                    reponse.message = "Une erreur majeur est survenue ,veuillez patienter svp";
                                    reponse.resultat = null;
                                }
                            }
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet Utilisateur n'existe plus ";
                            reponse.resultat = null;

                        }



                    }
                    // CASE IF VISITEUR ISN'T SYSTEME USER(SIMPLE VISITOR)
                    else
                    {
                        var password = new Random().Next(1000, 9999).ToString();
                        var sizeUtilisateurtStart = dc.Utilisateurs.Count();
                        var utl = new Utilisateur
                        {
                            structureID = structureID,
                            status = true,
                            date_creation = DateTime.Now,
                            nom = "Visiteur",
                            prenom = "Docteur",
                            pwd = BCrypt.Net.BCrypt.HashPassword(password),
                            role = "Utilisateur",
                            telephone = Phone,
                            titre = "Docteur"
                        };
                        var currentUtilisateurTemporaire = dc.Utilisateurs.Add(utl);
                        dc.SaveChanges();
                        var sizeUtilisateurtEnd = dc.Utilisateurs.Count();
                        if (sizeUtilisateurtEnd <= sizeUtilisateurtStart)
                        {
                            dc.Utilisateurs.Remove(currentUtilisateurTemporaire);
                            dc.SaveChanges();
                            reponse.statusCode = 201;
                            reponse.message = "Une erreur majeur est survenue ,veuillez patienter svp";
                            reponse.resultat = null;



                        }
                        else
                        {
                            var sizeTransfertPatientStart = dc.PartageInfos.Count();
                            var getUser = dc.Utilisateurs.Where(a => a.telephone == Phone && a.status == true).FirstOrDefault();
                            if (getUser != null)
                            {
                                var partageInfo = new PartageInfo
                                {
                                    date_partageInfo = DateTime.Now,
                                    docteur_secondaireID = currentUtilisateurTemporaire.utilisateurID,
                                    duree_partage = DateLimiteLong,
                                    id_choisi = id_choisi,
                                    patientID = idPatient,
                                    status = true,
                                    structre_secondaireID = getUser.structureID,
                                    structure_primaireID = structureID,
                                    type_dossier = Type_dossier

                                };
                                var currentTransfertPatient = dc.PartageInfos.Add(partageInfo);
                                dc.SaveChanges();
                                var sizeTransfertPatientEnd = dc.PartageInfos.Count();

                                if (sizeTransfertPatientEnd > sizeTransfertPatientStart)
                                {
                                    //Fayen.Where(a => a.thank == THANKS).Select(a => a.you).FirstOrDefault();
                                    string message = "Bonjour cher(e) invité(e)  veuillez vous connecter en entrant le mot de passe suivant : " + password + " sur  http://bullz.net , nous vous souhaitons une excellente journee, Equipe HOSTO";
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://41.214.66.206:65/api/SendSMS?tel=" + phForSendMessage + "&sms=" + message);
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                    {

                                        string jsont = "{\"tel\":\"" + phForSendMessage + "\"," +
                                                   "\"sms\":\"" + message + "}";

                                        streamWriter.WriteLine();
                                    }
                                    try
                                    {
                                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                        if (httpResponse.StatusCode.GetHashCode().Equals(200))
                                        {
                                            currentUtilisateurTemporaire.validite_compte_temporaire = true;
                                            dc.Entry(getUser).State = System.Data.Entity.EntityState.Modified;
                                            dc.SaveChanges();
                                            reponse.statusCode = 200;
                                            reponse.message = "La déléguation a été éffectué avec succès ";
                                            reponse.resultat = null;


                                        }
                                        else
                                        {
                                            dc.Utilisateurs.Remove(currentUtilisateurTemporaire);
                                            dc.SaveChanges();
                                            dc.PartageInfos.Remove(currentTransfertPatient);
                                            dc.SaveChanges();
                                            reponse.statusCode = 201;
                                            reponse.message = "Envoi SMS refusé";
                                            reponse.resultat = null;
                                        }
                                    }
                                    catch (WebException)
                                    {
                                        dc.Utilisateurs.Remove(currentUtilisateurTemporaire);
                                        dc.SaveChanges();
                                        dc.PartageInfos.Remove(currentTransfertPatient);
                                        dc.SaveChanges();
                                        reponse.statusCode = 201;
                                        reponse.message = "Envoi SMS refusé";
                                        reponse.resultat = null;
                                    }

                                }
                                else
                                {
                                    dc.PartageInfos.Remove(currentTransfertPatient);
                                    dc.SaveChanges();
                                    reponse.statusCode = 201;
                                    reponse.message = "Une erreur majeur est survenue ,veuillez patienter svp ";
                                    reponse.resultat = null;
                                }

                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Utilisateur n'existe pas  ";
                                reponse.resultat = null;
                            }

                        }

                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors du partage ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.VoirObservationSuivi(long? idPatient, long? hospitalisationID)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (idPatient != null && hospitalisationID != null)
                    {
                        var hospitalisation = dc.Hospitalisations.Where(a => a.hospitalisationID == hospitalisationID).FirstOrDefault();
                        var id_suivi = dc.Suivis.Where(a => a.hospitalisationID == hospitalisationID).Select(a => a.suiviID).FirstOrDefault();
                        if (hospitalisation != null && id_suivi > 0)
                        {
                            var listeObservationItemSuivi = dc.observationItemSuivis.Where(a => a.suiviID == id_suivi).OrderByDescending(a => a.observationItemSuiviID).ToList();
                            var voirObservationSuivi = new VoirObservationSuivi
                            {
                                hospitalisation = hospitalisation,
                                IdPatient = idPatient,
                                listeObservationItemSuivis = listeObservationItemSuivi,
                                NoSuivi = id_suivi
                            };
                            reponse.statusCode = 200;
                            reponse.resultat = voirObservationSuivi;

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "L'hospitalisation ou la suivie n'existe pas ";
                            reponse.resultat = new VoirObservationSuivi();

                        }

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Le patient ou la suivie n'existe pas  ";
                        reponse.resultat = new VoirObservationSuivi();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = new VoirObservationSuivi();
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.AddObservationToSuivi(string No, string observation)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null)
                    {
                        var itemObservation = new ObservationItemSuivi
                        {
                            date_observationItemSuivi = DateTime.Now,
                            observation = observation,
                            suiviID = Convert.ToInt32(No)

                        };
                        dc.observationItemSuivis.Add(itemObservation);
                        dc.SaveChanges();
                        reponse.resultat = true;
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette suivie n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'ajout d'observation ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.mettreFinHospitalisation(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var hospi = db.Hospitalisations.Where(a => a.hospitalisationID == id).FirstOrDefault();
                        if (hospi != null)
                        {
                            hospi.date_sortie = DateTime.Now;
                            db.Entry(hospi).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La cloture de l'hospitalisation a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cette hospitalisation n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette hospitalisation n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la cloture de cette  hospitalisation";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.OrdonnanceList(long? structureID, long? userConnectedID, long? patientID, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (patientID != null && structureID != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                        var list = dc.Ordonnances.Where(a => a.structureID == structureID).ToList();
                        var listUsers = dc.Utilisateurs.Where(a => a.structureID == structureID).ToList();
                        var detail_ordonnance = dc.Detail_ordonnances.ToList();

                        // RECUPERATION DES SUIVIS RECU A TRAVERS LE PARTAGE DU DOSSIER MEDICAL ENVOYER PAR UN AUTRE DOCTEUR INTERNE
                        var myHealthFolderID = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == structureID && p.type_dossier == "ORDONNANCE").Select(a => a.partageInfoID).FirstOrDefault();
                        if (myHealthFolderID > 0)
                        {
                            var med_doc = dc.PartageInfos.Where(p => p.status == true && p.docteur_secondaireID == userConnectedID && p.structre_secondaireID == structureID && p.type_dossier == "ORDONNANCE").FirstOrDefault();
                            var listConsultationForPatientExterne = dc.Consultations.Where(a => a.patientID == med_doc.patientID && a.structureID == med_doc.structre_secondaireID).ToList();
                            var ordoOfConsultationExterne = (med_doc.id_choisi != null) ? (from ta in listConsultationForPatientExterne
                                                                                           join tb in list.Where(d => d.ordonnanceID == med_doc.id_choisi).ToList() on ta.consultationID equals tb.consultationID
                                                                                           select new Ordonnance
                                                                                           {
                                                                                               ordonnanceID = tb.ordonnanceID,
                                                                                               date_ordonnance = tb.date_ordonnance
                                                                                           }).ToList().ToPagedList(pageIndex, pageSize)
                                                                                       : (from ta in listConsultationForPatientExterne
                                                                                          join tb in list on ta.consultationID equals tb.consultationID
                                                                                          select new Ordonnance
                                                                                          {
                                                                                              ordonnanceID = tb.ordonnanceID,
                                                                                              date_ordonnance = tb.date_ordonnance
                                                                                          }).ToList().ToPagedList(pageIndex, pageSize);

                            var lstOrdo = (string.IsNullOrEmpty(searching)) ? ordoOfConsultationExterne.ToPagedList(pageIndex, pageSize)
                                                                               : ordoOfConsultationExterne.Where(a => a.ordonnanceID.ToString().StartsWith(searching)
                                                                               || a.date_ordonnance.ToString().Contains(searching)).ToPagedList(pageIndex, pageSize);


                            var tuple = new Tuple<IPagedList<Ordonnance>, List<Detail_ordonnance>, List<Utilisateur>, IPagedList<Ordonnance>>(lstOrdo, detail_ordonnance, listUsers, lstOrdo);


                        }
                        else
                        {
                            var cons = dc.Consultations.Where(a => a.patientID == patientID).OrderByDescending(a => a.consultationID).ToList();
                            var final = (from ta in cons
                                         join tb in list on ta.consultationID equals tb.consultationID
                                         select new Ordonnance
                                         {
                                             ordonnanceID = tb.ordonnanceID,
                                             date_ordonnance = tb.date_ordonnance
                                         }).ToList();

                            var lstOrdo = (searching != null) ? final.Where(a => a.ordonnanceID.ToString().StartsWith(searching)
                                                                                   || a.date_ordonnance.ToString().Contains(searching)).ToPagedList(pageIndex, pageSize)
                                                                                   : final.ToPagedList(pageIndex, pageSize);

                            var ordonnances = new OrdonnanceList
                            {
                                detail_ordonnances = detail_ordonnance,
                                FullNamePatient = dc.Patients.Where(a => a.patientID == patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault(),
                                ordonnances = lstOrdo,
                                ordonnancesExtenes = lstOrdo,
                                searching = searching,
                                utilisateurs = listUsers

                            };

                            reponse.statusCode = 200;
                            reponse.message = "Liste des ordonnances de " + dc.Patients.Where(a => a.patientID == patientID).Select(a => a.prenom + " " + " " + a.nom).FirstOrDefault();

                            reponse.resultat = ordonnances;

                        }

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "La structure ou le patient n'existe pas ";
                        reponse.resultat = new OrdonnanceList();
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = new OrdonnanceList();
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.EchangePatientEntreDocteur(long? structureID, string No, string DeDocteur, string ADocteur)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null && structureID != null && DeDocteur != null && ADocteur != null)
                    {
                        var docteurPrimaireID = Convert.ToInt64(DeDocteur);
                        var docteurSeconadireID = Convert.ToInt64(ADocteur);
                        var patienID = Convert.ToInt64(No);

                        var sizeTableTransfertPatientStart = dc.PartageInfos.Count();

                        var patientTransfer = new PartageInfo
                        {
                            date_partageInfo = DateTime.Now,
                            docteur_primaireID = docteurPrimaireID,
                            docteur_secondaireID = docteurSeconadireID,
                            patientID = patienID,
                            status = true,
                            structure_primaireID = structureID
                        };

                        var obj = dc.PartageInfos.Add(patientTransfer);
                        dc.SaveChanges();
                        var sizeTableTransfertPatientEnd = dc.PartageInfos.Count();

                        if (sizeTableTransfertPatientEnd > sizeTableTransfertPatientStart)
                        {
                            var consultation = dc.Consultations.Where(a => a.patientID == patienID && a.structureID == structureID && a.docteurID == docteurPrimaireID).FirstOrDefault();
                            consultation.docteurID = docteurSeconadireID;
                            dc.Entry(consultation).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "Le transfert a été bien effectué";

                        }
                        else
                        {
                            dc.PartageInfos.Remove(obj);
                            dc.SaveChanges();
                            reponse.statusCode = 201;
                            reponse.message = "Une erreur est survenue lors du transfert du patient ";
                        }



                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce patient ou la structure ou le docteur n'existe pas ";
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du patient ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.ListTransferts(long? structureID, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (structureID != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var listTransfertFiltered = dc.PartageInfos.Where(c => c.structure_primaireID == structureID && c.status == true && c.docteur_primaireID != null)
                                                                                     .Select(a =>
                                                                                      new PatientPartageInfos
                                                                                      {
                                                                                          nomCompletpatient = dc.Patients.Where(b => b.patientID == a.patientID).Select(b => b.nom + " " + " " + b.prenom).FirstOrDefault(),
                                                                                          nomCpmletDocteurPrimaire = dc.Utilisateurs.Where(b => b.utilisateurID == a.docteur_primaireID).Select(c => c.nom + " " + " " + c.prenom + "|" + " " + c.titre).FirstOrDefault(),
                                                                                          nomCpmletDocteurSecondaire = dc.Utilisateurs.Where(b => b.utilisateurID == a.docteur_secondaireID).Select(b => b.nom + " " + " " + b.prenom + "|" + " " + b.titre).FirstOrDefault(),
                                                                                          partageInfoDate = a.date_partageInfo,
                                                                                          partageInfoID = a.partageInfoID
                                                                                      }).ToList();

                        var listTransfert = (searching != null) ? listTransfertFiltered.Where(a => a.nomCompletpatient.ToUpper().Contains(searching)
                                                                         || a.nomCompletpatient.ToLower().Contains(searching)
                                                                         || a.partageInfoDate.ToString().StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         || a.nomCpmletDocteurPrimaire.ToLower().Contains(searching)
                                                                         || a.nomCpmletDocteurPrimaire.ToUpper().Contains(searching)
                                                                         || a.nomCpmletDocteurSecondaire.ToLower().Contains(searching)
                                                                         || a.nomCpmletDocteurSecondaire.ToUpper().Contains(searching)
                                                                         || a.partageInfoID.ToString().StartsWith(searching, StringComparison.CurrentCultureIgnoreCase)
                                                                         ).OrderByDescending(a => a.partageInfoID).ToPagedList(pageIndex, pageSize)
                                                      : listTransfertFiltered.OrderByDescending(a => a.partageInfoID).ToPagedList(pageIndex, pageSize);

                        reponse.statusCode = 200;
                        reponse.message = "Liste des patients transférés ";
                        reponse.resultat = listTransfert;
                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "la structure pur cette liste  n'existe pas ";
                        reponse.resultat = new List<PatientPartageInfos>().ToPagedList(0, 0);
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de la liste de  patient transféré ";
                    reponse.resultat = new List<PatientPartageInfos>().ToPagedList(0, 0);
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.AnnulerEchangePatientEntreDocteur(long? No)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null)
                    {
                        var patientTransfert = dc.PartageInfos.Where(a => a.partageInfoID == No).FirstOrDefault();
                        if (patientTransfert != null)
                        {
                            patientTransfert.status = false;
                            dc.Entry(patientTransfert).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            var consultation = dc.Consultations.Where(a => a.patientID == patientTransfert.patientID && a.structureID == patientTransfert.structure_primaireID && a.docteurID == patientTransfert.docteur_secondaireID).FirstOrDefault();

                            if (consultation != null)
                            {
                                consultation.docteurID = patientTransfert.docteur_primaireID;
                                dc.Entry(consultation).State = System.Data.Entity.EntityState.Modified;
                                dc.SaveChanges();
                                reponse.statusCode = 200;
                                reponse.message = "Le transfert a été bien annulé ";
                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "La consultation n'existe plus ";

                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Une erreur interne est survenue ";

                        }


                    }

                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Une erreur interne est survenue ";
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'annulation ";
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.SalleAttente(long? structureID, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (structureID != null)
                    {
                        int pageSize = 10;
                        int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var st = structureID;
                        var listConsultationOfUserConnectedStructure = dc.Consultations.Where(a => a.structureID == structureID && a.status == true && a.docteurID == 0).Select(a => a.patientID).Distinct().ToList();
                        var lstPatient = dc.Patients.Where(a => a.structureID == st && a.status == true && a.sexe.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.nom.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.prenom.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.nom.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.profession.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.date_enregistrement.ToString().StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.date_naissance.ToString().StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.adresse.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && a.telephone_patient.StartsWith(searching)
                                                           || a.structureID == st && a.status == true && searching == null
                                                           || a.structureID == st && a.status == true && a.prenom_accompagnant.StartsWith(searching)
                                                           ).ToList();
                        var patientAttente = (from ta in lstPatient
                                              join tb in listConsultationOfUserConnectedStructure on ta.patientID equals tb
                                              select new Patient
                                              {
                                                  patientID = ta.patientID,
                                                  nom = ta.nom,
                                                  prenom = ta.prenom,
                                                  adresse = ta.adresse,
                                                  sexe = ta.sexe,
                                                  telephone_patient = ta.telephone_patient,
                                                  profession = ta.profession,
                                                  prenom_accompagnant = ta.prenom_accompagnant,
                                                  date_enregistrement = ta.date_enregistrement,
                                                  date_naissance = ta.date_naissance
                                              }).ToPagedList(pageIndex, pageSize);

                        reponse.statusCode = 200;
                        reponse.message = "Liste des patients en Attente de docteur ";
                        reponse.resultat = patientAttente;

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette structure n'existe pas ";
                        reponse.resultat = new List<Patient>().ToPagedList(0, 0);
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de la liste ";
                    reponse.resultat = new List<Patient>().ToPagedList(0, 0);
                }


            }
            return reponse;
        }
        Reponse IPatientRepository.ListConsultationPatient(long? structureID, long? No, int? page, string searching)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (structureID != null)
                    {
                        if (No != null)
                        {
                            int pageSize = 10;
                            int pageIndex = 1; pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                            var listConsultationOfUserConnectedStructure = dc.Consultations.Where(a => a.structureID == structureID && a.status == true && a.docteurID == 0 && a.patientID == No).OrderByDescending(a => a.consultationID).ToPagedList(pageIndex, pageSize);
                            reponse.statusCode = 200;
                            reponse.message = "Liste des consultation en Attente pour " + dc.Patients.Where(a => a.patientID == No).Select(s => s.prenom + " " + s.nom).FirstOrDefault();
                            reponse.resultat = listConsultationOfUserConnectedStructure;
                        }

                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet patient n'existe plus ";
                            reponse.resultat = new List<Consultation>().ToPagedList(1, 1);
                        }

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "La structure n'existe plus  ";
                        reponse.resultat = new List<Consultation>().ToPagedList(1, 1);
                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de la liste ";
                    reponse.resultat = new List<Consultation>().ToPagedList(1, 1);
                }


            }
            return reponse;
        }

        Reponse IPatientRepository.RecupererConsultation(long? userConnectedID, long? No)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    if (No != null)
                    {
                        var consultation = dc.Consultations.Where(a => a.consultationID == No).FirstOrDefault();
                        if (consultation != null)
                        {
                            consultation.docteurID = userConnectedID;
                            dc.Entry(consultation).State = System.Data.Entity.EntityState.Modified;
                            dc.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La récuperation a reussi ";
                            reponse.resultat = consultation.patientID;

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "La consultation n'existe plus ";
                        }
                    }

                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "La consultation n'existe plus ";
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recupération ";
                }


            }
            return reponse;
        }


    }
}


