using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
    public interface IPatientRepository
    {
        Reponse ListePatient(long? userConnectedID, int? size, string searching);
        Reponse ChercherPatient(long? id);
        Reponse AjouterPatient(Patient patient);
        Reponse DetailPatient(long? id);
        Reponse ModifierPatient(Patient patient);
        Reponse bloquerPatient(long? id);
        Reponse ListeConsultation(long? No, long? structureID, int? size, string searching);
        Reponse Consultation(long? No, long? structureID);
        Reponse Consultation(string Observation, string Note, string ConID);
        Reponse ajouterOrdonnance(IEnumerable<Detail_ordonnance> things, long? consultationID, Ordonnance dd);
        Reponse SelectOrdo(long? userConnectedID, string ordo);
        Reponse PrintPrescription(long? userConnectedID, long? pres);
        Reponse ajouterAnalyse(IEnumerable<Detail_analyse> things, long? consultationID, Analyse dd);
        Reponse ajouterhospitalisation(long? consultationID, long? No, Hospitalisation ho);
        Reponse AnalyseFirst(long? userConnectedID, string analyseID);
        Reponse HoFirst(long? userConnectedID, string hostpi);
        Reponse PrintAnalyse(long? userConnectedID, long? pres);
        Reponse AnalyseList(long? userConnectedID, long? No, int? size, string searching);
        Reponse OrdonnanceList(long? structureID, long? userConnectedID, long? patientID, int? page, string searching);
        Reponse HospitalisationList(long? userConnectedID, long? No, int? size, string searching, DateTime? dateEntree, DateTime? dateSortie);
        Reponse PrintResultat(string patientID, long? analyseID);
        Reponse ConfiguerPolitiqueAccess(long? structureID, long? userConnectedID, long? idPatient, long? DateLimiteLong, long? id_choisi, string Type_dossier, string Phone);
        Reponse VoirObservationSuivi(long? idPatient, long? hospitalisationID);
        Reponse AddObservationToSuivi(string No, string observation);
        Reponse mettreFinHospitalisation(long? id);
        Reponse EchangePatientEntreDocteur(long? structureID, string No, string DeDocteur, string ADocteur);
        Reponse ListTransferts(long? structureID, int? size, string searching);
        Reponse AnnulerEchangePatientEntreDocteur(long? No);
        Reponse RecupererConsultation(long? userConnectedID, long? No);
        Reponse SalleAttente(long? structureID, int? page, string searching);
        Reponse ListConsultationPatient(long? structureID, long? No, int? page, string searching);
    }
}
