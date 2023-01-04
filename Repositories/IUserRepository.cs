using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IUserRepository
    {
        Reponse ListeUser(long? userConnectedID, int? size, string searching);
        Reponse ChercherUser(long? id);
        Reponse AjouterUser(long? structureID, Utilisateur user);
        Reponse ModifierUser(Utilisateur user);
        Reponse bloquerUser(long? id);
    }
}
