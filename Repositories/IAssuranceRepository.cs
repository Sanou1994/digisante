using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IAssuranceRepository
    {
        Reponse ListeAssurance(long? userConnectedID, int? size, string searching);
        Reponse ChercherAssurance(long? id);
        Reponse AjouterAssurance(long? structureID, Assurance assurance);
        Reponse ModifierAssurance(Assurance assurance);
        Reponse bloquerAssurance(long? id);
    }
}
