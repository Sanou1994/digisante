using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
namespace digi_sante.Repositories
{
    public class ListeAnalyseRepositoryImpl : IListeAnalyseRepository
    {
        Reponse IListeAnalyseRepository.AjouterListeAnalyse(long? structureID, Liste_analyse listeAnalyse)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
               try
                {
                    int listListeAnalyseSize = db.Liste_analyses.Where(a => a.structureID == structureID && a.nom.Equals(listeAnalyse.nom)).Select(a => a.nom).ToList().Count();
                    if (listListeAnalyseSize != 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Le département existe déjà pour cette analyse *";
                        reponse.resultat = listeAnalyse;
                    }
                    else
                    {
                        Liste_analyse listeAnalyseSave = db.Liste_analyses.Add(listeAnalyse);
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "Enregistrement de cette analyse a reussi";
                        reponse.resultat = listeAnalyseSave;
                        ;

                    }


                    
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement de cette analyse ";
                    reponse.resultat = null;
                }
                
               
            }
            return reponse;
        }

        Reponse IListeAnalyseRepository.bloquerListeAnalyse(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if(id != null)
                    {
                        var ListeAnalyse = db.Liste_analyses.Where(a => a.liste_analyseID == id).FirstOrDefault();
                        if (ListeAnalyse != null)
                        {
                            ListeAnalyse.status = false;
                            db.Entry(ListeAnalyse).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation de cette analyse a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cette analyse n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette analyse n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation de cette analyse ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IListeAnalyseRepository.ListeAnalyse(long? structureID, int? page ,string searching)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if(structureID != null)
                    {

                        var UserNeedList = dc.Utilisateurs.Where(a => a.structureID == structureID && a.status == true).FirstOrDefault();
                        int pageSize = 10;
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var lstListeAnalyse = dc.Liste_analyses.Where(a => a.structureID == structureID && a.status == true && a.nom.StartsWith(searching)
                                                                             || a.structureID == structureID && a.status == true && searching == null
                                                                              ).OrderByDescending(a => a.liste_analyseID).ToPagedList(pageIndex, pageSize);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 

                         reponse.statusCode = 200;
                         reponse.message = "Liste des analyses" ;
                         reponse.resultat = lstListeAnalyse;
                                          

                      


                    }
                    else
                    {

                        reponse.statusCode = 201;
                        reponse.message = "Cette structure n'existe pas ";
                        reponse.resultat = null;


                    }

                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la récuperation de cette liste d'analyse  ";
                    reponse.resultat = null;
                }


            }
            return reponse;

         }

        Reponse IListeAnalyseRepository.ModifierListeAnalyse(Liste_analyse listeAnalyse)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    int listListeAnalyseSize = db.Liste_analyses.Where(a => a.structureID == listeAnalyse.structureID && a.liste_analyseID != listeAnalyse.liste_analyseID && a.nom.Equals(listeAnalyse.nom)).Select(a => a.nom).ToList().Count();
                    if (listListeAnalyseSize != 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cette analyse existe déjà pour cette structure *";
                        reponse.resultat = listeAnalyse;
                    }
                    else
                    {
                        db.Entry(listeAnalyse).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "La modification  a reussi";
                        reponse.resultat = listeAnalyse;

                    }


                   
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la modification de cette analyse ";
                    reponse.resultat = listeAnalyse;
                }


            }
            return reponse;
        }

        Reponse IListeAnalyseRepository.ChercherListeAnalyse(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var ListeAnalyse = db.Liste_analyses.Where(a => a.liste_analyseID == id && a.status == true).FirstOrDefault();
                        if (ListeAnalyse != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat = ListeAnalyse;
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



