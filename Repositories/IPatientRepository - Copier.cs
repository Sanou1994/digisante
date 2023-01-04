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
        Reponse ModifierPatient(Patient patient);
        Reponse bloquerPatient(long? id);
        Reponse ListeConsultation(long? No, long? structureID, int? size, string searching);
        Reponse Consultation(long? No, long? structureID);
        Reponse ajouterOrdonnance(IEnumerable<Detail_ordonnance> things, Ordonnance dd);
        Reponse ajouterAnalyse(IEnumerable<Detail_analyse> things, Analyse dd);
        Reponse ajouterhospitalisation(Hospitalisation ho);


    }
}
