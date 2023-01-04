using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IStructureRepository
    {
        Reponse ListeStructure( long? structureID, int? size, string searching);
        Reponse ChercherStructure(long? id);
        Reponse AjouterStructure(long? utilisateurID,  Structure structure,Utilisateur utilisateur);
        Reponse ModifierStructure(Structure structure);
        Reponse bloquerStructure(long? id);
    }
}
