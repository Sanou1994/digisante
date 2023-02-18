using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace digi_sante.Models
{
    public class RecuPaiement
    {

        public float? montantapayer { get; set; }

        public double? Tth { get; set; }
        public double? Tva { get; set; }
        public List<Assurance> assurances { get; set; }

        public Structure structure { get; set; }

    }
    public class analysePatient
    {

        public long id_analyseD { get; set; }
        public long id_consultation { get; set; }
        public long id_structure { get; set; }
        public long id_user { get; set; }
        public DateTime? date_analyse { get; set; }

        public string nom_analyse { get; set; }

    }
    public class NomPatientAssuranceList
    {

        public List<Assurance> listAssurance { get; set; }
        public string nomCompletPatient { get; set; }

    }
    public class PatientTransfert
    {
        public int? idUser { get; set; }

        public string nomComplet { get; set; }
    }
    public class SelectOrdo
    {
        public string nomCompletPatient { get; set; }
        public long? NumberOrdo { get; set; }
        public long? consultationID { get; set; }
        public long? userConnected { get; set; }
        public List<Detail_ordonnance> detailsOrdos { get; set; }

    }
    public class PatientPartageInfos
    {
        public long? partageInfoID { get; set; }
        public DateTime? partageInfoDate { get; set; }
        public string nomCompletpatient { get; set; }
        public string nomCpmletDocteurPrimaire { get; set; }
        public string nomCpmletDocteurSecondaire { get; set; }
    }

    public class AnalyseList
    {
        public string FullNamePatient { get; set; }
        public IPagedList<Analyse> analyses;
        public List<Detail_analyse> detail_analyses { get; set; }
        public IPagedList<Analyse> analysesExtenes { get; set; }
        public List<Utilisateur> utilisateurs { get; set; }
        public string searching { get; set; }

    }
    public class OrdonnanceList
    {
        public string FullNamePatient { get; set; }
        public IPagedList<Ordonnance> ordonnances;
        public List<Detail_ordonnance> detail_ordonnances { get; set; }
        public IPagedList<Ordonnance> ordonnancesExtenes { get; set; }
        public List<Utilisateur> utilisateurs { get; set; }
        public string searching { get; set; }

    }

    public class AnalyseFirst
    {

        public long? userConnected { get; set; }
        public List<Detail_analyse> detail_analyses { get; set; }
        public List<Liste_analyse> liste_analyses { get; set; }
        public long? suis { get; set; }
        public long? Join { get; set; }
        public string nomCompletPatient { get; set; }
        public long? NumberAnalyse { get; set; }
        public long? consultationID { get; set; }

    }

    public class PrintResultat
    {
        public long? No_dossier { get; set; }
        public long? No { get; set; }
        public long? DatePatient { get; set; }
        public string NomComplet { get; set; }
        public string NomCompletDocteur { get; set; }
        public string TitreDocteur { get; set; }
        public string DescriptionResultat { get; set; }
        public string DescriptionCommentaire { get; set; }
        public string structure_adresse { get; set; }
        public string telephone_adresse { get; set; }
        public string email_adresse { get; set; }

        public string NomAnalyse { get; set; }
        public string NomStructure { get; set; }


    }


    public class VoirObservationSuivi
    {
        public long? IdPatient { get; set; }

        public long? NoSuivi { get; set; }
        public Hospitalisation hospitalisation { get; set; }
        public string lastObservation { get; set; }

        public List<ObservationItemSuivi> listeObservationItemSuivis { get; set; }


    }

    public class PrintPrescription
    {
        public long? numeroConsultation { get; set; }
        public string nom_structure { get; set; }
        public string adresse_structure { get; set; }
        public string telephone_structure { get; set; }
        public string email_structure { get; set; }
        public string FullNamePatient { get; set; }
        public DateTime? Date { get; set; }
        public long? No { get; set; }
        public long? NOrd { get; set; }
        public List<Detail_ordonnance> detail_ordonnances { get; set; }
        public Utilisateur utilisateur { get; set; }

    }
    public class PrintAnalyse
    {
        public long? NumeroConsultation { get; set; }
        public string nom_structure { get; set; }
        public string adresse_structure { get; set; }
        public string telephone_structure { get; set; }
        public string email_structure { get; set; }
        public string titre_docteur { get; set; }
        public string FullNameDocteur { get; set; }
        public string adresse_patient { get; set; }
        public string FullNamePatient { get; set; }
        public string date_naissance_patient { get; set; }
        public long? id_patient { get; set; }
        public List<Detail_analyse> detail_analyses { get; set; }
        public List<Liste_analyse> liste_analyses { get; set; }

    }

    public class HoFirst
    {
        public long? suis { get; set; }
        public string FullName { get; set; }
        public Hospitalisation techno { get; set; }

    }
    public class DetailPatient
    {
        public string Name { get; set; }
        public string date_naissance { get; set; }

        public string Surname { get; set; }
        public string Adresse { get; set; }
        public string Nationalite { get; set; }
        public long? Age { get; set; }
        public long? patientID { get; set; }

        public string Gender { get; set; }
        public string Telephone { get; set; }
        public string Profession { get; set; }
        public string Acc { get; set; }
        public long? Birth { get; set; }
        public List<Utilisateur> utilisateurs { get; set; }
        public List<Analyse> analyses { get; set; }
        public List<Utilisateur> liste_docteurs { get; set; }
        public List<Departement> liste_departements { get; set; }
        public List<Assurance> liste_assurances { get; set; }

    }


    public class ListConsultation
    {
        public long? consultationID { get; set; }
        public string nomTraitant { get; set; }
        public string prenomTraitant { get; set; }
        public string departement { get; set; }
        public DateTime? dateDebut { get; set; }
        public DateTime? dateFin { get; set; }
        public bool? isClose { get; set; }
        public long? patientID { get; set; }

    }
    public class ConsultationData
    {
        public long? consultationID { get; set; }
        public string observationPaiement { get; set; }
        public string observationTraitant { get; set; }
        public string constant { get; set; }
        public long? patientID { get; set; }
        public List<Structure> liste_structures { get; set; }

        public List<Liste_analyse> liste_anaylses { get; set; }

    }
    public class ListPatientData
    {
        public long? patientID { get; set; }
        public IPagedList<Patient> patients { get; set; }
        public List<Utilisateur> liste_docteurs { get; set; }
        public List<Consultation> liste_consultations { get; set; }
        public List<Departement> liste_departements { get; set; }
        public List<Assurance> liste_assurances { get; set; }

    }

    public class PatientTransfertTempon
    {
        public int id_patientTansfert { get; set; }
        public DateTime? date_patientTansfert { get; set; }
        public string patient { get; set; }
        public string docteur_primaire { get; set; }
        public string docteur_secondaire { get; set; }
    }

    public class ResultatAnalyse
    {
        public long no_dossier { get; set; }
        public long no_consultation { get; set; }

        public long no_aanalyse { get; set; }
        public string nom_analyse { get; set; }

        public string nom_docteur { get; set; }
        public string nom_complet_patient { get; set; }
        public string date_naissance_patient { get; set; }

        public string date_analyse { get; set; }

        public long? id_resultat { get; set; }
    }
    public class PaiementPatient
    {
        public string Patient { get; set; }
        public string PatientConcat { get; set; }
        public DateTime? date_paiement { get; set; }

        public float? montant_consultation { get; set; }
        public float? montant_espece { get; set; }
        public float? montant_assurance { get; set; }
        public long? paiementID { get; set; }

        public long? consultationID { get; set; }
        public string url_lettre_prise_en_charge { get; set; }
        public string taux_prise_en_charge { get; set; }
        public string departement { get; set; }
        public string NomCompletCaisse { get; set; }
        public string NomCompletCaisseConcat { get; set; }

        public long? assuranceID { get; set; }
        public string assurance { get; set; }
        public long? userID { get; set; }
        public long? structureID { get; set; }
    }

    public class Group<K, T>
    {
        public K Key;
        public IEnumerable<T> Values;
    }

    public class ParametreData
    {      
       
        public List<Departement> liste_departements { get; set; }
        public List<Assurance> liste_assurances { get; set; }

    }


}