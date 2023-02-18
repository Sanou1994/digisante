using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
namespace digi_sante.Repositories
{
    public class DepartementRepositoryImpl : IDepartementRepository
    {
        Reponse IDepartementRepository.AjouterDepartement(long? structureID, Departement departement)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
               try
                {
                    int listDepartementSize = db.Departements.Where(a => a.structureID == structureID && a.nom.Equals(departement.nom)).Select(a => a.nom).ToList().Count();
                    if (listDepartementSize != 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Le département existe déjà pour cette structure *";
                        reponse.resultat = departement;
                    }
                    else
                    {
                        Departement DepartementSave = db.Departements.Add(departement);
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "Enregistrement du Departement a reussi";
                        reponse.resultat = DepartementSave;

                    }


                    
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du Departement ";
                    reponse.resultat = null;
                }
                
               
            }
            return reponse;
        }

        Reponse IDepartementRepository.bloquerDepartement(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if(id != null)
                    {
                        var Departement = db.Departements.Where(a => a.departementID == id && a.status == true).FirstOrDefault();
                        if (Departement != null)
                        {
                            Departement.status = false;
                            db.Entry(Departement).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation du Departement a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce Departement n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce Departement n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation du Departement ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IDepartementRepository.ListeDepartement(long ? userConnectedID,int? page ,string searching)
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
                                var lstDepartement = dc.Departements.Where(a => a.structureID == st && a.status == true && a.nom.StartsWith(searching)
                                                                             || a.structureID == st && a.status == true && searching == null
                                                                              ).OrderByDescending(a => a.departementID).ToPagedList(pageIndex, pageSize);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 

                                reponse.statusCode = 200;
                                reponse.message = "Liste des Départements pour ce " + UserNeedList.titre;
                                reponse.resultat = lstDepartement;
                                          

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
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du Département ";
                    reponse.resultat = null;
                }


            }
            return reponse;

         }

        Reponse IDepartementRepository.ModifierDepartement(Departement departement)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    int listDepartementSize = db.Departements.Where(a => a.structureID == departement.structureID && a.departementID != departement.departementID && a.nom.Equals(departement.nom)).Select(a => a.nom).ToList().Count();
                    if (listDepartementSize != 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Le département existe déjà pour cette structure *";
                        reponse.resultat = departement;
                    }
                    else
                    {
                        db.Entry(departement).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "La modification du Departement a reussi";
                        reponse.resultat = departement;

                    }


                   
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la modification du Departement ";
                    reponse.resultat = departement;
                }


            }
            return reponse;
        }

        Reponse IDepartementRepository.ChercherDepartement(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var Departement = db.Departements.Where(a => a.departementID == id && a.status == true).FirstOrDefault();
                        if (Departement != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat = Departement;
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



