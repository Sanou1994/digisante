using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
namespace digi_sante.Repositories
{
    public class UserRepositoryImpl : IUserRepository
    {
        Reponse IUserRepository.AjouterUser(long? structureID,Utilisateur user)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {

              
                try
                {

                    if (structureID != null)
                    {
                        var checkphone = dc.Utilisateurs.Where(a => a.telephone == user.telephone && a.structureID == structureID).Select(a => a.telephone).FirstOrDefault();
                        var checkEmail = dc.Utilisateurs.Where(a => a.email != null && a.email == user.email && a.structureID == structureID).Select(a => a.email).FirstOrDefault();
                        var checkUser = dc.Utilisateurs.Where(a => a.nom == user.nom && a.prenom == user.prenom && a.role == user.role && a.titre == user.titre && a.structureID == structureID).Select(a => a.role).FirstOrDefault();
                       
                        if (checkphone != null && checkphone.Equals(user.telephone))
                        {
                            reponse.message = "* Ce numéro de téléphone existe déja *";
                            reponse.statusCode = 201;
                            reponse.resultat = user;
                        }
                        else if (checkEmail != null && checkEmail.Equals(user.email))
                        {
                            reponse.message = "* Cette adresse mail existe déja *";
                            reponse.statusCode = 201;
                            reponse.resultat = user;
                        }
                        else if (checkUser != null)
                        {
                            reponse.message = "* Cet Utilisateur existe déja *";
                            reponse.statusCode = 201;
                            reponse.resultat = user;
                        }
                        else
                        {
                            Utilisateur userSave = dc.Utilisateurs.Add(user);
                            dc.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "Enregistrement de l'utilisateur a reussi";
                            reponse.resultat = userSave;
                        }

                    }
                    else
                    {
                        reponse.statusCode = 500;
                        reponse.message = "Cet Utilisateur n'existe pas";
                        reponse.resultat = null;

                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement de l'utilisateur ";
                    reponse.resultat = null;
                }
                
               
            }
            return reponse;
        }

        Reponse IUserRepository.bloquerUser(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if(id != null)
                    {
                        var user = db.Utilisateurs.Where(a => a.utilisateurID == id && a.status == true).FirstOrDefault();
                        if (user != null)
                        {
                            user.status = false;
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation de l'utilisateur a reussi";
                            reponse.resultat = user;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet utilisateur n'existe pas ";
                            reponse.resultat = id;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cet utilisateur n'existe pas ";
                        reponse.resultat = id;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation de l'utilisateur ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

        Reponse IUserRepository.ListeUser(long ? userConnectedID,int? page ,string searching)
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

                        if (UserNeedList != null)
                        {

                            if (UserNeedList.role == "Manager")
                            {

                                var departements = dc.Departements.Where(a => a.structureID == UserNeedList.structureID).OrderByDescending(a => a.departementID).ToList();
                                var users = dc.Utilisateurs.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToList();
                                var listeUtilisateurs = dc.Utilisateurs.Where(a => a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.prenom.StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.nom.StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.titre.StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.role.StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.telephone.ToString().StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.email.StartsWith(searching)
                                                                || a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && searching == null)
                                                                .OrderByDescending(a => a.utilisateurID).ToPagedList(pageIndex, pageSize);
                                var tpl = new Tuple<IPagedList<Utilisateur>, List<Departement>>(listeUtilisateurs, departements);
                                
                                reponse.statusCode = 200;
                                reponse.message = "Liste des utilisateurs ";
                                reponse.resultat = tpl;
                            }
                            else
                            {
                                switch (UserNeedList.titre)
                                {
                                    case "Docteur":
                                        var departements = dc.Departements.Where(a => a.structureID == UserNeedList.structureID).ToList();
                                        var users = dc.Utilisateurs.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToList();
                                        var listeUtilisateurs = dc.Utilisateurs.Where(a =>a.titre=="Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.prenom.StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.nom.StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.titre.StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.role.StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.telephone.ToString().StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && a.email.StartsWith(searching)
                                                                        || a.titre == "Docteur" && a.validite_compte_temporaire == false && a.status == true && a.structureID == UserNeedList.structureID && searching == null)
                                                                        .OrderByDescending(a => a.utilisateurID).ToPagedList(pageIndex, pageSize);
                                        var tpl = new Tuple<IPagedList<Utilisateur>, List<Departement>>(listeUtilisateurs, departements);

                                        reponse.statusCode = 200;
                                        reponse.message = "Liste des utilisateurs ";
                                        reponse.resultat = tpl;


                                        break;
                                    case "Infirmier":
                                        // code block
                                        break;
                                    default:
                                        var listUtilisateursDefault = dc.Utilisateurs.Where(a => a.structureID == UserNeedList.structureID && a.status == true).ToPagedList(pageIndex, pageSize);
                                        var departementsDefault = dc.Departements.Where(a => a.structureID == UserNeedList.structureID).ToList();
                                        var tplDefault = new Tuple<IPagedList<Utilisateur>, List<Departement>>(listUtilisateursDefault, departementsDefault);

                                        reponse.statusCode = 200;
                                        reponse.message = "Liste des utilisateurs ";
                                        reponse.resultat = tplDefault;
                                        break;
                                }

                            }

                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "La liste est vide car ce docteur n'existe pas";
                            reponse.resultat = null;
                        }


                    }
                    else
                    {

                        reponse.statusCode = 201;
                        reponse.message = "Ce docteur n'existe pas ";
                        reponse.resultat = null;


                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recuperation de la liste  des utilisateurs ";
                    reponse.resultat = null;
                }


            }
            return reponse;

         }

        Reponse IUserRepository.ModifierUser(Utilisateur user)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {



                    if (user.utilisateurID > 0)
                    {
                            var checkphone = dc.Utilisateurs.Where(a => a.telephone == user.telephone && a.structureID == user.structureID && a.utilisateurID != user.utilisateurID).Select(a => a.telephone).FirstOrDefault();
                            var checkEmail = dc.Utilisateurs.Where(a => a.email != null && a.email == user.email && a.structureID == user.structureID && a.utilisateurID != user.utilisateurID).Select(a => a.email).FirstOrDefault();
                            var checkUser = dc.Utilisateurs.Where(a => a.nom == user.nom && a.prenom == user.prenom && a.role == user.role && a.titre == user.titre && a.telephone == user.telephone && a.structureID == user.structureID && a.utilisateurID != user.utilisateurID).Select(a => a.utilisateurID).FirstOrDefault();


                            if (checkphone != null)
                            {
                                 reponse.statusCode = 201;
                                 reponse.message = "Ce numéro de téléphone existe déja ";
                                 reponse.resultat = user;
                            }
                            else if (checkEmail != null)
                            {                              
                          
                                reponse.statusCode = 201;
                                reponse.message = "Cette adresse mail existe déja ";
                                reponse.resultat = user;
                            }
                            else if (checkUser > 0)
                            {
                               
                                reponse.statusCode = 201;
                                reponse.message = "Cet Utilisateur existe déja  ";
                                reponse.resultat = user;
                           }
                            else
                            {
                                  dc.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                  dc.SaveChanges();
                                  reponse.statusCode = 200;
                                  reponse.message = "La modification de l'utilisateur a reussi";
                                  reponse.resultat = user;
                                
                            }

                       

                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cet Utilisateur n'existe pas  ";
                        reponse.resultat = user;
                    }

                   
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la modification de l'utilisateur ";
                    reponse.resultat = user;
                }


            }
            return reponse;
        }

        Reponse IUserRepository.ChercherUser(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var utilisateur = db.Utilisateurs.Where(a => a.utilisateurID == id && a.status == true).FirstOrDefault();
                        if (utilisateur != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat = utilisateur;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet utilisateur n'existe pas ";
                            reponse.resultat = id;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cet utilisateur n'existe pas ";
                        reponse.resultat = id;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche de l'utilisateur ";
                    reponse.resultat = id;
                }


            }
            return reponse;
        }
    }
}




