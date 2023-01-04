using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IDepartementRepository
    {
        Reponse ListeDepartement(long? userConnectedID, int? size, string searching);
        Reponse ChercherDepartement(long? id);
        Reponse AjouterDepartement(long? structureID, Departement departement);
        Reponse ModifierDepartement(Departement departement);
        Reponse bloquerDepartement(long? id);
    }
}
