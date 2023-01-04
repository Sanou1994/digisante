using digi_sante.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace digi_sante.Repositories
{
    public class AuthentificationRepositoryImpl : IAuthentificationRepository
    {
        Reponse IAuthentificationRepository.Seconnecter(string phone, string pwd)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {
                    var checkUser1 = dc.Utilisateurs.Where(a => a.telephone == phone && a.status == true).Select(a => a.utilisateurID).FirstOrDefault();
                    if (checkUser1 > 0)
                    {
                        var checkUser = dc.Utilisateurs.Where(a => a.telephone == phone && a.status == true).FirstOrDefault();
                        var checkStructure = dc.Utilisateurs.Where(a => a.structureID == checkUser.structureID && a.status == true).Select(b => b.structureID).FirstOrDefault();
                        if (checkStructure > 0)
                        {
                            var takepassword = BCrypt.Net.BCrypt.Verify(pwd, checkUser.pwd);
                            if (takepassword)
                            {
                                Random dm = new Random();
                                int rnd = dm.Next(1000, 9999);
                                Code cc = new Code{
                                    CodeID = rnd,
                                    status = true,
                                    Codes=rnd,
                                    userConnectedID= checkUser.utilisateurID
                                };
                               
                                var code= dc.Codes.Add(cc);
                                dc.SaveChanges();
                                //    //Fayen.Where(a => a.thank == THANKS).Select(a => a.you).FirstOrDefault();

                                /*                 string message = "Bonjour cher(e) professionel(le) de sante veuillez vous connecter en entrant le code d'activation suivant : " + rnd + ", nous vous souhaitons une excellente journee, Equipe HOSTO";
                                                 var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://41.214.66.206:65/api/SendSMS?tel=" + ph + "&sms=" + message);
                                                 httpWebRequest.ContentType = "application/json";
                                                 httpWebRequest.Method = "POST";

                                                 using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                                 {

                                                     string jsont = "{\"tel\":\"" + ph + "\"," +
                                                                "\"sms\":\"" + message + "}";

                                                     streamWriter.WriteLine();
                                                 }
                                                 try
                                                 {
                                                     var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                                     if (httpResponse.StatusCode.GetHashCode().Equals(200))
                                                     {
                                                      return RedirectToAction("Activation", "Home");
                                                     }



                                                     else
                                                     {
                                                         ViewBag.Message = "Envoi SMS refusé";
                                                         return View();
                                                     }
                                                 }
                                                 catch (WebException)
                                                 {
                                                     ViewBag.Message = "Envoi SMS refusé";
                                                     return View();
                                                 } */
                                reponse.statusCode = 200;
                                reponse.message = "La connexion a reussi  ";
                                reponse.resultat = code;

                            }
                            else
                            {
                               
                                reponse.statusCode = 201;
                                reponse.message = "Mot de passe incorrect  ";
                                reponse.resultat = pwd;
                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce compte n'appartient pas à une structure  ";
                            reponse.resultat = phone;
                            
                        }

                    }
                    else
                    {
                       
                        reponse.statusCode = 201;
                        reponse.message = "Ce compte n'existe pas  ";
                        reponse.resultat = phone;
                    }

                }
                catch (Exception)
                {
                    reponse.statusCode = 500;
                    reponse.message = "Impossible de vous connecter  ";
                    reponse.resultat = phone;                  
                }


            }
            return reponse;
        }

        Reponse IAuthentificationRepository.activationCode(long? UserConnectedID,string code)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    int cd = Convert.ToInt32(code);
                    if(UserConnectedID != null)
                    {
                        var userConnected = dc.Utilisateurs.Where(a => a.utilisateurID == UserConnectedID && a.status == true).FirstOrDefault();

                        if (userConnected != null)
                        {
                            var checkCodeId = dc.Codes.Where(a => a.Codes == cd && a.status == true && a.userConnectedID == UserConnectedID).Select(a => a.CodeID).FirstOrDefault();
                            if (checkCodeId > 0)
                            {
                                var checkCode = dc.Codes.Where(a => a.Codes == cd && a.status == true && a.userConnectedID == UserConnectedID).FirstOrDefault();
                                if(checkCode != null)
                                {

                                    checkCode.status = false;
                                    dc.Entry(checkCode).State = System.Data.Entity.EntityState.Modified;
                                    dc.SaveChanges();

                                    if (userConnected.validite_compte_temporaire == true)
                                    {
                                        var transfertPatient = dc.PartageInfos.Where(a => a.status == true && a.docteur_secondaireID == userConnected.utilisateurID && a.structure_primaireID == userConnected.structureID).FirstOrDefault();

                                        switch (transfertPatient.type_dossier)
                                        {
                                            case "ANALYSE":
                                                {
                                                    if (transfertPatient.id_choisi != null)
                                                    {

                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une seule analyse  ";
                                                        reponse.resultat = new { action = "VoirAnalyse", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.id_choisi } };
                                                    }
                                                    else
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une liste analyse  ";
                                                        reponse.resultat = new { action = "AnalyseList", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.patientID * 785 } };

                                                    }
                                                }; break;

                                            case "SUIVI":
                                                {
                                                    if (transfertPatient.id_choisi != null)
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une seule suivie  ";
                                                        reponse.resultat = new { action = "VoirObservationUneSuivi", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.patientID * 785, idSuivi = transfertPatient.id_choisi } };

                                                    }
                                                    else
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une liste de suivie  ";
                                                        reponse.resultat = new { action = "SuiviPatient", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.id_choisi } };

                                                    }
                                                }; break;

                                            case "ORDONNANCE":
                                                {
                                                    if (transfertPatient.id_choisi != null)
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une seule ordonnance  ";
                                                        reponse.resultat = new { action = "VoirAnalyse", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.id_choisi } };

                                                    }
                                                    else
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Le visiteur est invité à visualiser une liste d'ordonnance  ";
                                                        reponse.resultat = new { action = "OrdonnanceList", userConnected = userConnected, controller = "Visiteur", param = new { No = transfertPatient.patientID * 785 } };

                                                    }
                                                }; break;

                                            default:
                                                {
                                                    reponse.statusCode = 200;
                                                    reponse.message = "aucun choix n'a été trouvé  ";
                                                    reponse.resultat = new { action = "index", controller = "Home" , param = "Null" };

                                                }; break;


                                        }

                                    }
                                    else
                                    {

                                        if (userConnected.role.Equals("Manager"))
                                        {
                                            if (userConnected.titre != "Docteur"
                                               && userConnected.titre != "Caissier(e)"
                                               && userConnected.titre != "Infirmier(e)")
                                            {
                                                reponse.statusCode = 200;
                                                reponse.message = "Espace laboratoire";
                                                reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Laboratory", param = "Null" };


                                            }
                                            else
                                            {
                                                reponse.statusCode = 200;
                                                reponse.message = "Espace manager";
                                                reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Admin", param = "Null" };
                                            };
                                        }
                                        else
                                        {
                                            switch (userConnected.titre)
                                            {
                                                case "Docteur":
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Espace Docteur";
                                                        reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Docteur", param = "Null" };
                                                    }; break;

                                                case "Caissier(e)":
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Espace Caisse";
                                                        reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Caisse", param = "Null" };
                                                    }; break;

                                                case "Infirmier(e)":
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Espace Infirmier";
                                                        reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Infirmier", param = "Null" };

                                                    }; break;
                                                default:
                                                    {
                                                        reponse.statusCode = 200;
                                                        reponse.message = "Espace Laboratory";
                                                        reponse.resultat = new { action = "index", userConnected = userConnected, controller = "Laboratory", param = "Null" };

                                                    }; break;
                                            }
                                        }

                                    }


                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "Code n'existe pas";
                                }
                                
                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "Code Incorrect";

                            }
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet utilisateur n'existe pas ";
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cet utilisateur n'existe pas ";
                    }
                  
                    return reponse;


                }
                catch (Exception)
                {
                    reponse.statusCode = 500;
                    reponse.message = "Code Incorrect";
                    return reponse;
                }

            }


        }
    }
}


