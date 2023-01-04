using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IListeAnalyseRepository
    {
        Reponse ListeAnalyse(long? structureID, int? size, string searching);
        Reponse ChercherListeAnalyse(long? id);
        Reponse AjouterListeAnalyse(long? structureID,Liste_analyse listeAnalyse);
        Reponse ModifierListeAnalyse(Liste_analyse listeAnalyse);
        Reponse bloquerListeAnalyse(long? id);
    }
}
