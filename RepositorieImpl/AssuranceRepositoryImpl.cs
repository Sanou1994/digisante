using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
namespace digi_sante.Repositories
{
    public class AssuranceRepositoryImpl : IAssuranceRepository
    {
        Reponse IAssuranceRepository.AjouterAssurance(long? structureID, Assurance assurance)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
               try
                {
                    var checkIsAssuranceExiste = db.Assurances.Where(a => (a.nom.Equals(assurance.nom.ToLower()) || a.nom.Equals(assurance.nom.ToUpper())) && a.status == true && a.structureID == assurance.structureID).Select(a => a.nom).Count();
                    var checkIsAssuranceExistePhone = db.Assurances.Where(a => a.telephone == assurance.telephone && a.status == true && a.structureID == assurance.structureID).Select(a => a.telephone).Count();


                    if (checkIsAssuranceExiste > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cet nom d'assurance existe déjà pour cette structure *";
                        reponse.resultat =assurance;
                    }
                    else if (checkIsAssuranceExistePhone > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cet numéro d'assurance existe déjà pour cette structure *";
                        reponse.resultat = assurance;
                    }
                    else
                    {
                       Assurance assuranceSave = db.Assurances.Add(assurance);
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "Enregistrement de l'assurance a reussi";
                        reponse.resultat = assuranceSave;

                    }


                    
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement de l'assurance ";
                    reponse.resultat = null;
                }
                
               
            }
            return reponse;
        }

        Reponse IAssuranceRepository.bloquerAssurance(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if(id != null)
                    {
                        var assurance = db.Assurances.Where(a => a.assuranceID == id && a.status == true).FirstOrDefault();
                        if (assurance != null)
                        {
                            assurance.status = false;
                            db.Entry(assurance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation de l'assurance a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet assurance n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette Assurance n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation duAssurance ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IAssuranceRepository.ListeAssurance(long ? userConnectedID,int? page ,string searching)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if(userConnectedID != null)
                    {

                        var UserNeedList = dc.Utilisateurs.Where(a => a.utilisateurID == userConnectedID && a.status == true).FirstOrDefault();
                        int pageSize = 10;
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                        if (UserNeedList != null)
                        {
                            var st = UserNeedList.structureID;

                            var lstAssurance = dc.Assurances.Where(a => a.structureID == UserNeedList.structureID && a.status == true && a.prenom_a_contacter.StartsWith(searching)
                                                    || a.structureID == UserNeedList.structureID && a.status == true && a.nom.StartsWith(searching)
                                                    || a.structureID == UserNeedList.structureID && a.status == true && a.adresse.StartsWith(searching)
                                                    || a.structureID == UserNeedList.structureID && a.status == true && a.telephone.StartsWith(searching)
                                                    || a.structureID == UserNeedList.structureID && a.status == true && searching == null)
                                                     .OrderByDescending(a => a.assuranceID).ToPagedList(pageIndex, pageSize);
                          
                                reponse.statusCode = 200;
                                reponse.message = "Liste des assurances pour ce " + UserNeedList.titre +" " + UserNeedList.role;
                                reponse.resultat = lstAssurance;
                                          

                       }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "La liste est vide car ce "+ UserNeedList .titre+ " n'existe pas";
                            reponse.resultat = null;
                        }


                    }
                    else
                    {

                        reponse.statusCode = 201;
                        reponse.message = "Cet compte n'existe pas ";
                        reponse.resultat = null;


                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement de l'assurance ";
                    reponse.resultat = null;
                }


            }
            return reponse;

         }

        Reponse IAssuranceRepository.ModifierAssurance(Assurance assurance)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {

                try
                {
                    var checkIsAssuranceExiste = db.Assurances.Where(a => (a.nom.Equals(assurance.nom.ToLower()) || a.nom.Equals(assurance.nom.ToUpper())) && a.status == true && a.structureID == assurance.structureID && a.assuranceID != assurance.assuranceID).Select(a => a.nom).Count();
                    var checkIsAssuranceExistePhone = db.Assurances.Where(a => a.telephone == assurance.telephone && a.status == true && a.structureID == assurance.structureID && a.assuranceID != assurance.assuranceID).Select(a => a.telephone).Count();


                    if (checkIsAssuranceExiste > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cet nom d'assurance existe déjà pour cette structure *";
                        reponse.resultat = assurance;
                    }
                    else if (checkIsAssuranceExistePhone > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cet numéro d'assurance existe déjà pour cette structure *";
                        reponse.resultat = assurance;
                    }
                    else
                    {
                        db.Entry(assurance).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "La modification de l'assurance a reussi";
                        reponse.resultat = assurance;

                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement de l'assurance ";
                    reponse.resultat = null;
                }

            }
            return reponse;
        }

        Reponse IAssuranceRepository.ChercherAssurance(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var assurance = db.Assurances.Where(a => a.assuranceID == id && a.status == true).FirstOrDefault();
                        if (assurance != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat =assurance;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.resultat = id;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.resultat = id;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

      
    }
}


