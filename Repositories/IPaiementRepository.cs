using digi_sante.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace digi_sante.Repositories
{
    public interface IPaiementRepository
    {
        Reponse ListePaiement(long? userConnectedID, long? structureID, int? size, string searching);
        Reponse ChercherPatientPourPaiement(long? id, long? structureID);
        Reponse AjouterPaiement(Paiement pp, HttpPostedFileBase file, Consultation cons);
        Reponse recuPaiement(long? No);
        Reponse voirPriseEncharge(long? paiementID);
    }
}
