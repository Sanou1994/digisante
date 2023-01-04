using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digi_sante.Repositories
{
  public interface IAuthentificationRepository
    {
        Reponse Seconnecter(string phone, string pwd);
        Reponse activationCode(long? UserConnectedID, string code);
        
    }
}
