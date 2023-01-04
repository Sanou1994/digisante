using digi_sante.Migrations;
using digi_sante.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace digi_sante
{
    public class DigiSante : DbContext
    {
        // Votre contexte a été configuré pour utiliser une chaîne de connexion « DigiSante » du fichier 
        // de configuration de votre application (App.config ou Web.config). Par défaut, cette chaîne de connexion cible 
        // la base de données « digi_sante.DigiSante » sur votre instance LocalDb. 
        // 
        // Pour cibler une autre base de données et/ou un autre fournisseur de base de données, modifiez 
        // la chaîne de connexion « DigiSante » dans le fichier de configuration de l'application.
        public DigiSante()
            : base("name=DigiSante")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DigiSante,Configuration>("DigiSante"));
        }

        // Ajoutez un DbSet pour chaque type d'entité à inclure dans votre modèle. Pour plus d'informations 
        // sur la configuration et l'utilisation du modèle Code First, consultez http://go.microsoft.com/fwlink/?LinkId=390109.
        public virtual DbSet<Analyse> Analyses { get; set; }
        public virtual DbSet<Structure_Utilisateur> Structure_Utilisateurs { get; set; }

        public virtual DbSet<Structure> Structures { get; set; }
        public virtual DbSet<Code> Codes { get; set; }
        public virtual DbSet<Consultation> Consultations { get; set; }
        public virtual DbSet<Departement> Departements { get; set; }
        public virtual DbSet<Detail_analyse> Detail_analyses { get; set; }
        public virtual DbSet<Detail_hospitalisation> Detail_hospitalisations { get; set; }
        public virtual DbSet<Detail_ordonnance> Detail_ordonnances { get; set; }
        public virtual DbSet<Hospitalisation> Hospitalisations { get; set; }
        public virtual DbSet<Liste_analyse> Liste_analyses { get; set; }
        public virtual DbSet<Ordonnance> Ordonnances { get; set; }
        public virtual DbSet<Paiement> Paiements { get; set; }
        public virtual DbSet<PartageInfo> PartageInfos { get; set; }
        
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Resultat> Resultats { get; set; }
        public virtual DbSet<Suivi> Suivis { get; set; }
        public virtual DbSet<Utilisateur> Utilisateurs { get; set; }
        public virtual DbSet<Assurance> Assurances { get; set; }
        public virtual DbSet<ObservationItemSuivi> observationItemSuivis { get; set; }

    }


}