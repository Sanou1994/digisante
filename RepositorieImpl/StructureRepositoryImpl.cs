using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.Web.UI;

namespace digi_sante.Repositories
{
    public class StructureRepositoryImpl : IStructureRepository
    {
        Reponse IStructureRepository.AjouterStructure(long? utilisateurID, Structure structure,Utilisateur utilisateur)
        {
            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
               try
                {

                    if (utilisateurID != null)
                    {
                        var structureSizeStart = dc.Structures.Count();

                        int telephoneExist = dc.Structures.Where(a => a.telephone != null && a.telephone.Equals(structure.telephone)).Select(b=>b.telephone).Count();
                        int emailExist = dc.Structures.Where(a =>a.email != null && a.email.Equals(structure.email)).Select(b => b.email).Count();

                        if(telephoneExist > 0)
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet numéro de téléphone existe déjà pour une autre struture ";
                            reponse.resultat = null;
                        }
                        else if(emailExist > 0)
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cet email  déjà pour une autre struture ";
                            reponse.resultat = null;
                        }
                        else
                        {


                            var structureSave = dc.Structures.Add(structure);
                            var result = dc.SaveChanges();
                            var structureSizeEnd = dc.Structures.Count();

                            if (structureSizeEnd > structureSizeStart)
                            {

                                // LA PARTIE DE CREATION DE MANAGER
                                int checkphone = dc.Utilisateurs.Where(a => a.telephone == utilisateur.telephone).Select(a => a.telephone).Count();
                                int checkEmail = dc.Utilisateurs.Where(a => a.email != null && a.email == utilisateur.email).Select(a => a.email).Count();
                                int checkUser = dc.Utilisateurs.Where(a => a.nom == utilisateur.nom && a.prenom == utilisateur.prenom && a.email == utilisateur.email).Select(a => a.role).Count();
                                var currentStructureCreate = dc.Structures.Where(a => a.structureID == structureSave.structureID).FirstOrDefault();
                                if (currentStructureCreate != null)
                                {


                                    if (checkphone > 0)
                                    {
                                        dc.Structures.Remove(currentStructureCreate);
                                        dc.SaveChanges();
                                        reponse.statusCode = 201;
                                        reponse.message = "Ce numéro de téléphone existe déja";
                                        reponse.resultat = null;

                                    }
                                    else if (checkEmail > 0)
                                    {
                                        dc.Structures.Remove(currentStructureCreate);
                                        dc.SaveChanges();
                                        reponse.statusCode = 201;
                                        reponse.message = "Cet email  existe déja";
                                        reponse.resultat = null;

                                    }
                                    else if (checkUser > 0)
                                    {
                                        dc.Structures.Remove(currentStructureCreate);
                                        dc.SaveChanges();
                                        reponse.statusCode = 201;
                                        reponse.message = "Cette utilisateur  existe déja";
                                        reponse.resultat = null;

                                    }
                                    else
                                    {
                                        var tableUserSizeStart = dc.Utilisateurs.Count();
                                        utilisateur.structureID = currentStructureCreate.structureID;
                                        var userCreate = dc.Utilisateurs.Add(utilisateur);
                                        dc.SaveChanges();
                                        var tableUserSizeEnd = dc.Utilisateurs.Count();
                                        // AJOUTER UNE LIGNE POUR LA TABLE D'ASSOCIATION UTILISATEUR_STRUCTURE(ONE TO MANY)
                                        if (tableUserSizeEnd > tableUserSizeStart)
                                        {
                                            var tableStrcutureUtilisateursSizeBefore = dc.Structure_Utilisateurs.Count();
                                            var st = new Structure_Utilisateur
                                            {
                                                structureID = currentStructureCreate.structureID,
                                                utilisateurID = utilisateurID
                                            };
                                            var strc_user_current = dc.Structure_Utilisateurs.Add(st);
                                            dc.SaveChanges();
                                            var tableStrcutureUtilisateursSizeAfter = dc.Structure_Utilisateurs.Count();
                                            if (tableStrcutureUtilisateursSizeBefore < tableStrcutureUtilisateursSizeAfter)
                                            {
                                                reponse.statusCode = 200;
                                                reponse.message = "*La structure de santé enregistré avec succès";
                                                reponse.resultat = null;

                                            }
                                            else
                                            {
                                                dc.Structure_Utilisateurs.Remove(strc_user_current);
                                                dc.Utilisateurs.Remove(userCreate);
                                                dc.Structures.Remove(currentStructureCreate);
                                                dc.SaveChanges();
                                                reponse.statusCode = 201;
                                                reponse.message = "*une erreur s'est produit lors de l'enregistrement";
                                                reponse.resultat = null;
                                            }


                                        }
                                        else
                                        {
                                            dc.Utilisateurs.Remove(userCreate);
                                            dc.Structures.Remove(currentStructureCreate);
                                            dc.SaveChanges();
                                            reponse.statusCode = 201;
                                            reponse.message = "Une erreur s'est produit lors de l'enregistrement";
                                            reponse.resultat = null;

                                        }


                                    }
                                }
                                else
                                {
                                    reponse.statusCode = 201;
                                    reponse.message = "La structure crée recemment a été supprimée";
                                    reponse.resultat = null;

                                }

                            }
                            else
                            {
                                reponse.statusCode = 201;
                                reponse.message = "impossible d'enregistrer cette structure ";
                                reponse.resultat = null;

                            }

 

                        }



                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce compte connecté n'existe plus ";
                        reponse.resultat = null;

                    }


                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du Structure ";
                    reponse.resultat = null;
                } 
                
               
            }
            return reponse;
        }

        Reponse IStructureRepository.bloquerStructure(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if(id != null)
                    {
                        var structure = db.Structures.Where(a => a.structureID == id && a.status == true).FirstOrDefault();
                        if (structure != null)
                        {
                            structure.status = false;
                            db.Entry(structure).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            reponse.statusCode = 200;
                            reponse.message = "La désactivation de la structure a reussi";
                            reponse.resultat = true;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Cette structure n'existe pas ";
                            reponse.resultat = false;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Cette structure n'existe pas ";
                        reponse.resultat = false;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la désactivation de cette tructure ";
                    reponse.resultat = false;
                }


            }
            return reponse;
        }

        Reponse IStructureRepository.ListeStructure(long? structureID, int? page ,string searching)
        {

            Reponse reponse = new Reponse();

            using (var dc = new DigiSante())
            {
                try
                {

                    if(structureID != null)
                    {

                       
                        int pageSize = 10;
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        var listeStruc_Utilisateur = dc.Structure_Utilisateurs.ToList();                        
                        
                        var ListeStructures = (from str_utl in listeStruc_Utilisateur
                                               join utilisateur in dc.Utilisateurs.ToList() on str_utl.utilisateurID equals utilisateur.utilisateurID
                                               where utilisateur.structureID == structureID
                                               join structure in dc.Structures.ToList() on str_utl.structureID equals structure.structureID

                                               select new Structure
                                               {
                                                   adresse = structure.adresse,
                                                   date_creation = structure.date_creation,
                                                   email = structure.email,
                                                   nom = structure.nom,
                                                   status = structure.status,
                                                   status_premiere_connexion_manager = structure.status_premiere_connexion_manager,
                                                   structureID = structure.structureID,
                                                   telephone = structure.telephone,
                                                   type = structure.type
                                               }
                                             );


                        var lstStructure =(searching != null)? dc.Structures.Where(a =>a.status == true && a.adresse.StartsWith(searching)
                                                                || a.status == true && a.email.StartsWith(searching)
                                                                ||a.status == true && a.nom.StartsWith(searching)
                                                                || a.status == true && a.telephone.StartsWith(searching)
                                                                ).OrderByDescending(a => a.structureID).ToPagedList(pageIndex, pageSize)
                                                             : ListeStructures.OrderByDescending(a => a.structureID).ToPagedList(pageIndex, pageSize);
                        reponse.statusCode = 200;
                        reponse.message = "Liste des Structures";
                        reponse.resultat = lstStructure;


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
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du Structure ";
                    reponse.resultat = null;
                }


            }
            return reponse;

         }

        Reponse IStructureRepository.ModifierStructure(Structure structure)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {

                try
                {

                    int telephoneIsExiste = db.Structures.Where(a =>a.telephone !=null && a.telephone == structure.telephone && a.structureID != structure.structureID ).Select(a => a.telephone).ToList().Count();
                    int emailIsExiste = db.Structures.Where(a => a.email != null && a.email == structure.email && a.structureID != structure.structureID).Select(a => a.email).ToList().Count();


                    if (telephoneIsExiste > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Ce numéro de téléphone est déjà utilisé par une Structure*";
                        reponse.resultat = structure;
                    }
                    else if (emailIsExiste > 0)
                    {
                        reponse.statusCode = 201;
                        reponse.message = "* Cet email  est déjà utilisé par une Structure*";
                        reponse.resultat = structure;
                    }
                    else
                    {
                        db.Entry(structure).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        reponse.statusCode = 200;
                        reponse.message = "La modification de la Structure a reussi";
                        reponse.resultat = structure;

                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de l'enregistrement du Structure ";
                    reponse.resultat = null;
                }

            }
            return reponse;
        }

        Reponse IStructureRepository.ChercherStructure(long? id)
        {
            Reponse reponse = new Reponse();

            using (var db = new DigiSante())
            {
                try
                {
                    if (id != null)
                    {
                        var Structure = db.Structures.Where(a => a.structureID == id && a.status == true).FirstOrDefault();
                        if (Structure != null)
                        {
                            reponse.statusCode = 200;
                            reponse.resultat = Structure;
                        }
                        else
                        {
                            reponse.statusCode = 201;
                            reponse.message = "Ce Structure n'existe pas ";
                            reponse.resultat = id;
                        }


                    }
                    else
                    {
                        reponse.statusCode = 201;
                        reponse.message = "Ce Structure n'existe pas ";
                        reponse.resultat = id;
                    }
                }
                catch
                {
                    reponse.statusCode = 500;
                    reponse.message = "Une erreur est survenue lors de la recherche du Structure ";
                    reponse.resultat = null;
                }


            }
            return reponse;
        }

      
    }
}

